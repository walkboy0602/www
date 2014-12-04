using App.Core.Data;
using App.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;

namespace App.Core.Services
{
    public interface IListingService
    {
        int CreateNew(int UserId);
        int GetNewListing(int UserId);
        Listing GetListingById(int ListingId, int UserId = 0);
        IEnumerable<Listing> GetAll();
        IEnumerable<Listing> GetAllByUserId(int UserId);

        void InsertLog(ListingLog listingLog);

        void Save(Listing listing);
    }

    public class ListingService : IListingService
    {
        private AdsDBEntities db;

        public ListingService()
        {
            db = new AdsDBEntities();
        }

        void IListingService.Save(Listing listing)
        {
            listing.LastUpdate = DateTime.Now;

            db.SaveChanges();
        }

        int IListingService.CreateNew(int UserId)
        {
            Listing listing = new Listing();

            listing.CreateBy = UserId;
            listing.CreateDate = DateTime.Now;

            db.Listings.Add(listing);
            db.SaveChanges();

            return listing.Id;
        }

        int IListingService.GetNewListing(int UserId)
        {
            var listingId = db.Listings
                            .Where(l => l.CreateBy == UserId)
                            .Where(l => l.Status == (int)XtEnum.ListingStatus.New).Select(n => n.Id).FirstOrDefault();

            return listingId;
        }

        Listing IListingService.GetListingById(int ListingId, int UserId = 0)
        {

            var listing = from l in db.Listings
                          where l.Id == ListingId
                          select l;

            if (UserId != 0)
            {
                listing = listing.Where(l => l.CreateBy == UserId);
            }

            return listing.FirstOrDefault();
        }

        IEnumerable<Listing> IListingService.GetAll()
        {
            return db.Listings;
        }

        IEnumerable<Listing> IListingService.GetAllByUserId(int UserId)
        {
            var listing = db.Listings
                            .Where(l => l.CreateBy == UserId);

            return listing;
        }


        void IListingService.InsertLog(ListingLog listingLog)
        {
            db.ListingLogs.Add(listingLog);
            db.SaveChanges();
        }

    }
}
