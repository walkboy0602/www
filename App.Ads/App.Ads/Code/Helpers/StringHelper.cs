using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace App.Ads.Code.Helpers
{
    public static class StringHelper
    {
        public static string ToSeoUrl(this string url)
        {
            // make the url lowercase
            string encodedUrl = (url ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }   

        public static string DecodeSeoUrl(this string url)
        {
            string decodedUrl = (url ?? "").ToLower();

            // replace & with and
            decodedUrl = Regex.Replace(decodedUrl, @"\band\b", "&");

            decodedUrl = decodedUrl.Replace("-", " ");

            return decodedUrl;
        }

        public static string ToRelativeDate(this DateTime input)
        {
            if (input == null)
            {
                return "A while ago";
            }

            TimeSpan oSpan = DateTime.Now.Subtract(input.AddHours(8));
            double TotalMinutes = oSpan.TotalMinutes;
            string Suffix = " ago";

            if (TotalMinutes < 6.0)
            {
                //TotalMinutes = Math.Abs(TotalMinutes);
                Suffix = "";
            }

            var aValue = new SortedList<double, Func<string>>();
            //aValue.Add(0.75, () => "less than a minute");
            //aValue.Add(1.5, () => "about a minute");
            aValue.Add(6, () => "Just now");
            aValue.Add(45, () => string.Format("{0} minutes", Math.Round(TotalMinutes)));
            aValue.Add(90, () => "about an hour");
            aValue.Add(1440, () => string.Format("{0} hours", Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
            aValue.Add(2880, () => "a day"); // 60 * 48
            aValue.Add(43200, () => string.Format("{0} days", Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
            aValue.Add(86400, () => "about a month"); // 60 * 24 * 60
            aValue.Add(525600, () => string.Format("{0} months", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365 
            aValue.Add(1051200, () => "about a year"); // 60 * 24 * 365 * 2
            aValue.Add(double.MaxValue, () => string.Format("{0} years", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

            return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
        }
    }
}