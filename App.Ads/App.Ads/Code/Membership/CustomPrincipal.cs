using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;

namespace App.Ads.Code.Membership
{
    public class CustomPrincipal: IPrincipal
    {
        #region Implementation of IPrincipal

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name="role">The name of the role for which to check membership. </param>
        public bool IsInRole(string role)
        {
            string[] userRoles = ((CustomIdentity)Identity).UserRoles.Select(u => u.Role).Select(r => r.RoleName).ToArray();

            if (userRoles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        public CustomIdentity CustomIdentity { get { return (CustomIdentity)Identity; } set { Identity = value; } }

        public CustomPrincipal(CustomIdentity identity)
        {
            Identity = identity;
        }
    }
}