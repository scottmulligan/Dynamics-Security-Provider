// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmPropertyAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmPropertyAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute
{
  using System;
  using CRMSecurityProvider.Sources.Utils.Extensions;
  using CRMSecurityProvider.crm4.webservice;
  using Sitecore;
  using Sitecore.Data;

  // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

  /// <summary>
  /// The crm property attribute adapter.
  /// </summary>
  internal class CrmPropertyAttributeAdapter : CrmAttributeAdapter<Property>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmPropertyAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmPropertyAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, Property internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    public override string GetStringifiedValue()
    {
      if (this.Adaptee is StringProperty)
      {
        return ((StringProperty)this.Adaptee).Value;
      }

      if (this.Adaptee is UniqueIdentifierProperty)
      {
        return ((UniqueIdentifierProperty)this.Adaptee).Value.Value.ToString();
      }

      if (this.Adaptee is StatusProperty)
      {
        return ((StatusProperty)this.Adaptee).Value.name;
      }

      if (this.Adaptee is PicklistProperty)
      {
        return ((PicklistProperty)this.Adaptee).Value.name;
      }

      if (this.Adaptee is OwnerProperty)
      {
        return ((OwnerProperty)this.Adaptee).Value.name;
      }

      if (this.Adaptee is LookupProperty)
      {
        return ((LookupProperty)this.Adaptee).Value.name;
      }

      if (this.Adaptee is KeyProperty)
      {
        return ((KeyProperty)this.Adaptee).Value.Value.ToString();
      }

      if (this.Adaptee is EntityNameReferenceProperty)
      {
        return ((EntityNameReferenceProperty)this.Adaptee).Value.Value;
      }

      if (this.Adaptee is DynamicEntityArrayProperty)
      {
        var dynamycProperty = (DynamicEntityArrayProperty)this.Adaptee;
        if (dynamycProperty.Value.Length > 0 && dynamycProperty.Value[0] != null && dynamycProperty.Value[0].Properties.Length > 0 &&
          dynamycProperty.Value[0].Properties[0] != null)
        {
          return new CrmAttributeAdapterFactory(this.AttributeCollection).Create(dynamycProperty.Value[0].Properties[0]).GetStringifiedValue();
        }

        return string.Empty;
      }

      if (this.Adaptee is CustomerProperty)
      {
        return ((CustomerProperty)this.Adaptee).Value.name;
      }

      if (this.Adaptee is CrmNumberProperty)
      {
        return ((CrmNumberProperty)this.Adaptee).Value.Value.ToString();
      }

      if (this.Adaptee is CrmMoneyProperty)
      {
        return ((CrmMoneyProperty)this.Adaptee).Value.formattedvalue;
      }

      if (this.Adaptee is CrmFloatProperty)
      {
        return ((CrmFloatProperty)this.Adaptee).Value.formattedvalue;
      }

      if (this.Adaptee is CrmDecimalProperty)
      {
        return ((CrmDecimalProperty)this.Adaptee).Value.formattedvalue;
      }

      if (this.Adaptee is CrmDateTimeProperty)
      {
        return ((CrmDateTimeProperty)this.Adaptee).Value.Value;
      }

      if (this.Adaptee is CrmBooleanProperty)
      {
        return ((CrmBooleanProperty)this.Adaptee).Value.name;
      }

      return string.Empty;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public override void SetValue(string value, params string[] data)
    {
      if (this.Adaptee is StringProperty)
      {
        ((StringProperty)this.Adaptee).Value = value;
        return;
      }

      if (this.Adaptee is UniqueIdentifierProperty)
      {
        ID idValue;
        if (ID.TryParse(value, out idValue))
        {
          var identifier = new UniqueIdentifier
          {
            Value = idValue.Guid
          };
          ((UniqueIdentifierProperty)this.Adaptee).Value = identifier;
        }

        return;
      }

      if (this.Adaptee is StatusProperty)
      {
        int intValue;
        if (int.TryParse(value, out intValue))
        {
          var status = new Status
          {
            Value = intValue
          };
          ((StatusProperty)this.Adaptee).Value = status;
        }

        return;
      }

      if (this.Adaptee is PicklistProperty)
      {
        var picklistValue = value;
        Picklist list = null;
        int intValue;

        if (!string.IsNullOrEmpty(picklistValue) && int.TryParse(picklistValue, out intValue))
        {
          list = new Picklist
          {
            Value = intValue
          };
        }
        else
        {
          picklistValue = null;
        }

        if (string.IsNullOrEmpty(picklistValue))
        {
          list = new Picklist
          {
            IsNull = true,
            IsNullSpecified = true
          };
        }

        ((PicklistProperty)this.Adaptee).Value = list;
        return;
      }

      if (this.Adaptee is OwnerProperty)
      {
        ID idValue;
        if (ID.TryParse(value, out idValue))
        {
          var owner = new Owner
          {
            Value = idValue.Guid,
            type = data[0]
          };
          ((OwnerProperty)this.Adaptee).Value = owner;
        }

        return;
      }

      if (this.Adaptee is LookupProperty)
      {
        ID idValue;
        if (ID.TryParse(value, out idValue))
        {
          var lookup = new Lookup
          {
            Value = idValue.Guid,
            type = data[0]
          };
          ((LookupProperty)this.Adaptee).Value = lookup;
        }

        return;
      }

      if (this.Adaptee is KeyProperty)
      {
        ID idValue;
        if (ID.TryParse(value, out idValue))
        {
          var key = new Key
          {
            Value = idValue.Guid
          };
          ((KeyProperty)this.Adaptee).Value = key;
        }

        return;
      }

      if (this.Adaptee is EntityNameReferenceProperty)
      {
        var reference = new EntityNameReference
        {
          Value = value
        };
        ((EntityNameReferenceProperty)this.Adaptee).Value = reference;
        return;
      }

      if (this.Adaptee is DynamicEntityArrayProperty)
      {
        var dynamycProperty = (DynamicEntityArrayProperty)this.Adaptee;

        dynamycProperty.Value = new[]
        {
          new DynamicEntity
          {
            Name = "activityparty",
            Properties = new[]
            {
              new LookupProperty
              {
                Name = "partyid",
                Value = new Lookup
                {
                  Value = new Guid(value),
                  type = data[0]
                }
              }
            }
          }
        };
      }

      if (this.Adaptee is CustomerProperty)
      {
        ID idValue;
        if (ID.TryParse(value, out idValue))
        {
          var customer = new Customer
          {
            Value = idValue.Guid,
            type = data[0]
          };
          ((CustomerProperty)this.Adaptee).Value = customer;
        }

        return;
      }

      if (this.Adaptee is CrmNumberProperty)
      {
        int intValue;
        if (int.TryParse(value, out intValue))
        {
          var number = new CrmNumber
          {
            Value = intValue
          };
          ((CrmNumberProperty)this.Adaptee).Value = number;
        }

        return;
      }

      if (this.Adaptee is CrmMoneyProperty)
      {
        decimal decimalValue;
        if (decimal.TryParse(value, out decimalValue))
        {
          var money = new CrmMoney
          {
            Value = decimalValue
          };
          ((CrmMoneyProperty)this.Adaptee).Value = money;
        }

        return;
      }

      if (this.Adaptee is CrmFloatProperty)
      {
        float decimalValue;
        if (float.TryParse(value, out decimalValue))
        {
          var crmFloat = new CrmFloat
          {
            Value = decimalValue
          };
          ((CrmFloatProperty)this.Adaptee).Value = crmFloat;
        }

        return;
      }

      if (this.Adaptee is CrmDecimalProperty)
      {
        decimal decimalValue;
        if (decimal.TryParse(value, out decimalValue))
        {
          var crmDecimal = new CrmDecimal
          {
            Value = decimalValue
          };
          ((CrmDecimalProperty)this.Adaptee).Value = crmDecimal;
        }

        return;
      }

      if (this.Adaptee is CrmDateTimeProperty)
      {
        DateTime parsedValue;
        if (value.TryParseDateTime(out parsedValue))
        {
          ((CrmDateTimeProperty)this.Adaptee).Value = new CrmDateTime { Value = parsedValue.ToCrmDateTimeString() };
        }
      }

      if (this.Adaptee is CrmBooleanProperty)
      {
        var boolValue = MainUtil.GetBool(value, false);
        var crmBool = new CrmBoolean
        {
          Value = boolValue
        };
        ((CrmBooleanProperty)this.Adaptee).Value = crmBool;
      }
    }
  }

  // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull
}