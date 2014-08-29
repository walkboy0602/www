using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;

namespace App.Core.Services
{
    public interface IFeatureService
    {
        IEnumerable<ListingFeatureType> Get(string code);
        IEnumerable<ListingFeatureType> GetByGroupCode(string groupCode);

        void AddPurchaseLog(ListingPurchaseLog log);
        void SavePurchaseLog(ListingPurchaseLog log);
    }

    public class FeatureService : IFeatureService
    {
        private AdsDBEntities db;

        public FeatureService()
        {
            this.db = new AdsDBEntities();
        }

        IEnumerable<ListingFeatureType> IFeatureService.Get(string code)
        {
            return db.ListingFeatureTypes.Where(c => c.Code == code);
        }

        IEnumerable<ListingFeatureType> IFeatureService.GetByGroupCode(string groupCode)
        {
            return db.ListingFeatureTypes.Where(t => t.GroupCode == groupCode).ToList();
        }

        void IFeatureService.AddPurchaseLog(ListingPurchaseLog log)
        {
            db.ListingPurchaseLogs.Add(log);
            db.SaveChanges();
        }

        void IFeatureService.SavePurchaseLog(ListingPurchaseLog log)
        {
            db.SaveChanges();
        }
    }
}
