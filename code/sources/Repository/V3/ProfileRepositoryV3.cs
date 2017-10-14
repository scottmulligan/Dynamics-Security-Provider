namespace CRMSecurityProvider.Repository.V3
{
  using System;
  using System.Collections.Generic;
  using System.Configuration.Provider;
  using System.Linq;

  using Sitecore;
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.crm3.webservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Profile repository class (API v3).
  /// </summary>
  public class ProfileRepositoryV3 : ProfileRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileRepositoryV3"/> class.
    /// </summary>
    /// <param name="metadataService">The CRM metadata service.</param>
    /// <param name="crmService">The CRM service.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    public ProfileRepositoryV3(IMetadataServiceV3 metadataService, ICrmServiceV3 crmService, UserRepositoryBase userRepository, ICacheService cacheService)
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
    protected IMetadataServiceV3 CrmMetadataService { get; private set; }

    /// <summary>
    /// Gets or sets the CRM service.
    /// </summary>
    protected ICrmServiceV3 CrmService { get; private set; }

    /// <exception cref="ProviderException">Creating the attributes isn't supported by the CRM service v3.</exception>
    public override bool CreateContactAttribute(string attributeName, SupportedTypes attributeType, bool throwIfExists)
    {
      throw new ProviderException("Creating the attributes isn't supported by the CRM service v3.");
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
        keyProperty.Value = new Key();
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

          var propertyToValueConverter = new PropertyToValueConverterV3();
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
            booleanProperty.Value = new CrmBoolean();
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
            datetimeProperty.Value = new CrmDateTime();

            var propertyValid = false;
            if (propertyValue is DateTime)
            {
              datetimeProperty.Value.Value = ((DateTime)propertyValue).ToString("yyyy-MM-ddTHH:mm:sszzzz");
              propertyValid = true;
            }
            else
            {
              DateTime dateTimeValue;
              if (DateTime.TryParse(propertyStringValue, out dateTimeValue))
              {
                datetimeProperty.Value.Value = dateTimeValue.ToString("yyyy-MM-ddTHH:mm:sszzzz");
                propertyValid = true;
              }
            }

            if (propertyValid)
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
            floatProperty.Value = new CrmFloat();
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
            numberProperty.Value = new CrmNumber();
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
            var attributeMetadata = (PicklistAttributeMetadata)this.CrmMetadataService.RetrieveAttributeMetadata(EntityName.contact.ToString(), propertyName);
            var options = attributeMetadata.Options.ToDictionary(option => option.Description, option => option.OptionValue);

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

        try
        {
          attributeMetadata = this.CrmMetadataService.RetrieveAttributeMetadata(EntityName.contact.ToString(), propertyName);
          ConditionalLog.Info(String.Format("GetPropertyType({0}). Retrieved from CRM.", propertyName), this, TimerAction.Tick, GetPropertyTypeKey);
        }
        catch (Exception e)
        {
          ConditionalLog.Error(String.Format("Couldn't retrieve metadata for the {0} attribute from CRM.", propertyName), e, this);
          throw new ProviderException(String.Format("Couldn't retrieve metadata for the {0} attribute from CRM.", propertyName), e);
        }

        switch (attributeMetadata.Type)
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
              picklistAttributeMetadata.Options.ToList().ForEach(option => options.Add(option.Description, option.OptionValue));
            }
            break;
          default:
            ConditionalLog.Info(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.Type), this, TimerAction.Tick, GetPropertyTypeKey);
            throw new ProviderException(String.Format("The {0} attribute is of unsupported ({1}) type.", propertyName, attributeMetadata.Type));
        }

        contactAttribute = (options != null) ? new PicklistContactAttribute(propertyType, options) : new ContactAttribute(propertyType);
        this.CacheService.MetadataCache.Add(propertyName, contactAttribute);
      }

      ConditionalLog.Info(String.Format("GetPropertyType({0}). Finished.", propertyName), this, TimerAction.Stop, GetPropertyTypeKey);
      return propertyType;
    }
  }
}
