namespace CRMSecurityProvider.Repository.V4
{
    using Sitecore.Diagnostics;
    using CRMSecurityProvider.crm4.webservice;
    using Sources.Repository.V4.Extensions;
    using System;
    /// <summary>
    /// Marketing list to role converter class (API v4).
    /// </summary>
    public class MarketingListToRoleConverterV4 : IMarketingListToRoleConverterV4
    {
        /// <summary>
        /// Converts the marketing list into the role.
        /// </summary>
        /// <param name="marketingList">The marketing list.</param>
        /// <returns>The role.</returns>
        public CRMRole Convert(list marketingList)
        {
            Assert.ArgumentNotNull(marketingList, "marketingList");

            var role = new CRMRole(marketingList.listname, marketingList.listid.Value);
            foreach (var attribute in marketingList.GetAttributes())
            {
                if (attribute.Value is DateTime)
                {
                    role.SetPropertyValue(attribute.Key, ((DateTime)attribute.Value).ToLocalTime());
                }
                else
                {
                    role.SetPropertyValue(attribute.Key, attribute.Value);
                }
            }
            return role;
        }
    }
}
