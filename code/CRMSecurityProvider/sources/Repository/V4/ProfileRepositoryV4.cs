namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Collections.Generic;
  using System.Configuration.Provider;
  using System.Linq;

  using Sitecore;
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.crm4.metadataservice;
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Profile repository class (API v4).
  /// </summary>
  public class ProfileRepositoryV4 : ProfileRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileRepositoryV4"/> class.
    /// </summary>
    /// <param name="metadataService">The CRM metadata service.</param>
    /// <param name="crmService">The CRM service.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    public ProfileRepositoryV4(IMetadataServiceV4 metadataService, ICrmServiceV4 crmService, UserRepositoryBase userRepository, ICacheService cacheService)
      : base(userRepository, cacheService)
    {
      Assert.ArgumentNotNull(metadataService, "metadataService");
      Assert.ArgumentNotNull(crmService, "crmService");

      this.CrmMetadataService = metadataService;
      this.CrmService = crmService;
    }

    /// <summary>
    /// Gets or sets the CRM metadata service.
    /// </summary>
    protected IMetadataServiceV4 CrmMetadataService { get; private set; }

    /// <summary>
    /// Gets or sets the CRM service.
    /// </summary>
    protected ICrmServiceV4 CrmService { get; private set; }

    public override bool CreateContactAttribute(string attributeName, SupportedTypes attributeType, bool throwIfExists)
    {
      Assert.ArgumentNotNull(attributeName, "attributeName");

      var retrieveAttributeRequest = new RetrieveAttributeRequest
      {
        EntityLogicalName = EntityName.contact.ToString(),
        LogicalName = attributeName,
        RetrieveAsIfPublished = true
      };

      try
      {
        var retrieveAttributeResponse = (RetrieveAttributeResponse)this.CrmMetadataService.Execute(retrieveAttributeRequest);
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
          ColumnSet = new ColumnSet()
          {
            Attributes = new[] { "languagecode" }
          },
          EntityName = EntityName.organization.ToString()
        },
        ReturnDynamicEntities = true,
      };

      var languagecode = 0;
      try
      {
        var response = (RetrieveMultipleResponse)this.CrmService.Execute(request);
        if ((response != null) && (response.BusinessEntityCollection != null))
        {
          var entity = response.BusinessEntityCollection.BusinessEntities.Cast<DynamicEntity>().First();
          var property = (CrmNumberProperty)entity.Properties.ByName("languagecode");

          languagecode = property.Value.Value;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get organization from CRM.", e, this);
        return false;
      }

      var attributeRequest = new CreateAttributeRequest
      {
        EntityName = EntityName.contact.ToString()
      };

      AttributeType crmAttributeType;
      switch (attributeType)
      {
        case SupportedTypes.CrmBoolean:
          crmAttributeType = AttributeType.Boolean;
          var booleanAttribute = new BooleanAttributeMetadata();

          var falseLabel = new crm4.metadataservice.LocLabel
          {
            Label = "False",
            LanguageCode = new crm4.metadataservice.CrmNumber
            {
              Value = languagecode
            }
          };
          var falseCrmLabel = new crm4.metadataservice.CrmLabel
          {
            LocLabels = new[] { falseLabel }
          };
          booleanAttribute.FalseOption = new Option
          {
            Label = falseCrmLabel,
            Value = new crm4.metadataservice.CrmNumber
            {
              Value = 0
            }
          };

          var trueLabel = new crm4.metadataservice.LocLabel
          {
            Label = "True",
            LanguageCode = new crm4.metadataservice.CrmNumber
            {
              Value = languagecode
            }
          };
          var trueCrmLabel = new crm4.metadataservice.CrmLabel
          {
            LocLabels = new[] { trueLabel }
          };
          booleanAttribute.TrueOption = new Option
          {
            Label = trueCrmLabel,
            Value = new crm4.metadataservice.CrmNumber
            {
              Value = 1
            }
          };

          attributeRequest.Attribute = booleanAttribute;
          break;
        case SupportedTypes.CrmDateTime:
          crmAttributeType = AttributeType.DateTime;
          attributeRequest.Attribute = new DateTimeAttributeMetadata();
          break;
        case SupportedTypes.CrmNumber:
          crmAttributeType = AttributeType.Integer;
          attributeRequest.Attribute = new IntegerAttributeMetadata();
          break;
        case SupportedTypes.CrmFloat:
          crmAttributeType = AttributeType.Float;
          attributeRequest.Attribute = new FloatAttributeMetadata();
          break;
        case SupportedTypes.CrmDecimal:
          crmAttributeType = AttributeType.Decimal;
          attributeRequest.Attribute = new DecimalAttributeMetadata();
          break;
        case SupportedTypes.CrmMoney:
          crmAttributeType = AttributeType.Money;
          attributeRequest.Attribute = new MoneyAttributeMetadata();
          break;
        case SupportedTypes.Picklist:
          crmAttributeType = AttributeType.Picklist;
          attributeRequest.Attribute = new PicklistAttributeMetadata
          {
            Options = new Option[]
            {
            }
          };
          break;
        default:
          crmAttributeType = AttributeType.String;
          attributeRequest.Attribute = new StringAttributeMetadata
          {
            MaxLength = new crm4.metadataservice.CrmNumber
            {
              IsNull = false,
              IsNullSpecified = false,
              Value = 256
            }
          };
          break;
      }

      attributeRequest.Attribute.MetadataId = new crm4.metadataservice.Key
      {
        Value = Guid.NewGuid()
      };
      attributeRequest.Attribute.AttributeType = new CrmAttributeType
      {
        IsNull = false,
        IsNullSpecified = false,
        Value = crmAttributeType
      };
      attributeRequest.Attribute.SchemaName = attributeName;
      attributeRequest.Attribute.DisplayName = new crm4.metadataservice.CrmLabel
      {
        LocLabels = new[]
        {
          new crm4.metadataservice.LocLabel
          {
            Label = attributeName,
            LanguageCode = new crm4.metadataservice.CrmNumber
            {
              Value = languagecode
            }
          }
        }
      };
      attributeRequest.Attribute.IsCustomField = new crm4.metadataservice.CrmBoolean
      {
        Value = true
      };
      attributeRequest.Attribute.RequiredLevel = new CrmAttributeRequiredLevel
      {
        Value = AttributeRequiredLevel.None
      };
      attributeRequest.Attribute.EntityLogicalName = EntityName.contact.ToString();

      try
      {
        this.CrmMetadataService.Execute(attributeRequest);
        return true;
      }
      catch (Exception e)
      {
        ConditionalLog.Error(String.Format("Couldn't create attribute '{0}' of {1} type.", attributeName, crmAttributeType), e, this);
        return false;
      }
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

        var propertiesToUpdate = new List<Property>();

        var keyProperty = new KeyProperty();
        keyProperty.Name = "contactid";
        keyProperty.Value = new crm4.webservice.Key();
        keyProperty.Value.Value = user.ID;

        propertiesToUpdate.Add(keyProperty);
        foreach (var property in properties)
        {
          this.AddPropertyToCollection(property.Key, property.Value, this.GetPropertyType(property.Key), propertiesToUpdate);
        }

        var update = new UpdateRequest
        {
          Target = new TargetUpdateDynamic
          {
            Entity = new DynamicEntity
            {
              Name = EntityName.contact.ToString(),
              Properties = propertiesToUpdate.ToArray()
            }
          }
        };

        try
        {
          this.CrmService.Execute(update);
          ConditionalLog.Info(String.Format("UpdateUserProperties({0}). Updated in CRM.", userName), this, TimerAction.Tick, UpdateUserProrpertiesKey);

          var propertyToValueConverter = new PropertyToValueConverterV4();
          foreach (var property in propertiesToUpdate)
          {
            user.SetPropertyValue(property.Name, propertyToValueConverter.Convert(property));
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
    protected void AddPropertyToCollection(string propertyName, object propertyValue, SupportedTypes propertyType, List<Property> properties)
    {
      var propertyStringValue = propertyValue as string;
      switch (propertyType)
      {
        case SupportedTypes.String:
          var stringProperty = new StringProperty();

          stringProperty.Name = propertyName;
          stringProperty.Value = (string)propertyValue;

          properties.Add(stringProperty);

          break;
        case SupportedTypes.CrmBoolean:
          if (propertyStringValue != null || (propertyValue is bool))
          {
            var booleanProperty = new CrmBooleanProperty();

            booleanProperty.Name = propertyName;
            booleanProperty.Value = new crm4.webservice.CrmBoolean();
            booleanProperty.Value.IsNull = true;

            if (propertyValue is bool)
            {
              booleanProperty.Value.Value = (bool)propertyValue;
            }
            else
            {
              booleanProperty.Value.Value = MainUtil.GetBool(propertyStringValue, false);
            }
            booleanProperty.Value.IsNull = false;

            properties.Add(booleanProperty);
          }

          break;
        case SupportedTypes.CrmDateTime:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is DateTime))
          {
            var datetimeProperty = new CrmDateTimeProperty();

            datetimeProperty.Name = propertyName;
            datetimeProperty.Value = new crm4.webservice.CrmDateTime();
            datetimeProperty.Value.IsNull = true;

            if (propertyValue is DateTime)
            {
              datetimeProperty.Value.Value = ((DateTime)propertyValue).ToString("yyyy-MM-ddTHH:mm:sszzzz");
              datetimeProperty.Value.IsNull = false;
            }
            else
            {
              DateTime dateTimeValue;
              if (DateTime.TryParse(propertyStringValue, out dateTimeValue))
              {
                datetimeProperty.Value.Value = dateTimeValue.ToString("yyyy-MM-ddTHH:mm:sszzzz");
                datetimeProperty.Value.IsNull = false;
              }
            }

            if (!datetimeProperty.Value.IsNull)
            {
              properties.Add(datetimeProperty);
            }
          }

          break;
        case SupportedTypes.CrmFloat:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is float))
          {
            var floatProperty = new CrmFloatProperty();

            floatProperty.Name = propertyName;
            floatProperty.Value = new crm4.webservice.CrmFloat();
            floatProperty.Value.IsNull = true;

            if (propertyValue is float)
            {
              floatProperty.Value.Value = (float)propertyValue;
              floatProperty.Value.IsNull = false;
            }
            else
            {
              float floatValue;
              if (float.TryParse(propertyStringValue, out floatValue))
              {
                floatProperty.Value.Value = floatValue;
                floatProperty.Value.IsNull = false;
              }
            }

            if (!floatProperty.Value.IsNull)
            {
              properties.Add(floatProperty);
            }
          }

          break;
        case SupportedTypes.CrmDecimal:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is decimal))
          {
            var decimalProperty = new CrmDecimalProperty();

            decimalProperty.Name = propertyName;
            decimalProperty.Value = new CrmDecimal();
            decimalProperty.Value.IsNull = true;

            if (propertyValue is decimal)
            {
              decimalProperty.Value.Value = (decimal)propertyValue;
              decimalProperty.Value.IsNull = false;
            }
            else
            {
              decimal decimalValue;
              if (Decimal.TryParse(propertyStringValue, out decimalValue))
              {
                decimalProperty.Value.Value = decimalValue;
                decimalProperty.Value.IsNull = false;
              }
            }

            if (!decimalProperty.Value.IsNull)
            {
              properties.Add(decimalProperty);
            }
          }

          break;
        case SupportedTypes.CrmMoney:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is decimal))
          {
            var moneyProperty = new CrmMoneyProperty();

            moneyProperty.Name = propertyName;
            moneyProperty.Value = new CrmMoney();
            moneyProperty.Value.IsNull = true;

            if (propertyValue is decimal)
            {
              moneyProperty.Value.Value = (decimal)propertyValue;
              moneyProperty.Value.IsNull = false;
            }
            else
            {
              decimal moneyValue;
              if (Decimal.TryParse(propertyStringValue, out moneyValue))
              {
                moneyProperty.Value.Value = moneyValue;
                moneyProperty.Value.IsNull = false;
              }
            }

            if (!moneyProperty.Value.IsNull)
            {
              properties.Add(moneyProperty);
            }
          }

          break;
        case SupportedTypes.CrmNumber:
          if (!String.IsNullOrEmpty(propertyStringValue) || (propertyValue is int))
          {
            var numberProperty = new CrmNumberProperty();

            numberProperty.Name = propertyName;
            numberProperty.Value = new crm4.webservice.CrmNumber();
            numberProperty.Value.IsNull = true;

            if (propertyValue is int)
            {
              numberProperty.Value.Value = (int)propertyValue;
              numberProperty.Value.IsNull = false;
            }
            else
            {
              int numberValue;
              if (Int32.TryParse(propertyStringValue, out numberValue))
              {
                numberProperty.Value.Value = numberValue;
                numberProperty.Value.IsNull = false;
              }
            }

            if (!numberProperty.Value.IsNull)
            {
              properties.Add(numberProperty);
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

            attributeRequest.EntityLogicalName = EntityName.contact.ToString();
            attributeRequest.LogicalName = propertyName;

            var attributeResponse = (RetrieveAttributeResponse)this.CrmMetadataService.Execute(attributeRequest);

            var attributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
            var options = attributeMetadata.Options.ToDictionary(option => option.Label.UserLocLabel.Label, option => option.Value.Value);

            picklistAttribute = new PicklistContactAttribute(SupportedTypes.Picklist, options);
            this.CacheService.MetadataCache.Add(propertyName, picklistAttribute);
          }

          if (!String.IsNullOrEmpty(propertyStringValue))
          {
            var value = picklistAttribute.Options[propertyStringValue];
            if (value >= 0)
            {
              var picklistProperty = new PicklistProperty();

              picklistProperty.Name = propertyName;
              picklistProperty.Value = new Picklist();
              picklistProperty.Value.Value = value;

              properties.Add(picklistProperty);
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
        AttributeMetadata attributeMetadata;
        Dictionary<string, int> options = null;

        var request = new RetrieveAttributeRequest
        {
          EntityLogicalName = EntityName.contact.ToString(),
          LogicalName = propertyName
        };

        try
        {
          var response = (RetrieveAttributeResponse)this.CrmMetadataService.Execute(request);
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
          case AttributeType.String:
          case AttributeType.Memo:
            propertyType = SupportedTypes.String;
            break;
          case AttributeType.Boolean:
            propertyType = SupportedTypes.CrmBoolean;
            break;
          case AttributeType.DateTime:
            propertyType = SupportedTypes.CrmDateTime;
            break;
          case AttributeType.Float:
            propertyType = SupportedTypes.CrmFloat;
            break;
          case AttributeType.Decimal:
            propertyType = SupportedTypes.CrmDecimal;
            break;
          case AttributeType.Money:
            propertyType = SupportedTypes.CrmMoney;
            break;
          case AttributeType.Integer:
            propertyType = SupportedTypes.CrmNumber;
            break;
          case AttributeType.Picklist:
            propertyType = SupportedTypes.Picklist;

            var picklistAttributeMetadata = attributeMetadata as PicklistAttributeMetadata;
            if (picklistAttributeMetadata != null)
            {
              options = new Dictionary<string, int>();
              picklistAttributeMetadata.Options.ToList().ForEach(option => options.Add(option.Label.UserLocLabel.Label, option.Value.Value));
            }
            break;
          default:
            ConditionalLog.Info(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.AttributeType.Value), this, TimerAction.Tick, GetPropertyTypeKey);
            throw new ProviderException(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.AttributeType.Value));
        }

        contactAttribute = (options != null) ? new PicklistContactAttribute(propertyType, options) : new ContactAttribute(propertyType);
        this.CacheService.MetadataCache.Add(propertyName, contactAttribute);
      }

      ConditionalLog.Info(String.Format("GetPropertyType({0}). Finished.", propertyName), this, TimerAction.Stop, GetPropertyTypeKey);
      return propertyType;
    }
  }
}
