namespace CRMSecurityProvider.Repository.V3
{
  using CRMSecurityProvider.crm3.webservice;

  /// <summary>
  /// Contact to user converter interface (API v3).
  /// </summary>
  public interface IContactToUserConverterV3
  {
    /// <summary>
    /// Converts the contact into the user.
    /// </summary>
    /// <param name="contact">The contact.</param>
    /// <returns>The user.</returns>
    CRMUser Convert(DynamicEntity contact);
  }
}
