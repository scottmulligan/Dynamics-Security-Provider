// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeAdapterFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeAdapterFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System;
  using CRMSecurityProvider.Sources.Attribute;
  using Microsoft.Xrm.Sdk;

  /// <summary>
  /// The crm attribute adapter factory.
  /// </summary>
  internal class CrmAttributeAdapterFactory : AdapterFactoryBase<object, ICrmAttribute>
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
    /// Creates the specified adaptee.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    /// <returns>The crm attribute.</returns>
    public override ICrmAttribute Create(object adaptee)
    {
      if (adaptee is OptionSetValue)
      {
        return new CrmOptionSetValueAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as OptionSetValue);
      }

      if (adaptee is EntityReference)
      {
        return new CrmEntityReferenceAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as EntityReference);
      }

      if (adaptee is string)
      {
        return new CrmStringAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as string);
      }

      if (adaptee is int)
      {
        return new CrmIntegerAttributeAdapter(this.crmAttributeCollectionAdapter, (int)adaptee);
      }

      if (adaptee is long)
      {
        return new CrmLongAttributeAdapter(this.crmAttributeCollectionAdapter, (long)adaptee);
      }

      if (adaptee is bool)
      {
        return new CrmBoolAttributeAdapter(this.crmAttributeCollectionAdapter, (bool)adaptee);
      }

      if (adaptee is DateTime)
      {
        return new CrmDateTimeAttributeAdapter(this.crmAttributeCollectionAdapter, (DateTime)adaptee);
      }

      if (adaptee is decimal)
      {
        return new CrmDecimalAttributeAdapter(this.crmAttributeCollectionAdapter, (decimal)adaptee);
      }

      if (adaptee is double)
      {
        return new CrmDoubleAttributeAdapter(this.crmAttributeCollectionAdapter, (double)adaptee);
      }

      if (adaptee is Guid)
      {
        return new CrmGuidAttributeAdapter(this.crmAttributeCollectionAdapter, (Guid)adaptee);
      }

      if (adaptee is Money)
      {
        return new CrmMoneyAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee as Money);
      }

      return new CrmObjectAttributeAdapter(this.crmAttributeCollectionAdapter, adaptee);
    }
  }
}