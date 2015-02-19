<%@ WebHandler Language="C#" Class="AdminDataManager" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Transactions;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class AdminDataManager : IHttpHandler {

    private ObjectCache cache = MemoryCache.Default;
    public void ProcessRequest (HttpContext context) 
    {
        var json = context.Request.Form[0];
        var dataPacket = JsonConvert.DeserializeObject<DataRequestPacket>(json);

        if (cache["areas"] == null)
        {
            UpdateCache();
        }                 

        if (cache["areaTypes"] == null)
        {
            LoadAndCacheAreaTypes();
        }        
        switch (dataPacket.Instruction)
        {          
            case "load_area": 
                ILocation area = LoadArea((int)dataPacket.LocationID, dataPacket.ResCommAgri);
                context.Response.Write(JsonConvert.SerializeObject(area, Domain.CreateDefaultJsonSettings()));
                break; 
            case "load_areas_data":
                context.Response.Write(JsonConvert.SerializeObject(cache["areas"], Domain.CreateDefaultJsonSettings()));
                break;       
            case "find_area_under_location":
                var location = TryFindAreaUnderLatLong(dataPacket.LatLng);
                context.Response.Write(JsonConvert.SerializeObject(location, Domain.CreateDefaultJsonSettings()));
                break;                
            case "add_or_update_area":
                string errMsg ="";
                bool success = CreateOrUpdateArea(dataPacket, ref errMsg);
                var responseObject = new { Status = success ? "success" : errMsg, LocationId = dataPacket.LocationID };
                context.Response.Write(JsonConvert.SerializeObject(responseObject, Domain.CreateDefaultJsonSettings()));
                break;  
            case "get_area_types":
                context.Response.Write(JsonConvert.SerializeObject(cache["areaTypes"], Domain.CreateDefaultJsonSettings()));
                break;                                                   
        }
    }

    private void UpdateCache()
    {
        using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
        {
            var areasAreaTypes = (from a in seeff.areas
                                 join kml in seeff.kml_areas on a.areaId equals kml.area_id
                                 let areaParentName = (from a2 in seeff.areas where a.areaParentId == a2.areaId select a2.areaName).First()
                                  let areaPolyType = kml.area_type == 'R' ? "Residential" : (kml.area_type == 'C' ? "Commercial" : (kml.area_type == 'A' ? "Agricultural" : "Unmapped"))
                                 select new AreaPolyTypeItem
                                 {
                                     AreaId = a.areaId,
                                     FullAreaName = a.areaName + ", " + areaParentName + " (" + areaPolyType + ")",
                                     AreaType = kml.area_type.ToString()                          
                                 }).Distinct();
            var kmlIds = from kml in seeff.kml_areas select kml.area_id;
            areasAreaTypes = areasAreaTypes.Union(
                                                    (from a in seeff.areas
                                                     where !kmlIds.Contains(a.areaId)
                                                    select new AreaPolyTypeItem
                                                    {
                                                        AreaId =a.areaId,
                                                        FullAreaName = string.Concat(a.areaName, ", ", seeff.GetAreaParentName(a.areaId), " (Unmapped)"),
                                                        AreaType = "Unmapped"
                                                    }).Distinct()
                                                );
            //ls_base.sp_generate_area_parent_lookup_table();
            //string json = ls_base.ufn_get_area_parent_lookup_json();
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in areasAreaTypes)
            {
                strBuilder.Append(string.Format("{{Name: \"{0}\", id: \"{1}\", type: \"{2}\"}}", item.FullAreaName, item.AreaId, item.AreaType));
            }
            string json = "[" + strBuilder.ToString().Replace("}{", "},{") + "]";
            
            List<GenericArea> result = JsonConvert.DeserializeObject<List<GenericArea>>(json);
            result = result.OrderBy(ga => ga.LocationName).ToList();
            cache["areas"] = result;    
        }
    }

    private bool CreateOrUpdateArea(DataRequestPacket inputData, ref string errMsg)
    {
        try
        {
            // Determine if area exists. If so update it, otherwise add a new area.
            bool  isNewArea = inputData.LocationID == null;
            if (isNewArea)
            {
                // Get the next area id to use
                using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
                {
                    inputData.LocationID = seeff.areas.Max(a => a.areaId) + 1;
                }
                CreateArea(inputData);
            }
            else
            {
                UpdateArea(inputData);                
            }

            UpdateCache();
            return true;   
        }
        catch (Exception e)
        {
            errMsg = e.Message;
            return false;
        }
    }

    private void CreateArea(DataRequestPacket inputData)
    {
        // TODO: the contents of this method must be done in a transaction as multiple tables are affected!  
        using (var transaction = new TransactionScope())
        {
            using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
            {
            // Insert a record into the area table
            area newAreaRecord = new area
            {
                areaId = (int)inputData.LocationID,
                areaName = inputData.LocationName,
                areaParentId = inputData.ParentLocationId,
                fkAreaTypeId = inputData.AreaTypeId
            };

            // Insert a record into the AreaMap table
            AreaMap am = new AreaMap
            {
                sPath = BuildAreaSPath(seeff, (int)inputData.ParentLocationId, (int)inputData.LocationID),
                fkAreaId = inputData.LocationID,
                fkAreaParentId = inputData.ParentLocationId,
                areaName = inputData.LocationName,
                fkAreaTypeId = inputData.AreaTypeId
            };

                seeff.areas.InsertOnSubmit(newAreaRecord);
                seeff.AreaMaps.InsertOnSubmit(am);

                using (var innerTran = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    using (var ls_base = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        inputData.PolyCoords = Domain.MakePolygonValid(ls_base, inputData.PolyCoords);
                    }
                }                
                
                // Add a new set of records for each poly area type - R - C - A that was chosen in the input
                foreach (var type in inputData.ResCommAgriItemArray)
                {
                    for (int i = 0; i < inputData.PolyCoords.Count; i++)
                    {
                        kml_area newRecord = new kml_area
                        {
                            area_id = (int)inputData.LocationID,
                            area_type = Char.Parse(type),
                            latitude = inputData.PolyCoords[i].Lat,
                            longitude = inputData.PolyCoords[i].Lng,
                            seq = i + 1 // Starts from 1
                        };

                        seeff.kml_areas.InsertOnSubmit(newRecord);
                    }                    
                }                

                // update kml_area, area_layer table, AND set IsNewPoly to false;
                seeff.SubmitChanges();

                using (var innerTran = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    using (var ls_base = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        int? provId = seeff.GetAreaPathMap(inputData.LocationID, 2);
                        foreach (var type in inputData.ResCommAgriItemArray)
                        {
                            area_layer newRecord = new area_layer
                            {
                                area_id = (int)inputData.LocationID,
                                area_type = type,
                                province_id = provId,
                                formatted_poly_coords = Domain.PolyCoordsToString(ls_base, inputData.PolyCoords)
                            };

                            ls_base.area_layers.InsertOnSubmit(newRecord);
                        }
                        ls_base.SubmitChanges();
                    }

                    innerTran.Complete();
                }
            }

            UpdateRelatedAreas(inputData);
            UpdateNeighboringAreas(inputData);

            transaction.Complete();
        }
    }

    private void UpdateArea(DataRequestPacket inputData)
    {
        // TODO: the contents of this method must be done in a transaction as multiple tables are affected!
        using (var transaction = new TransactionScope())
        {
            using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
            {
                var existingArea = seeff.areas.Where(a => a.areaId == inputData.LocationID).First();

                existingArea.areaName = inputData.LocationName;
                existingArea.areaParentId = inputData.ParentLocationId;
                existingArea.fkAreaTypeId = inputData.AreaTypeId;

                seeff.SubmitChanges();

                var existingAreaMap = seeff.AreaMaps.Where(am => am.fkAreaId == inputData.LocationID).First();
                existingAreaMap.sPath = BuildAreaSPath(seeff, (int)inputData.ParentLocationId, (int)inputData.LocationID);
                existingAreaMap.fkAreaParentId = inputData.ParentLocationId;
                existingAreaMap.areaName = inputData.LocationName;
                existingAreaMap.fkAreaTypeId = inputData.AreaTypeId;

                seeff.SubmitChanges();

                // Remove existing kml data for each area type
                foreach (var type in inputData.ResCommAgriItemArray)
                {
                    var existingKml = from kml in seeff.kml_areas
                                      where kml.area_id == inputData.LocationID && kml.area_type == char.Parse(type)
                                      select kml;
                    seeff.kml_areas.DeleteAllOnSubmit(existingKml);
                }
                seeff.SubmitChanges();                                                                  
                
                // Add the most up-to-date kml data
                using (var innerTran = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    using (var ls_base = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        inputData.PolyCoords = Domain.MakePolygonValid(ls_base, inputData.PolyCoords);
                    }
                }

                foreach (var type in inputData.ResCommAgriItemArray)
                {
                    for (int i = 0; i < inputData.PolyCoords.Count; i++)
                    {
                        kml_area newRecord = new kml_area
                        {
                            area_id = (int)inputData.LocationID,
                            area_type = char.Parse(type),
                            latitude = inputData.PolyCoords[i].Lat,
                            longitude = inputData.PolyCoords[i].Lng,
                            seq = i + 1 // Starts from 1
                        };

                        seeff.kml_areas.InsertOnSubmit(newRecord);
                    }
                }
                seeff.SubmitChanges();

                using (var innerTran = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    using (var ls_base = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        int? provId = seeff.GetAreaPathMap(inputData.LocationID, 2);

                        var existingAreaLayerRecords = ls_base.area_layers.Where(al => al.area_id == inputData.LocationID);
                        ls_base.area_layers.DeleteAllOnSubmit(existingAreaLayerRecords);
                        ls_base.SubmitChanges();
                        
                        foreach (var type in inputData.ResCommAgriItemArray)
                        {
                            area_layer newRecord = new area_layer
                            {
                                area_id = (int)inputData.LocationID,
                                area_type = type,
                                province_id = provId,
                                formatted_poly_coords = Domain.PolyCoordsToString(ls_base, inputData.PolyCoords)
                            };

                            ls_base.area_layers.InsertOnSubmit(newRecord);                                                    
                        }
                        ls_base.SubmitChanges();
                    }

                    innerTran.Complete();
                }
            }

            UpdateRelatedAreas(inputData);
            UpdateNeighboringAreas(inputData);
            
            transaction.Complete();
        }
    }

    private void UpdateRelatedAreas(DataRequestPacket inputData)
    {
        using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
        {
            // Delete the old/existing mappings
            var oldRelatedAreas = from a in seeff.related_areas
                                  where a.fkAreaId == inputData.LocationID
                                  select a;
            seeff.related_areas.DeleteAllOnSubmit(oldRelatedAreas);
            seeff.SubmitChanges();

            // Insert the new mappings
            foreach (var newRecord in inputData.RelatedAreas)
            {
                related_area ra = new related_area
                {
                    fkAreaId = (int)inputData.LocationID,
                    fkRelatedAreaId = newRecord
                };

                seeff.related_areas.InsertOnSubmit(ra);
            }

            seeff.SubmitChanges();
        }
    }

    private void UpdateNeighboringAreas(DataRequestPacket inputData)
    {
        using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
        {
            // Delete old/existing neighboring areas
            var oldNeightboringAreas = from a in seeff.neighbouring_areas
                                       where a.fkAreaId == inputData.LocationID
                                       select a;
            seeff.neighbouring_areas.DeleteAllOnSubmit(oldNeightboringAreas);
            seeff.SubmitChanges();

            // Insert new records
            foreach (var newRecord in inputData.NeighboringAreas)
            {
                neighbouring_area na = new neighbouring_area
                {
                    fkAreaId = inputData.LocationID,
                    fkNeighbouringAreaId = newRecord
                };

                seeff.neighbouring_areas.InsertOnSubmit(na);
            }

            seeff.SubmitChanges();
        }
    }

    private string BuildAreaSPath(SeeffDataContext seeff, int parentId, int areaId)
    {
        var partialPath = seeff.AreaMaps.Where(am => am.fkAreaId == parentId).FirstOrDefault();
        if (partialPath == null)
        {
            partialPath = seeff.AreaMaps.Where(am => am.fkAreaParentId == parentId).FirstOrDefault();
        }

        if (partialPath != null)
        {
            return partialPath.sPath + areaId + "|";
        }

        // Throw if we cannot generate the sPath
        throw new Exception("Unable to generate an sPath for for area.");
    }

    private ILocation TryFindAreaUnderLatLong(GeoLocation location)
    {
        using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
        {
            int? result = null;
            foreach (var provId in new[] { 2, 11, 12, 13, 14, 15, 16, 17, 18 })
            {
                result = lsbase.find_area_id(location.Lat, location.Lng, "R", provId);
                if (result > 0)
                {
                    return LoadArea(result.Value, "R");
                }
            }
        }

        return null;
    }

    private ILocation LoadArea(int areaId, string resCommAgri)
    {
        ILocation area = new GenericArea();
        area.LocationID = areaId;
        area.LocationName = ((List<GenericArea>)cache["areas"]).Where(a => a.LocationID == areaId && a.ResCommAgri == resCommAgri).Select(a => a.LocationName).First();
        area.PolyCoords = Domain.LoadPolyCoords(areaId, resCommAgri);
        area.RelatedAreas = Domain.LoadRelatedAreas(areaId);
        area.NeighboringAreas = Domain.LoadNeighboringAreas(areaId);
        area.ParentLocationId = Domain.GetParentAreaId(areaId);
        area.AreaTypeId = Domain.GetAreaTypeId(areaId);
        area.ResCommAgri = Domain.LoadAllSavedPolygonTypes(areaId);

        return area;
    }

    private void LoadAndCacheAreaTypes()
    {
        using (var seeff = DataManager.DataContextRetriever.GetSeeffDataContext())
        {
            var areaTypes = from s in seeff.areaTypes
                            select new AreaType { AreaTypeName =  s.areaTypeName,  AreaTypeId = s.areaTypeId};

            cache["areaTypes"] = areaTypes.ToList();                                                                    
        }
    }


    private T GetItemFromCache<T>(string key)
        where T : class
    {
        if (cache[key] != null)
        {
            return (T)cache[key];
        }

        return null;
    }

    private void InvalidateCacheItem(string key)
    {
        MemoryCache.Default.Remove(key);
    }

    class DataRequestPacket
    {
        public string Instruction { get; set; }

        public int? LocationID { get; set; }
        public GeoLocation LatLng { get; set; }
        public string LocationName { get; set; }
        public List<GeoLocation> PolyCoords { get; set; }
        public int? ParentLocationId { get; set; }
        public int? AreaTypeId { get; set; }

        public List<int> RelatedAreas { get; set; }
        public List<int> NeighboringAreas { get; set; }

        public string ResCommAgri { get; set; }

        public string[] ResCommAgriItemArray
        {
            get { return ResCommAgri.Split(new[] { ',' }); }
        }
    }

    class AreaPolyTypeItem
    {
        public int AreaId { get; set; }
        public string FullAreaName { get; set; }
        public string AreaType { get; set; }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}