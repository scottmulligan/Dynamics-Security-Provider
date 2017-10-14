// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeAdapterFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeAdapterFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute
{
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm attribute adapter factory.
  /// </summary>
  internal class CrmAttributeAdapterFactory : AdapterFactoryBase<Property, ICrmAttribute>
  {
    /// <summary>
    /// The CRM attribute collection adapter
    /// </summary>
    private readonly CrmAttributeCollectionAdapter crmAttributeCollectionAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeAdapterFactory"/> class.
    /// </summary>
    /// <param name="crmAttributeCollectionAdapter">The CRM attribute collection adapter.</param>
    public CrmAttributeAdapterFactory(CrmAttributeCollectionAdapter crmAttributeCollectionAdapter)
    {
      this.crmAttributeCollectionAdapter = crmAttributeCollectionAdapter;
    }

    /// <summary>
    /// Creates crm attribute adapter for the specified adaptee property.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    /// <returns>Crm attribute</returns>
    public override ICrmAttribute Create(Property adaptee)
    {
      if (adaptee is StateProperty)
      {
        return new CrmStateAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as StateProperty);
      }

      if (adaptee is StatusProperty)
      {
        return new CrmStatusAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as StatusProperty);
      }

      return new CrmPropertyAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee);
    }
  }
}