// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute.Metadata
{
  using CRMSecurityProvider.crm4.metadataservice;
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
        return this.Adaptee.DisplayName != null && this.Adaptee.DisplayName.UserLocLabel != null && !string.IsNullOrEmpty(this.Adaptee.DisplayName.UserLocLabel.Label) ? this.Adaptee.DisplayName.UserLocLabel.Label : string.Empty;
      }
    }

    /// <summary>
    /// Gets the logical name.
    /// </summary>
    public override string LogicalName
    {
      get
      {
        return this.Adaptee.LogicalName;
      }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public override string Description
    {
      get
      {
        if (this.Adaptee.Description != null && this.Adaptee.Description.UserLocLabel != null)
        {
          return this.Adaptee.Description.UserLocLabel.Label;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for create.
    /// </summary>
    public override bool IsValidForCreate
    {
      get
      {
        return this.Adaptee.ValidForCreate.Value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for read.
    /// </summary>
    public override bool IsValidForRead
    {
      get
      {
        return this.Adaptee.ValidForRead.Value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for update.
    /// </summary>
    public override bool IsValidForUpdate
    {
      get
      {
        return this.Adaptee.ValidForUpdate.Value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is valid for advanced find.
    /// </summary>
    public override bool IsValidForAdvancedFind
    {
      get
      {
        return this.Adaptee.DisplayMask != null && (this.Adaptee.DisplayMask.Value & DisplayMasks.ValidForAdvancedFind) != 0;
      }
    }

    /// <summary>
    /// Gets the required level.
    /// </summary>
    public override Sources.Attribute.CrmAttributeRequiredLevel RequiredLevel
    {
      get
      {
        if (this.Adaptee.RequiredLevel == null)
        {
          return Sources.Attribute.CrmAttributeRequiredLevel.Empty;
        }

        switch (this.Adaptee.RequiredLevel.Value)
        {
          case AttributeRequiredLevel.Required:
            return Sources.Attribute.CrmAttributeRequiredLevel.ApplicationRequired;

          case AttributeRequiredLevel.None:
            return Sources.Attribute.CrmAttributeRequiredLevel.None;

          case AttributeRequiredLevel.Recommended:
            return Sources.Attribute.CrmAttributeRequiredLevel.Recommended;

          case AttributeRequiredLevel.SystemRequired:
            return Sources.Attribute.CrmAttributeRequiredLevel.SystemRequired;

          case AttributeRequiredLevel.ReadOnly:
            return Sources.Attribute.CrmAttributeRequiredLevel.ReadOnly;
        }

        return Sources.Attribute.CrmAttributeRequiredLevel.Unknown;
      }
    }

    /// <summary>
    /// Gets the attribute type.
    /// </summary>
    public override Sources.Attribute.CrmAttributeType AttributeType
    {
      get
      {
        if (this.Adaptee.AttributeType == null)
        {
          return Sources.Attribute.CrmAttributeType.Empty;
        }

        switch (this.Adaptee.AttributeType.Value)
        {
          case crm4.metadataservice.AttributeType.Boolean:
            return Sources.Attribute.CrmAttributeType.Boolean;

          case crm4.metadataservice.AttributeType.Customer:
            return Sources.Attribute.CrmAttributeType.Customer;

          case crm4.metadataservice.AttributeType.DateTime:
            return Sources.Attribute.CrmAttributeType.DateTime;

          case crm4.metadataservice.AttributeType.Decimal:
            return Sources.Attribute.CrmAttributeType.Decimal;

          case crm4.metadataservice.AttributeType.Float:
            return Sources.Attribute.CrmAttributeType.Float;

          case crm4.metadataservice.AttributeType.Integer:
            return Sources.Attribute.CrmAttributeType.Integer;

          case crm4.metadataservice.AttributeType.Internal:
            return Sources.Attribute.CrmAttributeType.Internal;

          case crm4.metadataservice.AttributeType.Lookup:
            return Sources.Attribute.CrmAttributeType.Lookup;

          case crm4.metadataservice.AttributeType.Memo:
            return Sources.Attribute.CrmAttributeType.Memo;

          case crm4.metadataservice.AttributeType.Money:
            return Sources.Attribute.CrmAttributeType.Money;

          case crm4.metadataservice.AttributeType.Owner:
            return Sources.Attribute.CrmAttributeType.Owner;

          case crm4.metadataservice.AttributeType.PartyList:
            return Sources.Attribute.CrmAttributeType.PartyList;

          case crm4.metadataservice.AttributeType.Picklist:
            return Sources.Attribute.CrmAttributeType.Picklist;

          case crm4.metadataservice.AttributeType.PrimaryKey:
            return Sources.Attribute.CrmAttributeType.PrimaryKey;

          case crm4.metadataservice.AttributeType.State:
            return Sources.Attribute.CrmAttributeType.State;

          case crm4.metadataservice.AttributeType.Status:
            return Sources.Attribute.CrmAttributeType.Status;

          case crm4.metadataservice.AttributeType.String:
            return Sources.Attribute.CrmAttributeType.String;

          case crm4.metadataservice.AttributeType.UniqueIdentifier:
            return Sources.Attribute.CrmAttributeType.UniqueIdentifier;

          case crm4.metadataservice.AttributeType.Virtual:
            return Sources.Attribute.CrmAttributeType.Virtual;

          case crm4.metadataservice.AttributeType.CalendarRules:
            return Sources.Attribute.CrmAttributeType.CalendarRules;
        }

        return Sources.Attribute.CrmAttributeType.Unknown;
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