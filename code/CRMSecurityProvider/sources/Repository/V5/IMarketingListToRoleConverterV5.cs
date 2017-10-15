namespace CRMSecurityProvider.Repository.V5
{
  using Microsoft.Xrm.Sdk;

  /// <summary>
  /// Marketing list to role converter interface (API v5).
  /// </summary>
  public interface IMarketingListToRoleConverterV5
  {
    /// <summary>
    /// Converts the marketing list into the role.
    /// </summary>
    /// <param name="marketingList">The marketing list.</param>
    /// <returns>The role.</returns>
    CRMRole Convert(Entity marketingList);
  }
}
