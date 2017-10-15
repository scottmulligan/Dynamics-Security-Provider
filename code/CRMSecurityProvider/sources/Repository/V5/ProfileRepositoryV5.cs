namespace CRMSecurityProvider.Repository.V5
{
  using System;
  using System.Collections.Generic;
  using System.Configuration.Provider;
  using System.Linq;
  using CRMSecurityProvider.Sources.Repository.V5;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Messages;
  using Microsoft.Xrm.Sdk.Metadata;
  using Microsoft.Xrm.Sdk.Query;

  using Sitecore;
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Profile repository class (API v5).
  /// </summary>
  public class ProfileRepositoryV5 : ProfileRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileRepositoryV5"/> class.
    /// </summary>
    /// <param name="organizationServiceCache"></param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    public ProfileRepositoryV5(OrganizationServiceCacheV5 organizationServiceCache, UserRepositoryBase userRepository, ICacheService cacheService)
      : base(userRepository, cacheService)
    {
      Assert.ArgumentNotNull(organizationServiceCache, "organizationServiceCache");
      this.organizationServiceCache = organizationServiceCache;
    }

    private readonly OrganizationServiceCacheV5 organizationServiceCache;

    public override bool CreateContactAttribute(string attributeName, SupportedTypes attributeType, bool throwIfExists)
    {
      Assert.ArgumentNotNull(attributeName, "attributeName");

      var retrieveAttributeRequest = new RetrieveAttributeRequest
      {
        EntityLogicalName = "contact",
        LogicalName = attributeName,
        RetrieveAsIfPublished = true
      };

      try
      {
        var retrieveAttributeResponse = (RetrieveAttributeResponse)this.organizationServiceCache.GetOrganizationService().Execute(retrieveAttributeRequest);
        if (retrieveAttributeResponse != null && retrieveAttributeResponse.AttributeMetadata != null)
        {
          if (throwIfExists)
          {
            throw new ProviderException("The attribute can't be created because it already exists.");
          }

          return true;
        }
      }
      catch (Exception e)
      {
        if (!e.Message.Contains("Could not find an attribute with specified name:"))
        {
          ConditionalLog.Error("Couldn't get attribute metadata from CRM", e, this);
          return false;
        }
      }

      var request = new RetrieveMultipleRequest()
      {
        Query = new QueryExpression()
        {
          ColumnSet = new ColumnSet("languagecode"),
          EntityName = "organization",
          NoLock = true,
          Distinct = false
        }
      };

      var languagecode = 0;
      try
      {
        var response = (RetrieveMultipleResponse)this.organizationServiceCache.GetOrganizationService().Execute(request);
        if ((response != null) && (response.EntityCollection != null))
        {
          var entity = response.EntityCollection.Entities.First();

          languagecode = (int)entity["languagecode"];
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get organization from CRM.", e, this);
        return false;
      }

      var attributeRequest = new CreateAttributeRequest();
      attributeRequest.EntityName = "contact";

      AttributeTypeCode crmAttributeType;
      switch (attributeType)
      {
        case SupportedTypes.CrmBoolean:
          crmAttributeType = AttributeTypeCode.Boolean;
          var booleanAttribute = new BooleanAttributeMetadata();

          var trueLabel = new Label("True", languagecode);
          var trueOptionMetadata = new OptionMetadata(trueLabel, 1);

          var falseLabel = new Label("False", languagecode);
          var falseOptionMetadata = new OptionMetadata(falseLabel, 0);

          booleanAttribute.OptionSet = new BooleanOptionSetMetadata(trueOptionMetadata, falseOptionMetadata);
          attributeRequest.Attribute = booleanAttribute;
          break;
        case SupportedTypes.CrmDateTime:
          crmAttributeType = AttributeTypeCode.DateTime;
          attributeRequest.Attribute = new DateTimeAttributeMetadata();
          break;
        case SupportedTypes.CrmNumber:
          crmAttributeType = AttributeTypeCode.Integer;
          attributeRequest.Attribute = new IntegerAttributeMetadata();
          break;
        case SupportedTypes.CrmFloat:
          crmAttributeType = AttributeTypeCode.Double;
          attributeRequest.Attribute = new DoubleAttributeMetadata();
          break;
        case SupportedTypes.CrmDecimal:
          crmAttributeType = AttributeTypeCode.Decimal;
          attributeRequest.Attribute = new DecimalAttributeMetadata();
          break;
        case SupportedTypes.CrmMoney:
          crmAttributeType = AttributeTypeCode.Money;
          attributeRequest.Attribute = new MoneyAttributeMetadata();
          break;
        case SupportedTypes.Picklist:
          crmAttributeType = AttributeTypeCode.Picklist;

          var picklistAttribute = new PicklistAttributeMetadata();
          picklistAttribute.OptionSet = new OptionSetMetadata();
          attributeRequest.Attribute = picklistAttribute;
          break;
        default:
          crmAttributeType = AttributeTypeCode.String;

          var stringAttribute = new StringAttributeMetadata();
          stringAttribute.MaxLength = 256;
          attributeRequest.Attribute = stringAttribute;
          break;
      }

      attributeRequest.Attribute.MetadataId = Guid.NewGuid();
      attributeRequest.Attribute.SchemaName = attributeName;
      attributeRequest.Attribute.DisplayName = new Label(attributeName, languagecode);
      attributeRequest.Attribute.RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);

      try
      {
        this.organizationServiceCache.GetOrganizationService().Execute(attributeRequest);
        return true;
      }
      catch (Exception e)
      {
        ConditionalLog.Error(String.Format("Couldn't create attribute '{0}' of {1} type in CRM.", attributeName, crmAttributeType), e, this);
        return false;
      }
    }

     public override object GetPropertyValue(CRMUser user, string propertyName)
    {
      var value = base.GetPropertyValue(user, propertyName);
      var propertyType = this.GetPropertyType(propertyName);
      if (propertyType == SupportedTypes.Picklist)
      {
        OptionSetValue optionSet = value as OptionSetValue;
        var picklistAttribute = this.CacheService.MetadataCache[propertyName] as PicklistContactAttribute;
        if (optionSet != null && picklistAttribute != null)
        {
          value = picklistAttribute.Options.FirstOrDefault(p => p.Value == optionSet.Value).Key;
        }
      }
      if (propertyType == SupportedTypes.CrmMoney)
      {
        Money money = value as Money;
        if (money != null)
        {
          value = money.Value.ToString("f2");
        }
      }
      if (propertyType == SupportedTypes.CrmBoolean)
      {
        bool boolean = (bool)value;
        value = boolean ? "1" : "0";
      }
      return value;
    }

	public override bool UpdateUserProperties(string userName, Dictionary<string, object> properties)
    {
      Assert.ArgumentNotNull(userName, "userName");
      Assert.ArgumentNotNull(properties, "properties");

      const string UpdateUserProrpertiesKey = "updateUserProperties";
      ConditionalLog.Info(String.Format("UpdateUserProperties({0}). Started.", userName), this, TimerAction.Start, UpdateUserProrpertiesKey);

      var result = false;

      var user = this.UserRepository.GetUser(userName);
      if (user != null)
      {
        if (properties.ContainsKey("fullname"))
        {
          this.ProcessFullNameProperty((string)properties["fullname"], properties);
        }

        var propertiesToUpdate = new Dictionary<string, object>();

        propertiesToUpdate.Add("contactid", user.ID);
        foreach (var property in properties)
        {
          this.AddPropertyToCollection(property.Key, property.Value, this.GetPropertyType(property.Key), propertiesToUpdate);
        }

        var update = new UpdateRequest();
        update.Target = new Entity();
        update.Target.LogicalName = "contact";
        foreach (var property in propertiesToUpdate)
        {
          update.Target.Attributes.Add(property.Key, property.Value);
        }

        try
        {
          this.organizationServiceCache.GetOrganizationService().Execute(update);
          ConditionalLog.Info(String.Format("UpdateUserProperties({0}). Updated in CRM.", userName), this, TimerAction.Tick, UpdateUserProrpertiesKey);

          foreach (var property in propertiesToUpdate)
          {
            user.SetPropertyValue(property.Key, property.Value);
          }

          result = true;
        }
        catch (Exception e)
        {
          ConditionalLog.Error(String.Format("Couldn't save profile changes for the contact {0} in CRM.", userName), e, this);
        }
      }

      ConditionalLog.Info(String.Format("UpdateUserProperties({0}). Finished.", userName), this, TimerAction.Stop, UpdateUserProrpertiesKey);
      return result;
    }

    /// <summary>
    /// Adds the property to collection.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValue">Value of the property.</param>
    /// <param name="propertyType">Type of the property.</param>
    /// <param name="properties">The properties.</param>
    protected void AddPropertyToCollection(string propertyName, object propertyValue, SupportedTypes propertyType, Dictionary<string, object> properties)
    {
      var propertyStringValue = propertyValue as string;
      switch (propertyType)
      {
        case SupportedTypes.String:
          properties.Add(propertyName, propertyValue);

          break;
        case SupportedTypes.CrmBoolean:
          if (propertyStringValue != null || (propertyValue is bool))
          {
            if (propertyValue is bool)
            {
              properties.Add(propertyName, propertyValue);
            }
            else
            {
              properties.Add(propertyName, MainUtil.GetBool(propertyStringValue, false));
            }
          }

          break;
        case SupportedTypes.CrmDateTime:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is DateTime))
          {
            if (propertyValue is DateTime)
            {
              properties.Add(propertyName, propertyValue);
            }
            else
            {
              DateTime dateTimeValue;
              if (DateTime.TryParse(propertyStringValue, out dateTimeValue))
              {
                properties.Add(propertyName, dateTimeValue);
              }
            }
          }

          break;
        case SupportedTypes.CrmFloat:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is double))
          {
            if (propertyValue is double)
            {
              properties.Add(propertyName, propertyValue);
            }
            else
            {
              double doubleValue;
              if (Double.TryParse(propertyStringValue, out doubleValue))
              {
                properties.Add(propertyName, doubleValue);
              }
            }
          }

          break;
        case SupportedTypes.CrmDecimal:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is decimal))
          {
            if (propertyValue is decimal)
            {
              properties.Add(propertyName, propertyValue);
            }
            else
            {
              decimal decimalValue;
              if (Decimal.TryParse(propertyStringValue, out decimalValue))
              {
                properties.Add(propertyName, decimalValue);
              }
            }
          }

          break;
        case SupportedTypes.CrmMoney:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is decimal))
          {
            if (propertyValue is decimal)
            {
              properties.Add(propertyName, new Money((decimal)propertyValue));
            }
            else
            {
              decimal moneyValue;
              if (Decimal.TryParse(propertyStringValue, out moneyValue))
              {
                properties.Add(propertyName, new Money(moneyValue));
              }
            }
          }

          break;
        case SupportedTypes.CrmNumber:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is int))
          {
            if (propertyValue is int)
            {
              properties.Add(propertyName, propertyValue);
            }
            else
            {
              int intValue;
              if (Int32.TryParse(propertyStringValue, out intValue))
              {
                properties.Add(propertyName, intValue);
              }
            }
          }

          break;
        case SupportedTypes.Picklist:
          PicklistContactAttribute picklistAttribute;
          if (this.CacheService.MetadataCache.ContainsKey(propertyName))
          {
            picklistAttribute = (PicklistContactAttribute)this.CacheService.MetadataCache[propertyName];
          }
          else
          {
            var attributeRequest = new RetrieveAttributeRequest();

            attributeRequest.EntityLogicalName = "contact";
            attributeRequest.LogicalName = propertyName;

            var attributeResponse = (RetrieveAttributeResponse)this.organizationServiceCache.GetOrganizationService().Execute(attributeRequest);

            var attributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
            var options = attributeMetadata.OptionSet.Options.ToDictionary(option => option.Label.UserLocalizedLabel.Label, option => option.Value.Value);

            picklistAttribute = new PicklistContactAttribute(SupportedTypes.Picklist, options);
            this.CacheService.MetadataCache.Add(propertyName, picklistAttribute);
          }

          if (!String.IsNullOrEmpty(propertyStringValue))
          {
            var value = picklistAttribute.Options[propertyStringValue];
            if (value >= 0)
            {
              properties.Add(propertyName, new OptionSetValue(value));
            }
            else
            {
              CrmHelper.ShowMessage(String.Format("The picklist value ({0}) of the '{1}' property couldn't be recognized", propertyValue, propertyName));
              break;
            }
          }

          break;
      }
    }

    public override SupportedTypes GetPropertyType(string propertyName)
    {
      Assert.ArgumentNotNullOrEmpty(propertyName, "propertyName");

      const string GetPropertyTypeKey = "getPropertyType";
      ConditionalLog.Info(String.Format("GetPropertyType({0}). Started.", propertyName), this, TimerAction.Start, GetPropertyTypeKey);

      SupportedTypes propertyType;
      ContactAttribute contactAttribute;

      if (this.CacheService.MetadataCache.ContainsKey(propertyName))
      {
        contactAttribute = (ContactAttribute)this.CacheService.MetadataCache[propertyName];
        propertyType = contactAttribute.Type;

        ConditionalLog.Info(String.Format("GetPropertyType({0}). Retrieved from cache.", propertyName), this, TimerAction.Tick, GetPropertyTypeKey);
      }
      else
      {
        var request = new RetrieveAttributeRequest
        {
          EntityLogicalName = "contact",
          LogicalName = propertyName
        };

        AttributeMetadata attributeMetadata;
        try
        {
          var response = (RetrieveAttributeResponse)this.organizationServiceCache.GetOrganizationService().Execute(request);
          attributeMetadata = response.AttributeMetadata;

          ConditionalLog.Info(String.Format("GetPropertyType({0}). Retrieved from CRM.", propertyName), this, TimerAction.Tick, GetPropertyTypeKey);
        }
        catch (Exception e)
        {
          ConditionalLog.Error(String.Format("Couldn't retrieve metadata for the {0} attribute from CRM.", propertyName), e, this);
          throw new ProviderException(String.Format("Couldn't retrieve metadata for the {0} attribute from CRM.", propertyName), e);
        }

        switch (attributeMetadata.AttributeType.Value)
        {
          case AttributeTypeCode.String:
          case AttributeTypeCode.Memo:
            propertyType = SupportedTypes.String;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Boolean:
            propertyType = SupportedTypes.CrmBoolean;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.DateTime:
            propertyType = SupportedTypes.CrmDateTime;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Double:
            propertyType = SupportedTypes.CrmFloat;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Decimal:
            propertyType = SupportedTypes.CrmDecimal;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Money:
            propertyType = SupportedTypes.CrmMoney;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Integer:
            propertyType = SupportedTypes.CrmNumber;
            contactAttribute = new ContactAttribute(propertyType);

            break;
          case AttributeTypeCode.Picklist:
            propertyType = SupportedTypes.Picklist;

            var picklistAttributeMetadata = (PicklistAttributeMetadata)attributeMetadata;
            var options = picklistAttributeMetadata.OptionSet.Options.ToDictionary(option => option.Label.UserLocalizedLabel.Label, option => option.Value.Value);

            contactAttribute = new PicklistContactAttribute(propertyType, options);
            break;
          default:
            ConditionalLog.Info(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.AttributeType.Value), this, TimerAction.Tick, GetPropertyTypeKey);
            throw new ProviderException(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.AttributeType.Value));
        }

        this.CacheService.MetadataCache.Add(propertyName, contactAttribute);
      }

      ConditionalLog.Info(String.Format("GetPropertyType({0}). Finished.", propertyName), this, TimerAction.Stop, GetPropertyTypeKey);
      return propertyType;
    }
  }
}
