using App.Core.Models;
using App.Core.Data;
using App.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace App.Core.Services
{
    public interface IImageService
    {
        List<ListingImage> Get(int ListingId, int UserId);
        ListingImage Uploads(int UserId, int ListingId, DateTime CreateDate);
    }

    public class ImageService : IImageService
    {
        private AdsDBEntities db = new AdsDBEntities();

        List<ListingImage> IImageService.Get(int ListingId, int UserId)
        {
            var result = (from l in db.Listings
                          join lb in db.ListingImages on l.id equals lb.ListingId
                          where l.id == ListingId
                          where l.CreateBy == UserId
                          select lb
                              ).ToList();
            return result;

        }


        ListingImage IImageService.Uploads(int UserId, int ListingId, DateTime CreateDate)
        {
            ListingImage listingImage = new ListingImage();

            S3Helper s3 = new S3Helper();

            string YearMonthDay = string.Empty;

            YearMonthDay = CreateDate.Year.ToString() + CreateDate.Month.ToString("d2") + CreateDate.Day.ToString();

            var image = WebImage.GetImageFromRequest();
            string hashName = s3.Hash(image.FileName, ListingId);

            //file format
            //env + listing + yyyymmdd + listingid + size

            string imageURL = string.Join("/", new string[] { s3.env, "listing", YearMonthDay, ListingId.ToString(), "####size####", hashName });

            bool status = false;

            byte[] fileBytes = image.GetBytes();

            //Thumnbnail
            byte[] s0 = s3.CreateImage(fileBytes, image.FileName, 120, 120);
            status = s3.UploadToS3(s0, imageURL.Replace("####size####", "s0"));

            //Standard
            byte[] s1 = s3.CreateImage(fileBytes, image.FileName, 315, 230);
            status = s3.UploadToS3(s1, imageURL.Replace("####size####", "s1"));

            //Large
            byte[] s2 = s3.CreateImage(fileBytes, image.FileName, 640, 480);
            status = s3.UploadToS3(s2, imageURL.Replace("####size####", "s2"));

            listingImage.ListingId = ListingId;
            listingImage.FileName = hashName;
            listingImage.Description = image.FileName.Substring(0, image.FileName.IndexOf(".")).Truncate(30);
            listingImage.Src = imageURL;
            listingImage.CreateDate = DateTime.Now;

            db.ListingImages.Add(listingImage);
            db.SaveChanges();

            return listingImage;
        }
    }
}
