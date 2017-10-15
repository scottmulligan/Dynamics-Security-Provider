// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyCrmEntityMetadata.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyCrmEntityMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Entity
{
  using System.Linq;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The empty CRM entity metadata.
  /// </summary>
  internal sealed class EmptyCrmEntityMetadata : CrmEntityMetadata
  {
    /// <summary>
    /// The Instance
    /// </summary>
    public static readonly CrmEntityMetadata Instance = new EmptyCrmEntityMetadata();

    /// <summary>
    /// The attributes
    /// </summary>
    private readonly ICrmAttributeMetadata[] attributes = Enumerable.Empty<ICrmAttributeMetadata>().ToArray();

    /// <summary>
    /// Prevents a default instance of the <see cref="EmptyCrmEntityMetadata"/> class from being created.
    /// </summary>
    private EmptyCrmEntityMetadata()
    {
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
      get { return string.Empty; }
    }

    /// <summary>
    /// Gets the primary field.
    /// </summary>
    /// <value>
    /// The primary field.
    /// </value>
    public override string PrimaryField
    {
      get { return string.Empty; }
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
        return string.Empty;
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
      get { return string.Empty; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is customizable.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is customizable; otherwise, <c>false</c>.
    /// </value>
    public override bool IsCustomizable
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is custom entity.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is custom entity; otherwise, <c>false</c>.
    /// </value>
    public override bool IsCustomEntity
    {
      get { return false; }
    }
  }
}
