// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  /// <summary>
  /// The crm attribute metadata adapter.
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute metadata.</typeparam>
  internal class CrmAttributeMetadataAdapter<T> : CrmAttributeMetadataAdapterBase<T> where T : AttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeMetadataAdapter{T}"/> class.
    /// </summary>
    /// <param name="attributeMetadata">
    /// The attribute metadata.
    /// </param>
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
        return this.Adaptee.DisplayName != null && this.Adaptee.DisplayName.UserLocalizedLabel != null && !string.IsNullOrEmpty(this.Adaptee.DisplayName.UserLocalizedLabel.Label) ? this.Adaptee.DisplayName.UserLocalizedLabel.Label : string.Empty;
      }
    }

    /// <summary>
    /// Gets the logical name.
    /// </summary>
    public override string LogicalName
    {
      get { return this.Adaptee.LogicalName; }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public override string Description
    {
      get
      {
        if (this.Adaptee.Description != null && this.Adaptee.Description.UserLocalizedLabel != null)
        {
          return this.Adaptee.Description.UserLocalizedLabel.Label;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for create.
    /// </summary>
    public override bool IsValidForCreate
    {
      get { return this.Adaptee.IsValidForCreate.HasValue && this.Adaptee.IsValidForCreate.Value; }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for read.
    /// </summary>
    public override bool IsValidForRead
    {
      get { return this.Adaptee.IsValidForRead.HasValue && this.Adaptee.IsValidForRead.Value; }
    }

    /// <summary>
    /// Gets a value indicating whether is valid for update.
    /// </summary>
    public override bool IsValidForUpdate
    {
      get { return this.Adaptee.IsValidForUpdate.HasValue && this.Adaptee.IsValidForUpdate.Value; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is valid for advanced find.
    /// </summary>
    public override bool IsValidForAdvancedFind
    {
      get
      {
        return this.Adaptee.IsValidForAdvancedFind.Value;
      }
    }

    /// <summary>
    /// Gets the required level.
    /// </summary>
    public override CrmAttributeRequiredLevel RequiredLevel
    {
      get
      {
        if (this.Adaptee.RequiredLevel == null)
        {
          return CrmAttributeRequiredLevel.Empty;
        }

        switch (this.Adaptee.RequiredLevel.Value)
        {
          case AttributeRequiredLevel.ApplicationRequired:
            return CrmAttributeRequiredLevel.ApplicationRequired;

          case AttributeRequiredLevel.None:
            return CrmAttributeRequiredLevel.None;

          case AttributeRequiredLevel.Recommended:
            return CrmAttributeRequiredLevel.Recommended;

          case AttributeRequiredLevel.SystemRequired:
            return CrmAttributeRequiredLevel.SystemRequired;
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
        if (this.Adaptee.AttributeType == null)
        {
          return CrmAttributeType.Empty;
        }

        switch (this.Adaptee.AttributeType)
        {
          case AttributeTypeCode.Boolean:
            return CrmAttributeType.Boolean;

          case AttributeTypeCode.Customer:
            return CrmAttributeType.Customer;

          case AttributeTypeCode.DateTime:
            return CrmAttributeType.DateTime;

          case AttributeTypeCode.Decimal:
            return CrmAttributeType.Decimal;
          case AttributeTypeCode.Double:
            return CrmAttributeType.Double;
          case AttributeTypeCode.Integer:
            return CrmAttributeType.Integer;

          case AttributeTypeCode.Lookup:
            return CrmAttributeType.Lookup;

          case AttributeTypeCode.Memo:
            return CrmAttributeType.Memo;

          case AttributeTypeCode.Money:
            return CrmAttributeType.Money;

          case AttributeTypeCode.Owner:
            return CrmAttributeType.Owner;

          case AttributeTypeCode.PartyList:
            return CrmAttributeType.PartyList;

          case AttributeTypeCode.Picklist:
            return CrmAttributeType.Picklist;

          case AttributeTypeCode.State:
            return CrmAttributeType.State;

          case AttributeTypeCode.Status:
            return CrmAttributeType.Status;

          case AttributeTypeCode.String:
            return CrmAttributeType.String;

          case AttributeTypeCode.Uniqueidentifier:
            return CrmAttributeType.UniqueIdentifier;

          case AttributeTypeCode.CalendarRules:
            return CrmAttributeType.CalendarRules;

          case AttributeTypeCode.Virtual:
            return CrmAttributeType.Virtual;
          case AttributeTypeCode.BigInt:
            return CrmAttributeType.BigInt;
          case AttributeTypeCode.ManagedProperty:
            return CrmAttributeType.ManagedProperty;
          case AttributeTypeCode.EntityName:
            return CrmAttributeType.EntityName;
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