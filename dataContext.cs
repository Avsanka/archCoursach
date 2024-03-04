using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace course
{
    class dataContext: DbContext
    {
        public dataContext() : base("DbConnection") 
        { }
        public DbSet<part> Parts { get; set; }
    }
}
