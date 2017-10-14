namespace CRMSecurityProvider.Repository.V5
{
  using System;

  using Microsoft.Xrm.Sdk;

  using Sitecore.Diagnostics;

  /// <summary>
  /// Marketing list to role converter class (API v5).
  /// </summary>
  public class MarketingListToRoleConverterV5 : IMarketingListToRoleConverterV5
  {
    /// <summary>
    /// Converts the marketing list into the role.
    /// </summary>
    /// <param name="marketingList">The marketing list.</param>
    /// <returns>The role.</returns>
    public CRMRole Convert(Entity marketingList)
    {
      Assert.ArgumentNotNull(marketingList, "marketingList");

      var role = new CRMRole((string)marketingList[CRMRole.AttributeListName], (Guid)marketingList[CRMRole.AttributeListId]);
      foreach (var attribute in marketingList.Attributes)
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
