namespace CRMSecurityProvider.Repository.V4
{
  using System;

  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// Property to value converter class (API v4).
  /// </summary>
  public class PropertyToValueConverterV4
  {
    /// <summary>
    /// Converts property into its value.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The value of the property</returns>
    public object Convert(Property property)
    {
      var stringProperty = property as StringProperty;
      if (stringProperty != null)
      {
        return stringProperty.Value;
      }

      var boolProperty = property as CrmBooleanProperty;
      if ((boolProperty != null) && !boolProperty.Value.IsNullSpecified)
      {
        return boolProperty.Value.Value ? "1" : "0";
      }

      var dateTimeProperty = property as CrmDateTimeProperty;
      if ((dateTimeProperty != null) && !String.IsNullOrEmpty(dateTimeProperty.Value.Value))
      {
        return DateTime.Parse(dateTimeProperty.Value.Value);
      }

      var floatProperty = property as CrmFloatProperty;
      if ((floatProperty != null) && !floatProperty.Value.IsNullSpecified)
      {
        return floatProperty.Value.Value;
      }

      var moneyProperty = property as CrmMoneyProperty;
      if ((moneyProperty != null) && !moneyProperty.Value.IsNullSpecified)
      {
        return moneyProperty.Value.Value;
      }

      var numberProperty = property as CrmNumberProperty;
      if ((numberProperty != null) && !numberProperty.Value.IsNullSpecified)
      {
        return numberProperty.Value.Value;
      }

      var picklistProperty = property as PicklistProperty;
      if ((picklistProperty != null) && !picklistProperty.Value.IsNullSpecified)
      {
        return picklistProperty.Value.name;
      }

      return null;
    }
  }
}
