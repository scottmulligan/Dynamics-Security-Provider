// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmEntityMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Entity
{
  using System.Linq;
  using CRMSecurityProvider.crm4.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.Repository.V4.Attribute.Metadata;

  /// <summary>
  /// The crm entity metadata adapter.
  /// </summary>
  internal class CrmEntityMetadataAdapter : CrmEntityMetadata
  {
    /// <summary>
    /// The entity metadata
    /// </summary>
    private readonly EntityMetadata entityMetadata;

    /// <summary>
    /// The attributes
    /// </summary>
    private readonly ICrmAttributeMetadata[] attributes;

    /// <summary>
    /// The attribute attribute metadata factory
    /// </summary>
    private readonly CrmAttributeMetadataFactory attributeMetadataFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmEntityMetadataAdapter"/> class.
    /// </summary>
    /// <param name="entityMetadata">The entity metadata.</param>
    public CrmEntityMetadataAdapter(EntityMetadata entityMetadata)
    {
      this.entityMetadata = entityMetadata;
      this.attributeMetadataFactory = new CrmAttributeMetadataFactory();

      if (entityMetadata.Attributes == null)
      {
        return;
      }

      this.attributes = entityMetadata.Attributes.Select(a => this.attributeMetadataFactory.Create(a)).ToArray();
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    public override ICrmAttributeMetadata[] Attributes
    {
      get
      {
        return this.attributes;
      }
    }

    /// <summary>
    /// Gets the primary key.
    /// </summary>
    /// <value>
    /// The primary key.
    /// </value>
    public override string PrimaryKey
    {
      get
      {
        return this.entityMetadata.PrimaryKey;
      }
    }

    /// <summary>
    /// Gets the primary field.
    /// </summary>
    /// <value>
    /// The primary field.
    /// </value>
    public override string PrimaryField
    {
      get
      {
        return this.entityMetadata.PrimaryField;
      }
    }

    /// <summary>
    /// Gets the name of the logical.
    /// </summary>
    /// <value>
    /// The name of the logical.
    /// </value>
    public override string LogicalName
    {
      get
      {
        return this.entityMetadata.LogicalName;
      }
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public override string DisplayName
    {
      get { return this.entityMetadata.DisplayName != null && this.entityMetadata.DisplayName.UserLocLabel != null && !string.IsNullOrEmpty(this.entityMetadata.DisplayName.UserLocLabel.Label) ? this.entityMetadata.DisplayName.UserLocLabel.Label : string.Empty; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is customizable.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is customizable; otherwise, <c>false</c>.
    /// </value>
    public override bool IsCustomizable
    {
      get
      {
        return this.entityMetadata.IsCustomizable.Value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is custom entity.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is custom entity; otherwise, <c>false</c>.
    /// </value>
    public override bool IsCustomEntity
    {
      get
      {
        return this.entityMetadata.IsCustomEntity.Value;
      }
    }
  }
}