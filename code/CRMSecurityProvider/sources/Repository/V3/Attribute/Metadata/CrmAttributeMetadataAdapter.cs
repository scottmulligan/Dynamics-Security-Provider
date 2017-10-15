// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm attribute metadata adapter.
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute metadata.</typeparam>
  internal class CrmAttributeMetadataAdapter<T> : CrmAttributeMetadataAdapterBase<T> where T : AttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeMetadataAdapter{T}"/> class.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    public CrmAttributeMetadataAdapter(T attributeMetadata)
      : base(attributeMetadata)
    {
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public override string DisplayName
    {
      get
      {
        return this.Adaptee.DisplayName;
      }
    }

    /// <summary>
    /// Gets the logical name.
    /// </summary>
    public override string LogicalName
    {
      get
      {
        return this.Adaptee.Name;
      }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public override string Description
    {
      get
      {
        return this.Adaptee.Description;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for create.
    /// </summary>
    public override bool IsValidForCreate
    {
      get
      {
        return this.Adaptee.ValidForCreate;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for read.
    /// </summary>
    public override bool IsValidForRead
    {
      get
      {
        return this.Adaptee.ValidForRead;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for update.
    /// </summary>
    public override bool IsValidForUpdate
    {
      get
      {
        return this.Adaptee.ValidForUpdate;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is valid for advanced find.
    /// </summary>
    public override bool IsValidForAdvancedFind
    {
      get
      {
        return (this.Adaptee.DisplayMask & DisplayMasks.ValidForAdvancedFind) != 0;
      }
    }

    /// <summary>
    /// Gets the required level.
    /// </summary>
    public override CrmAttributeRequiredLevel RequiredLevel
    {
      get
      {
        switch (this.Adaptee.RequiredLevel)
        {
          case AttributeRequiredLevel.Required:
            return CrmAttributeRequiredLevel.ApplicationRequired;

          case AttributeRequiredLevel.None:
            return CrmAttributeRequiredLevel.None;

          case AttributeRequiredLevel.Recommended:
            return CrmAttributeRequiredLevel.Recommended;

          case AttributeRequiredLevel.SystemRequired:
            return CrmAttributeRequiredLevel.SystemRequired;

          case AttributeRequiredLevel.ReadOnly:
            return CrmAttributeRequiredLevel.ReadOnly;
        }

        return CrmAttributeRequiredLevel.Unknown;
      }
    }

    /// <summary>
    /// Gets the attribute type.
    /// </summary>
    public override CrmAttributeType AttributeType
    {
      get
      {
        switch (this.Adaptee.Type)
        {
          case crm3.metadataservice.AttributeType.Boolean:
            return CrmAttributeType.Boolean;

          case crm3.metadataservice.AttributeType.Customer:
            return CrmAttributeType.Customer;

          case crm3.metadataservice.AttributeType.DateTime:
            return CrmAttributeType.DateTime;

          case crm3.metadataservice.AttributeType.Decimal:
            return CrmAttributeType.Decimal;

          case crm3.metadataservice.AttributeType.Float:
            return CrmAttributeType.Float;

          case crm3.metadataservice.AttributeType.Integer:
            return CrmAttributeType.Integer;

          case crm3.metadataservice.AttributeType.Internal:
            return CrmAttributeType.Internal;

          case crm3.metadataservice.AttributeType.Lookup:
            return CrmAttributeType.Lookup;

          case crm3.metadataservice.AttributeType.Memo:
            return CrmAttributeType.Memo;

          case crm3.metadataservice.AttributeType.Money:
            return CrmAttributeType.Money;

          case crm3.metadataservice.AttributeType.Owner:
            return CrmAttributeType.Owner;

          case crm3.metadataservice.AttributeType.PartyList:
            return CrmAttributeType.PartyList;

          case crm3.metadataservice.AttributeType.Picklist:
            return CrmAttributeType.Picklist;

          case crm3.metadataservice.AttributeType.PrimaryKey:
            return CrmAttributeType.PrimaryKey;

          case crm3.metadataservice.AttributeType.State:
            return CrmAttributeType.State;

          case crm3.metadataservice.AttributeType.Status:
            return CrmAttributeType.Status;

          case crm3.metadataservice.AttributeType.String:
            return CrmAttributeType.String;

          case crm3.metadataservice.AttributeType.UniqueIdentifier:
            return CrmAttributeType.UniqueIdentifier;

          case crm3.metadataservice.AttributeType.Virtual:
            return CrmAttributeType.Virtual;

          case crm3.metadataservice.AttributeType.CalendarRules:
            return CrmAttributeType.CalendarRules;
        }

        return CrmAttributeType.Unknown;
      }
    }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public override string Title
    {
      get
      {
        return string.IsNullOrEmpty(this.DisplayName) ? this.LogicalName : this.DisplayName;
      }
    }
  }
}