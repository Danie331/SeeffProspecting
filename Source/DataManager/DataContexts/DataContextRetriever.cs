using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataManager
{
    public static class DataContextRetriever
    {       
        private const string _liveConnectionStringLSBase = @"Data Source=localhost;Initial Catalog=ls_base;Integrated security=True;";

        public static LightStoneDataContext GetLSBaseDataContext()
        {
            return new LightStoneDataContext(_liveConnectionStringLSBase);
        }
    }
}
