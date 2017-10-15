namespace CRMSecurityProvider.Repository.V5
{
  using Microsoft.Xrm.Sdk;

  /// <summary>
  /// Contact to user converter interface (API v5).
  /// </summary>
  public interface IContactToUserConverterV5
  {
    /// <summary>
    /// Converts the contact into the user.
    /// </summary>
    /// <param name="contact">The contact.</param>
    /// <returns>The user.</returns>
    CRMUser Convert(Entity contact);
  }
}
