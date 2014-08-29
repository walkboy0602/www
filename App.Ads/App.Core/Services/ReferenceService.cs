using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using App.Core.Data;
using App.Core.Models;

namespace App.Core.Services
{

    public interface IReferenceService
    {
        IEnumerable<RefTable> Get();

        RefTable Find(string code);

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

        RefTable IReferenceService.Find(string code)
        {
            return (this as IReferenceService).Get().Where(r => r.Code == code).FirstOrDefault();
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
                refTables = this.db.RefTables;
                HttpRuntime.Cache.Insert(cacheKey, refTables, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return refTables;
        }

        IEnumerable<RefTable> IReferenceService.GetByType(string type)
        {
            IEnumerable<RefTable> refTables = (this as IReferenceService).Get().Where(r => r.Type == type).Where(r => r.IsActive == true).OrderBy(s => s.Sort);

            return refTables;
        }

    }
}
