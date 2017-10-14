namespace CRMSecurityProvider.Repository.V3
{
  using CRMSecurityProvider.crm3.webservice;

  /// <summary>
  /// Marketing list to role converter interface (API v3).
  /// </summary>
  public interface IMarketingListToRoleConverterV3
  {
    /// <summary>
    /// Converts the marketing list into the role.
    /// </summary>
    /// <param name="marketingList">The marketing list.</param>
    /// <returns>The role.</returns>
    CRMRole Convert(list marketingList);
  }
}
