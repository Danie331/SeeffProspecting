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
        private const string _testConnectionStringBoss = @"Data Source=LOUISE-PC;Initial Catalog=boss;Integrated security=True;";
        private const string _testConnectionStringLSBase = @"Data Source=LOUISE-PC;Initial Catalog=ls_base;Integrated security=True;";
        private const string _testConnectionStringSeeff = @"Data Source=LOUISE-PC;Initial Catalog=seeff;Integrated security=True;";

        private const string _liveConnectionStringBoss = @"Data Source=41.222.226.213;Initial Catalog=boss;User id=boss_admin;Password=1nePtunE864;";
        private const string _liveConnectionStringLSBase = @"Data Source=41.222.226.215;Initial Catalog=ls_base;User id=marketshare_@dmin;Password=M@R_Je6#s_9;";
        private const string _liveConnectionStringSeeff = @"Data Source=41.222.226.215;Initial Catalog=seeff;User id=marketshare_@dmin;Password=M@R_Je6#s_9;";

        // Stores the active/current connection string of the target environment
        private static string _activeConnectionStringBoss;
        private static string _activeConnectionStringLSBase;
        private static string _activeConnectionStringSeeff;

        static DataContextRetriever()
        {
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                _activeConnectionStringBoss = _testConnectionStringBoss;
                _activeConnectionStringLSBase = _testConnectionStringLSBase;
                _activeConnectionStringSeeff = _testConnectionStringSeeff;
            }
            else
            {
                _activeConnectionStringBoss = _liveConnectionStringBoss;
                _activeConnectionStringLSBase = _liveConnectionStringLSBase;
                _activeConnectionStringSeeff = _liveConnectionStringSeeff;
            }
        }

        public static BossDataContext GetBossDataContext()
        {
            return new BossDataContext(_activeConnectionStringBoss);
        }

        public static LightStoneDataContext GetLSBaseDataContext()
        {
            return new LightStoneDataContext(_activeConnectionStringLSBase);
        }

        public static SeeffDataContext GetSeeffDataContext()
        {
            return new SeeffDataContext(_activeConnectionStringSeeff);
        }
    }
}
