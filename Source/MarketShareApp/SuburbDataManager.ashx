<%@ WebHandler Language="C#" Class="SuburbDataManager" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Globalization;

public class SuburbDataManager : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        int suburbId = int.Parse(context.Request.Form[0]);

        ILocation suburb = new GenericArea();
        suburb.LocationID = suburbId;

        var spatialReader = new DataManager.SeeffSpatial.SpatialServiceReader();
        var spatialSuburb = spatialReader.LoadSuburb(suburbId);

        suburb.PolyCoords = Domain.LoadPolyCoords(spatialSuburb.PolyWKT);
            suburb.UnderMaintenance = spatialSuburb.UnderMaintenance;
        List<LightstoneListing> lightstoneListings = Domain.LoadLightstoneListingsForSuburb(suburbId);
        //List<SeeffListing> currentSeeffListings = Domain.LoadCurrentSeeffListings(suburbId);

        // The Listings property expects only a List<IListing> so we need to upcast both lists here
        suburb.Listings = lightstoneListings.Cast<IListing>().ToList();
        //suburb.Listings.AddRange(currentSeeffListings);
        //suburb.LocationName = spatialSuburb.AreaName;

        var serializer = new JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        string json = serializer.Serialize(suburb);

        context.Response.Write(json);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}