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
        int Count(int ListingId);
        void Save(ListingImage listingImage);
        bool Delete(int ImageId, int UserId);
        void RemoveCoverImage(int ListingId);

        List<ListingImage> Get(int ListingId, int UserId);
        ListingImage GetById(int ImageId, int UserId);
        ListingImage Uploads(int UserId, int ListingId, DateTime CreateDate);
    }

    public class ImageService : IImageService
    {
        private AdsDBEntities db = new AdsDBEntities();
        private readonly IAWSService awsService;
        private readonly IConfigService configService;

        public ImageService(IAWSService awsService, IConfigService configService)
        {
            this.awsService = awsService;
            this.configService = configService;
        }

        void IImageService.Save(ListingImage listingImage)
        {
            db.SaveChanges();
        }

        void IImageService.RemoveCoverImage(int ListingId)
        {
            var image = (from lb in db.ListingImages
                         where lb.IsCover == true
                         where lb.ListingId == ListingId
                         select lb).FirstOrDefault();

            if (image != null)
            {
                image.IsCover = false;
            }
        }

        bool IImageService.Delete(int ImageId, int UserId)
        {
            ListingImage model = (from l in db.Listings
                                  join lb in db.ListingImages on l.Id equals lb.ListingId
                                  where lb.id == ImageId
                                  where l.CreateBy == UserId
                                  select lb).FirstOrDefault();

            if (model != null)
            {

                awsService.DeletePhoto(model.Src.Replace("####size####", "s0"));
                awsService.DeletePhoto(model.Src.Replace("####size####", "s1"));
                awsService.DeletePhoto(model.Src.Replace("####size####", "s2"));

                db.ListingImages.Remove(model);
                db.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }

        int IImageService.Count(int ListingId)
        {
            return this.db.ListingImages
                .Where(l => l.ListingId == ListingId).Count();
        }

        List<ListingImage> IImageService.Get(int ListingId, int UserId)
        {
            var result = (from l in db.Listings
                          join lb in db.ListingImages on l.Id equals lb.ListingId
                          where l.Id == ListingId
                          where l.CreateBy == UserId
                          select lb
                              ).ToList();

            return result;
        }

        ListingImage IImageService.GetById(int ImageId, int UserId)
        {
            var result = (from l in db.Listings
                          join lb in db.ListingImages on l.Id equals lb.ListingId
                          where lb.id == ImageId
                          where l.CreateBy == UserId
                          select lb
                              ).FirstOrDefault();

            return result;
        }

        ListingImage IImageService.Uploads(int UserId, int ListingId, DateTime CreateDate)
        {
            ListingImage listingImage = new ListingImage();

            string YearMonthDay = string.Empty;

            YearMonthDay = CreateDate.Year.ToString() + CreateDate.Month.ToString("d2") + CreateDate.Day.ToString();

            var image = WebImage.GetImageFromRequest();
            string hashName = awsService.Hash(image.FileName, ListingId);

            //file format
            //env + listing + yyyymmdd + listingid + size

            string imageURL = string.Join("/", new string[] { configService.GetValue(ConfigName.S3Env), "listing", YearMonthDay, ListingId.ToString(), "####size####", hashName });

            bool status = false;

            byte[] fileBytes = image.GetBytes();

            //Thumnbnail
            byte[] s0 = awsService.CreateImage(fileBytes, image.FileName, 200, 150);
            status = awsService.UploadToS3(s0, imageURL.Replace("####size####", "s0"));

            //Standard
            byte[] s1 = awsService.CreateImage(fileBytes, image.FileName, 315, 230);
            status = awsService.UploadToS3(s1, imageURL.Replace("####size####", "s1"));

            //Large
            byte[] s2 = awsService.CreateImage(fileBytes, image.FileName, 640, 480);
            status = awsService.UploadToS3(s2, imageURL.Replace("####size####", "s2"));

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
