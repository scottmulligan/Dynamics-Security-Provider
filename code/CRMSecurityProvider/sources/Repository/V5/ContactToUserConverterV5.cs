namespace CRMSecurityProvider.Repository.V5
{
  using System;

  using Microsoft.Xrm.Sdk;

  using Sitecore.Diagnostics;

  /// <summary>
  /// Contact to user converter class (API v5).
  /// </summary>
  public class ContactToUserConverterV5 : IContactToUserConverterV5
  {
    /// <summary>
    /// Converts the contact into the user.
    /// </summary>
    /// <param name="contact">The contact.</param>
    /// <returns>The user.</returns>
    public CRMUser Convert(Entity contact)
    {
      Assert.ArgumentNotNull(contact, "contact");

      var user = new CRMUser(
        (string)contact[Configuration.Settings.UniqueKeyProperty],
        (Guid)contact["contactid"],
        contact.Contains("emailaddress1") ? (string)contact["emailaddress1"] : String.Empty,
        null,
        contact.Contains("description") ? (string)contact["description"] : String.Empty,
        true,
        false,
        contact.Contains("createdon") ? ((DateTime)contact["createdon"]).ToLocalTime() : DateTime.MinValue,
        DateTime.Now,
        contact.Contains("lastusedincampaign") ? ((DateTime)contact["lastusedincampaign"]).ToLocalTime() : DateTime.MinValue,
        DateTime.MinValue,
        DateTime.MinValue);

      foreach (var attribute in contact.Attributes)
      {
        if (attribute.Value is DateTime)
        {
          user.SetPropertyValue(attribute.Key, ((DateTime)attribute.Value).ToLocalTime());
        }
        else
        {
          user.SetPropertyValue(attribute.Key, attribute.Value);
        }
      }

      return user;
    }
  }
}
