using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.DataContexts
{
    public partial class Entities : DbContext
    {
        public Entities(string connStr) : base(connStr)
        {
        }
    }
}
