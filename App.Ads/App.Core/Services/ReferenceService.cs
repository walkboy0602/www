using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using App.Core.Data;

namespace App.Core.Services
{

    public interface IReferenceService
    {
        IEnumerable<RefTable> Get();

        IEnumerable<RefTable> GetByType(string type);

    }

    public class ReferenceService : IReferenceService
    {
        private AdsDBEntities db;

        private string cacheKey = "refTables";

        public ReferenceService()
        {
            this.db = new AdsDBEntities();
        }

        IEnumerable<RefTable> IReferenceService.Get()
        {
            IEnumerable<RefTable> refTables = null;

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                refTables = (IEnumerable<RefTable>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheKey, refTables, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return refTables;
        }

        IEnumerable<RefTable> IReferenceService.GetByType(string type)
        {
            IEnumerable<RefTable> refTables = null;

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                refTables = (IEnumerable<RefTable>)HttpRuntime.Cache[cacheKey];
                refTables = refTables.Where(r => r.Type == type);
            }
            else
            {
                refTables = this.db.RefTables.Where(r => r.Type == type);
                HttpRuntime.Cache.Insert(cacheKey, refTables, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return refTables;
        }


    }
}
