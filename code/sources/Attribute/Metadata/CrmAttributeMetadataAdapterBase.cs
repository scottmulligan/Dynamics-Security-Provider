// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataAdapterBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataAdapterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm attribute metadata adapter base.
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute metadata.</typeparam>
  internal abstract class CrmAttributeMetadataAdapterBase<T> : AdapterBase<T>, ICrmAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeMetadataAdapterBase{T}"/> class.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    protected CrmAttributeMetadataAdapterBase(T adaptee)
      : base(adaptee)
    {
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the logical name.
    /// </summary>
    public abstract string LogicalName { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for create.
    /// </summary>
    public abstract bool IsValidForCreate { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for read.
    /// </summary>
    public abstract bool IsValidForRead { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for update.
    /// </summary>
    public abstract bool IsValidForUpdate { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is valid for advanced find.
    /// </summary>
    public abstract bool IsValidForAdvancedFind { get; }

    /// <summary>
    /// Gets the required level.
    /// </summary>
    public abstract CrmAttributeRequiredLevel RequiredLevel { get; }

    /// <summary>
    /// Gets the attribute type.
    /// </summary>
    public abstract CrmAttributeType AttributeType { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public abstract string Title { get; }
  }
}