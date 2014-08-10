using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using App.Core.Services;
using App.Core.ViewModel;
using System.Text.RegularExpressions;

namespace App.Ads.Code.Helpers
{
    public class ListingHelper
    {
        private static string S3ImagePath = ConfigurationManager.AppSettings["S3ImagePath"];

        public static string FormatImage(string Src, string Size = null)
        {
            if (!string.IsNullOrEmpty(Src) && !string.IsNullOrEmpty(Size))
            {
                Src = Src.Replace("####size####", Size);
            }

            return S3ImagePath + Src;
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
                newPrice = "RM" + Price;
            }

            return newPrice;
        }

        public static string FormatLocation(string locationName, string locationParentName)
        {
            string newLocation = locationName;

            if (!string.IsNullOrEmpty(locationParentName))
            {
                newLocation = locationName + ", " + locationParentName;
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

                case XtEnum.ListingStatus.Pending:
                    return "label label-warning";

                case XtEnum.ListingStatus.Published:
                    return "label label-success";

                case XtEnum.ListingStatus.Rejected:
                    return "label label-danger";

                default:
                    return "label label-default";
            }
        }

        public static string FormatListingStatusToolTip(int status)
        {
            switch ((XtEnum.ListingStatus)status)
            {
                case XtEnum.ListingStatus.Pending:
                    return "Pending for review";

                case XtEnum.ListingStatus.Published:
                    return "Ad successfully posted";

                case XtEnum.ListingStatus.Rejected:
                    return "Ad rejected for reasons";

                default:
                    return "";
            }
        }
    }
}