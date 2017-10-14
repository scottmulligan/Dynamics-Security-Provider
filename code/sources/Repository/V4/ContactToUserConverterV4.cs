namespace CRMSecurityProvider.Repository.V4
{
  using System;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Utils;
  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// Contact to user converter class (API v4).
  /// </summary>
  public class ContactToUserConverterV4 : IContactToUserConverterV4
  {
    /// <summary>
    /// Converts the contact into the user.
    /// </summary>
    /// <param name="contact">The contact.</param>
    /// <returns>The user.</returns>
    public CRMUser Convert(DynamicEntity contact)
    {
      Assert.ArgumentNotNull(contact, "contact");

      var nameProperty = (StringProperty)contact.Properties.ByName(Configuration.Settings.UniqueKeyProperty);
      var keyProperty = (KeyProperty)contact.Properties.ByName("contactid");

      var emailProperty = contact.Properties.ByName("emailaddress1") as StringProperty;
      emailProperty = emailProperty ?? new StringProperty
      {
        Name = "emailaddress1",
        Value = String.Empty
      };

      var descriptionProperty = contact.Properties.ByName("description") as StringProperty;
      descriptionProperty = descriptionProperty ?? new StringProperty
      {
        Name = "description",
        Value = String.Empty
      };

      var createdProperty = contact.Properties.ByName("createdon") as CrmDateTimeProperty;
      createdProperty = createdProperty ?? new CrmDateTimeProperty
      {
        Name = "createdon",
        Value = new CrmDateTime { Value = DateTime.MinValue.ToString() }
      };

      var lastActivityDateProperty = contact.Properties.ByName("lastusedincampaign") as CrmDateTimeProperty;
      lastActivityDateProperty = lastActivityDateProperty ?? new CrmDateTimeProperty
      {
        Name = "lastusedincampaign",
        Value = new CrmDateTime { Value = DateTime.MinValue.ToString() }
      };

      var user = new CRMUser(
        nameProperty.Value,
        keyProperty.Value.Value,
        emailProperty.Value,
        null,
        descriptionProperty.Value,
        true,
        false,
        DateTime.Parse(createdProperty.Value.Value),
        DateTime.Now,
        DateTime.Parse(lastActivityDateProperty.Value.Value),
        DateTime.MinValue,
        DateTime.MinValue);

      var propertyToValueConverter = new PropertyToValueConverterV4();
      foreach (var property in contact.Properties)
      {
        user.SetPropertyValue(property.Name, propertyToValueConverter.Convert(property));
      }

      return user;
    }
  }
}
