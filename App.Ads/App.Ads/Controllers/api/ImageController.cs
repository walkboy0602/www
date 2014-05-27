using App.Core.Utility;
using App.Core.Data;
using App.Core.Services;
using App.Core.ViewModel;
using App.Ads.Code.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Helpers;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using AutoMapper;

namespace App.Ads.Controllers.api
{
    [Authorize]
    public class ImageController : ApiController
    {
        private AdsDBEntities db = new AdsDBEntities();

        private readonly IListingService _listingService;
        private readonly IImageService _imageService;

        public ImageController(IListingService listingService, IImageService imageService)
        {
            _listingService = listingService;
            _imageService = imageService;
        }

        public HttpResponseMessage Get(int ListingId)
        {
            CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

            var images = _imageService.Get(ListingId, identity.UserId);

            List<ListingImageViewModel> model = Mapper.Map<List<ListingImage>, List<ListingImageViewModel>>(images);

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        public HttpResponseMessage Put(EditImageViewModel model)
        {

            CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

            ListingImage listingImage = _imageService.GetById(model.id, identity.UserId);

            if (listingImage == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid input");
            }

            if (ModelState.IsValid)
            {
                listingImage = Mapper.Map<EditImageViewModel, ListingImage>(model, listingImage);

                if (listingImage.IsCover)
                {
                    _imageService.RemoveCoverImage(listingImage.ListingId);
                }

                _imageService.Save(listingImage);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //Multiple Uploads
        [HttpPost]
        public async Task<HttpResponseMessage> Uploads()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            // Read the form data.
            await Request.Content.ReadAsMultipartAsync(provider);

            int id = 0;

            Int32.TryParse(provider.FormData.GetValues("id")[0], out id);

            CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

            Listing listing = _listingService.GetListingById(id, identity.UserId);

            if (listing == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Id.");
            }

            int totalImage = _imageService.Count(id);

            if (totalImage >= 8)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot upload more than 8 photos.");
            }

            DateTime CreateDate = listing.CreateDate ?? DateTime.Now;

            ListingImage listingImage = _imageService.Uploads(identity.UserId, id, CreateDate);

            return Request.CreateResponse(HttpStatusCode.OK, listingImage);

        }

        [HttpPost, ActionName("Delete")]
        public HttpResponseMessage Delete([FromUri] int ImageId)
        {
            CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

            _imageService.Delete(ImageId, identity.UserId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        // AJAX
        // POST: /Listing/ImageUpload

        //[HttpPost]
        //public HttpResponseMessage Post()
        //{
        //    HttpResponseMessage result = null;
        //    var httpRequest = HttpContext.Current.Request;
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        var docfiles = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
        //            postedFile.SaveAs(filePath);

        //            docfiles.Add(filePath);
        //        }
        //        result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
        //    }
        //    else
        //    {
        //        result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //    return result;
        //}

    }
}
