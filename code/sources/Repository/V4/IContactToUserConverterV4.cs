namespace CRMSecurityProvider.Repository.V4
{
  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// Contact to user converter interface (API v4).
  /// </summary>
  public interface IContactToUserConverterV4
  {
    /// <summary>
    /// Converts the contact into the user.
    /// </summary>
    /// <param name="contact">The contact.</param>
    /// <returns>The user.</returns>
    CRMUser Convert(DynamicEntity contact);
  }
}
