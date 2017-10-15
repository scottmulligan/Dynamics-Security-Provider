// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm attribute adapter.
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute.</typeparam>
  public abstract class CrmAttributeAdapter<T> : CrmAttributeAdapterBase<T>
  {
    /// <summary>
    /// The CRM attribute collection
    /// </summary>
    private readonly CrmAttributeCollectionAdapter crmAttributeCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeAdapter{T}"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    protected CrmAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, T internalAttribute)
      : base(internalAttribute)
    {
      this.crmAttributeCollection = crmAttributeCollection;
    }

    /// <summary>
    /// Gets the attribute collection.
    /// </summary>
    /// <value>
    /// The attribute collection.
    /// </value>
    public CrmAttributeCollectionAdapter AttributeCollection
    {
      get
      {
        return this.crmAttributeCollection;
      }
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public override string Name { get; set; }
  }
}