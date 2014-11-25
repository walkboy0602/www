﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using App.Core.Data;
using App.Core.ViewModel;
using App.Core.Models;
using App.Ads.Models;
using App.Ads.ViewModel;
using App.Ads.Areas.Account.Models;

namespace App.Ads
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                //Listing Mapper
                Mapper.CreateMap<Listing, EditListingViewModel>();
                Mapper.CreateMap<EditListingViewModel, Listing>();
                Mapper.CreateMap<Listing, DisplayListingViewModel>();
                Mapper.CreateMap<ListingImage, ListingImageViewModel>();
                Mapper.CreateMap<Listing, ListingDetailViewModel>();
                        //.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.RegionZone.Name))
                        //.ForMember(dest => dest.LocationParentId, opt => opt.MapFrom(src => src.RegionZone.ParentId));

                Mapper.CreateMap<ListingImage, AdImageViewModel>();
                Mapper.CreateMap<EditImageViewModel, ListingImage>();

                Mapper.CreateMap<Listing, SearchViewModel>();

                Mapper.CreateMap<CreateArticleModel, Article>();
                Mapper.CreateMap<Article, CreateArticleModel>();

                // UserProfile
                Mapper.CreateMap<App.Core.Data.UserProfile, EditUserProfile>();
                Mapper.CreateMap<EditUserProfile, App.Core.Data.UserProfile>();

                Mapper.CreateMap<SendEnquiryModel, Enquiry>();
            });
        }
    }
}