namespace CRMSecurityProvider.Repository.V3
{
    using CRMSecurityProvider.crm3.webservice;
    using Sitecore.Diagnostics;
    using Sources.Repository.V3.Extensions;
    using System;
    /// <summary>
    /// Marketing list to role converter class (API v3).
    /// </summary>
    public class MarketingListToRoleConverterV3 : IMarketingListToRoleConverterV3
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
