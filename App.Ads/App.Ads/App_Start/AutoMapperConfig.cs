using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using App.Core.Data;
using App.Core.ViewModel;

namespace App.Ads
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                //Listing
                Mapper.CreateMap<Listing, EditListingViewModel>();
                Mapper.CreateMap<EditListingViewModel, Listing>();
                Mapper.CreateMap<Listing, DisplayListingViewModel>();
                Mapper.CreateMap<ListingImage, ListingImageViewModel>();
            });
        }
    }
}