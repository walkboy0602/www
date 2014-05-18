using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Core.Data;

namespace App.Core.ViewModel
{
    public class UserMembership
    {
        public Membership Membership { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
