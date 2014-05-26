﻿using System;
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
                //Listing Mapper
                Mapper.CreateMap<Listing, EditListingViewModel>();
                Mapper.CreateMap<EditListingViewModel, Listing>();
                Mapper.CreateMap<Listing, DisplayListingViewModel>();
                Mapper.CreateMap<ListingImage, ListingImageViewModel>();
                Mapper.CreateMap<Listing, AdDetailViewModel>()
                        .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.RegionZone.Name))
                        .ForMember(dest => dest.LocationParentId, opt => opt.MapFrom(src => src.RegionZone.ParentId));

                Mapper.CreateMap<ListingImage, ListingImageiViewModel>();

            });
        }
    }
}