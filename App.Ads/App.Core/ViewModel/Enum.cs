using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.ViewModel
{
    public class XtEnum
    {
        public enum ListingStatus
        {
            New = 0,
            Draft = 100,
            Published = 200,
            Pending = 300,
            Rejected = 400,
            Expired = 500
        }

        public enum DealMethod
        {
            COD = 1,
            Postage = 2,
            OnlineBanking = 3
        }
    }
}
