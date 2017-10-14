// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityMetadata.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmEntityMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Entity
{
  using System.Linq;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The CRM entity metadata.
  /// </summary>
  public abstract class CrmEntityMetadata
  {
    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBeProtected.Global

    /// <summary>
    /// Gets the empty instance.
    /// </summary>
    public static CrmEntityMetadata Empty
    {
      get { return EmptyCrmEntityMetadata.Instance; }
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    public abstract ICrmAttributeMetadata[] Attributes { get; }

    /// <summary>
    /// Gets the primary key.
    /// </summary>
    /// <value>
    /// The primary key.
    /// </value>
    public abstract string PrimaryKey { get; }

    /// <summary>
    /// Gets the primary field.
    /// </summary>
    /// <value>
    /// The primary field.
    /// </value>
    public abstract string PrimaryField { get; }

    /// <summary>
    /// Gets the name of the logical.
    /// </summary>
    /// <value>
    /// The name of the logical.
    /// </value>
    public abstract string LogicalName { get; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is customizable.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is customizable; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsCustomizable { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is custom entity.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is custom entity; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsCustomEntity { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title
    {
      get
      {
        return string.IsNullOrEmpty(this.DisplayName) ? this.LogicalName : this.DisplayName;
      }
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <returns>The <see cref="ICrmAttributeMetadata"/>.</returns>
    public ICrmAttributeMetadata GetAttribute(string logicalName)
    {
      return this.Attributes != null ? this.Attributes.FirstOrDefault(a => a.LogicalName == logicalName) : null;
    }
  }

  // ReSharper restore MemberCanBeProtected.Global
  // ReSharper restore UnusedMember.Global
}
