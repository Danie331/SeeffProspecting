<%@ WebHandler Language="C#" Class="ListingUpdater" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq;

public class ListingUpdater : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        var json = context.Request.Form[0];
        ListingUpdatableFields dataPacket = (ListingUpdatableFields)new JavaScriptSerializer().Deserialize(json, typeof(ListingUpdatableFields));

        switch (dataPacket.instruction)
        {
            case "update_market_share":
                UpdateMarketShareAndAgency(dataPacket);
                break;
            case "update_agency":
                UpdateMarketShareAndAgency(dataPacket);
                break;
            case "update_parent_prop_id":
                UpdateParentPropertyId(dataPacket);
                break;
            case "update_property_address":
                UpdatePropertyAddress(dataPacket);
                break;                            
        } 
    }

    private void UpdatePropertyAddress(ListingUpdatableFields dataPacket)
    {
        using (var ls_base = new LightStoneDataContext())
        {
            var matchedListings = from listing in ls_base.base_datas where listing.property_id == dataPacket.propertyid select listing;
            foreach (var listing in matchedListings)
            {
                listing.property_address = dataPacket.streetName;
                listing.street_or_unit_no = dataPacket.streetOrUnitNo;                                
            }
            
            ls_base.SubmitChanges();
        }
    }    

    private void UpdateParentPropertyId(ListingUpdatableFields dataPacket)
    {
        using (var ls_base = new LightStoneDataContext())
        {
            var matchedListing = (from listing in ls_base.base_datas where listing.unique_id == dataPacket.uniqueid select listing).FirstOrDefault();
            if (matchedListing != null)
            {
                matchedListing.parent_property_id = dataPacket.propertyid;
                ls_base.SubmitChanges();
            }
        }
    }

    private void UpdateMarketShareAndAgency(ListingUpdatableFields dataPacket)
    {
        using (var lsBase = new LightStoneDataContext())
        {
            var matchedListing = (from listing in lsBase.base_datas
                                  where listing.unique_id.Equals(dataPacket.uniqueid)
                                  select listing).FirstOrDefault();

            if (matchedListing != null)
            {
                if (!string.IsNullOrEmpty(dataPacket.marketShareType) && dataPacket.marketShareType != "null")
                {
                    matchedListing.market_share_type = dataPacket.marketShareType.First().ToString();
                    matchedListing.fated = true;
                }
                else
                {
                    matchedListing.market_share_type = null;
                    matchedListing.fated = null;
                }

                matchedListing.agency_id = dataPacket.agencyid.HasValue && dataPacket.agencyid.Value > 0 ?
                                            dataPacket.agencyid.Value : (int?)null;

                lsBase.SubmitChanges();

                // Update the area fating table
                UpdateAreaFatingTable(lsBase, dataPacket.uniqueid);
            }
        }
    }

    private void UpdateAreaFatingTable(LightStoneDataContext lsBase, string uniqueId)
    {
        // Find area id belong to this listing
        var listing = lsBase.base_datas.FirstOrDefault(tran => tran.unique_id == uniqueId);

        if (listing != null)
        {
            var fatedCount = lsBase.base_datas.Where(tran => tran.seeff_area_id == listing.seeff_area_id 
                                                    && tran.fated.HasValue && tran.market_share_type != null).Count();
            var unfatedCount = lsBase.base_datas.Where(tran => tran.seeff_area_id == listing.seeff_area_id
                                                        && (tran.fated == null || tran.market_share_type == null)).Count();

            var areaFatingRecord = lsBase.area_fatings.Where(af => af.area_id == listing.seeff_area_id).FirstOrDefault();
            if (areaFatingRecord != null)
            {
                areaFatingRecord.fated = fatedCount;
                areaFatingRecord.unfated = unfatedCount;
            }

            lsBase.SubmitChanges();
        }   
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    private struct ListingUpdatableFields
    {
        public string instruction;
        
        public string uniqueid;
        public string marketShareType;
        public int? agencyid;
        public int? propertyid;
        public string streetName;
        public string streetOrUnitNo;
    }

}