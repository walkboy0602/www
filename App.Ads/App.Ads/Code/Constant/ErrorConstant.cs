﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads
{
    public static class ErrorConstant
    {
        public static string BAD_REQUEST = "Bad Request.";
        public static string GENERAL_ERROR = "Sorry, there was an error while processing your request. <br/> Please try again later.";
        public static string INVALID_QUERY_STRING = "Sorry, the link that you are trying to access does not correspond to any page on ohMyBazaar.com.";
        public static string NO_RESULT = "No ads were found!";
    }
}