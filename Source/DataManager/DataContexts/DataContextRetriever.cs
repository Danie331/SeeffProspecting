
using DataManager.DataContexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataManager
{
    public static class DataContextRetriever
    {
        private const string _liveConnectionStringLSBase = @"Data Source=localhost;Initial Catalog=ls_base;Integrated security=True;";
        private const string _clientDatabase = @"metadata=res://*/DataContexts.Client.csdl|res://*/DataContexts.Client.ssdl|res://*/DataContexts.Client.msl;provider=System.Data.SqlClient;provider connection string=""data source = localhost; initial catalog = client; integrated security = True; MultipleActiveResultSets=True;App=EntityFramework""";

        public static LightStoneDataContext GetLSBaseDataContext()
        {
            return new LightStoneDataContext(_liveConnectionStringLSBase);
        }
    }    
}
