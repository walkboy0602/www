using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;

namespace App.Core.Services
{
    public interface IPaymentService
    {
        IEnumerable<ListingType> Get(string code);
        IEnumerable<ListingType> GetByGroupCode(string groupCode);

        void SavePurchaseLog(ListingPurchaseLog log);
    }

    public class PaymentService : IPaymentService
    {
        private AdsDBEntities db;

        public PaymentService()
        {
            this.db = new AdsDBEntities();
        }

        IEnumerable<ListingType> IPaymentService.Get(string code)
        {
            return db.ListingTypes.Where(c => c.Code == code);
        }

        IEnumerable<ListingType> IPaymentService.GetByGroupCode(string groupCode)
        {
            return db.ListingTypes.Where(t => t.GroupCode == groupCode).ToList();
        }

        void IPaymentService.SavePurchaseLog(ListingPurchaseLog log)
        {
            db.ListingPurchaseLogs.Add(log);
            db.SaveChanges();
        }
    }
}
