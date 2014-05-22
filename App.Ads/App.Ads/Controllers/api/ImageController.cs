using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Helpers;
using App.Core.Utility;
using App.Core.Data;

namespace App.Ads.Controllers.api
{
    [Authorize]
    public class ImageController : ApiController
    {
        private AdsDBEntities db = new AdsDBEntities();
        // Get

        public HttpResponseMessage Get(int ListingId)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // AJAX
        // POST: /Listing/ImageUpload

        [HttpPost]
        public HttpResponseMessage Upload(Listing listing)
        {
            ListingImage listingImage = new ListingImage();

            S3Helper s3 = new S3Helper();

            DateTime create_date = (DateTime)listing.CreateDate;


            string YearMonthDay = string.Empty;

            YearMonthDay = create_date.Year.ToString() + create_date.Month.ToString("d2") + create_date.Day.ToString();

            var image = WebImage.GetImageFromRequest();
            string hashName = s3.Hash(image.FileName, listing.id);

            //file format
            //env + listing + yyyymmdd + listingid + size

            string imageURL = string.Join("/", new string[] { s3.env, "listing", YearMonthDay, listing.id.ToString(), "####size####", hashName });

            bool status = false;

            byte[] fileBytes = image.GetBytes();

            //Thumnbnail
            byte[] s0 = s3.CreateImage(fileBytes, image.FileName, 56, 42);
            status = s3.UploadToS3(s0, imageURL.Replace("####size####", "s0"));

            //Standard
            byte[] s1 = s3.CreateImage(fileBytes, image.FileName, 315, 230);
            status = s3.UploadToS3(s1, imageURL.Replace("####size####", "s1"));

            //Large
            byte[] s2 = s3.CreateImage(fileBytes, image.FileName, 640, 480);
            status = s3.UploadToS3(s2, imageURL.Replace("####size####", "s2"));

            listingImage.ListingId = listing.id;
            listingImage.FileName = hashName;
            listingImage.Description = image.FileName.Substring(0, image.FileName.IndexOf(".")).Truncate(30);
            listingImage.Src = imageURL;
            listingImage.CreateDate = DateTime.Now;

            db.ListingImages.Add(listingImage);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, listingImage);
        }
    }
}
