using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads.Models
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string[] Errors { get; set; }
        public object Data { get; set; }
    }
}