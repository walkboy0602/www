﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using App.Core.Services;
using App.Core.ViewModel;
using App.Core.Data;
using System.Text.RegularExpressions;

namespace App.Ads.Code.Helpers
{
    public class ListingHelper
    {
        public static string FormatImage(string Src, string Size = null)
        {
            if (!string.IsNullOrEmpty(Src) && !string.IsNullOrEmpty(Size))
            {
                Src = Src.Replace("####size####", Size);
            }

            return Src;
        }

        public static string GetCoverImage(List<App.Core.Data.ListingImage> listingImages, string size = null)
        {
            string coverImage = string.Empty;
            if (listingImages.Count > 0)
            {
                coverImage = listingImages
                                    .Where(i => i.IsCover)
                                    .Select(i => i.Src).FirstOrDefault();

                if (string.IsNullOrEmpty(coverImage)) coverImage = listingImages.Select(i => i.Src).FirstOrDefault();
            }

            return FormatImage(coverImage, size);
        }

        public static string FormatTitle(string title)
        {
            return string.IsNullOrEmpty(title) ? "Untitled" : title;
        }

        public static string FormatTitleAndPrice(string Title, decimal? Price)
        {
            string sb = string.Empty;

            sb = Title + "<b>" + FormatPrice(Price) + "</b>";

            return sb;
        }

        public static string FormatPrice(decimal? Price)
        {
            string newPrice = string.Empty;

            if (Price != null)
            {
                decimal p = Decimal.Truncate((decimal)Price);

                if (Price.Equals(p))
                {
                    newPrice = string.Format("{0:C0}", Price);
                }
                else
                {
                    newPrice = string.Format("{0:C2}", Price);
                }

            }

            return newPrice;
        }

        public static string FormatDate(DateTime? date, String format)
        {
            return date == null ? string.Empty : Convert.ToDateTime(date).ToString(format);
        }

        public static string FormatLocation(string area, string location)
        {
            string newLocation = location;

            if (!string.IsNullOrEmpty(area))
            {
                newLocation = area + ", " + location;
            }

            return newLocation;
        }

        public static string FormatCreateDate(DateTime dateVal)
        {
            int dayDiff = (DateTime.Now - dateVal).Days;

            return dayDiff == 0 ? "Today" : dayDiff + " day ago";
        }

        public static string FormatDescription(string description, int maxLength)
        {
            if (!string.IsNullOrEmpty(description))
            {
                description = Regex.Replace(description, @"<[^>]*>", string.Empty);
                description = description.Replace("&nbsp;", string.Empty);

                if (description.Length >= maxLength)
                {
                    return description.Substring(0, maxLength) + "...";
                }
            }

            return description;
        }

        public static string FormatListingStatusCss(int status)
        {
            switch ((XtEnum.ListingStatus)status)
            {
                case XtEnum.ListingStatus.Draft:
                case XtEnum.ListingStatus.New:
                    return "label label-default";

                case XtEnum.ListingStatus.Expired:
                    return "label label-info";

                case XtEnum.ListingStatus.Processing:
                    return "label label-warning";

                case XtEnum.ListingStatus.Online:
                    return "label label-success";

                case XtEnum.ListingStatus.Rejected:
                    return "label label-danger";

                default:
                    return "label label-default";
            }
        }

        public static string FormatListingStatusToolTip(int status, DateTime endDate)
        {
            switch ((XtEnum.ListingStatus)status)
            {
                case XtEnum.ListingStatus.Processing:
                    return "Pending for review";

                //case XtEnum.ListingStatus.Online:
                //    return "Ad expired on " + endDate.ToString("ddMMMyy");

                case XtEnum.ListingStatus.Rejected:
                    return "Ad rejected for reasons";

                case XtEnum.ListingStatus.Expired:
                    return "Ad has been expired";

                default:
                    return "";
            }
        }

        public static string GetSearchTitle(string keyword, string category, string area, string city, string listType)
        {
            category = string.IsNullOrEmpty(category) ? "Almost anything for sale" : category;

            string location = string.IsNullOrEmpty(area) ? city : area + ", " + city;

            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword + " - ";

            return keyword + category + " " + listType + " in " + location;
        }
    }
}