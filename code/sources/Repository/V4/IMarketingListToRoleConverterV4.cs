namespace CRMSecurityProvider.Repository.V4
{
  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// Marketing list to role converter interface (API v4).
  /// </summary>
  public interface IMarketingListToRoleConverterV4
  {
    /// <summary>
    /// Converts the marketing list into the role.
    /// </summary>
    /// <param name="marketingList">The marketing list.</param>
    /// <returns>The role.</returns>
    CRMRole Convert(list marketingList);
  }
}
