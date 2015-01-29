using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Core.Data;
using App.Core.Services;
using AutoMapper;
using App.Ads.ViewModel;

namespace App.Ads.Controllers.api
{
    [RoutePrefix("api")]
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IReferenceService _referenceService;

        public CategoryController(ICategoryService categoryService, IReferenceService referenceService)
        {
            _categoryService = categoryService;
            _referenceService = referenceService;
        }

        // GET api/category
        public HttpResponseMessage Get()
        {
            List<RefCategory> categories = _categoryService.Get(false).ToList();

            List<CategoryViewModel> model = Mapper.Map<List<RefCategory>, List<CategoryViewModel>>(categories);

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [Route("reference/{type:alpha}")]
        public HttpResponseMessage GetRefValue(string type)
        {
            List<RefTable> refValue = _referenceService.GetByType(type).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, refValue);
        }
    }
}