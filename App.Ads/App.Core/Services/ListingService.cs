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
using LinqKit;
using System.Linq;

namespace App.Core.Services
{
    public interface IListingService
    {
        int CreateNew(int UserId);
        int GetNewListing(int UserId);
        Listing GetListingById(int ListingId, int UserId = 0);
        IEnumerable<Listing> GetAll();
        IEnumerable<Listing> GetAllByUserId(int UserId);
        IQueryable<Listing> GetLatest(int PageSize);

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

        IQueryable<Listing> IListingService.GetLatest(int PageSize)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var predicate = PredicateBuilder.True<Listing>();

            predicate = predicate.And(p => p.Status == (int)App.Core.ViewModel.XtEnum.ListingStatus.Online);

            predicate = predicate.And(p => p.PostingEndDate >= DateTime.Now);

            var results = db.Listings
                            .Include("Location")
                            .Include("Area")
                            .Include("RefCategory")
                            .Include("ListingImages")
                            .AsExpandable()
                            .Where(predicate)
                            .OrderByDescending(o => o.PostedDate).Take(PageSize);

            return results;

        }

        void IListingService.InsertLog(ListingLog listingLog)
        {
            db.ListingLogs.Add(listingLog);
            db.SaveChanges();
        }

    }
}
