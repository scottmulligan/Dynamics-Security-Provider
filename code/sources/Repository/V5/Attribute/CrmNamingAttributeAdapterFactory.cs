// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmNamingAttributeAdapterFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmNamingAttributeAdapterFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm naming attribute adapter factory.
  /// </summary>
  internal class CrmNamingAttributeAdapterFactory : CrmAttributeAdapterFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmNamingAttributeAdapterFactory"/> class.
    /// </summary>
    /// <param name="crmAttributeCollectionAdapter">
    /// The crm attribute collection adapter.
    /// </param>
    public CrmNamingAttributeAdapterFactory(CrmAttributeCollectionAdapter crmAttributeCollectionAdapter) : base(crmAttributeCollectionAdapter)
    {
    }

    /// <summary>
    /// Creates the specified attribute name.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="adaptee">The adaptee.</param>
    /// <returns>The crm attribute</returns>
    public ICrmAttribute Create(string attributeName, object adaptee)
    {
      var returnValue = base.Create(adaptee);
      returnValue.Name = attributeName;
      return returnValue;
    }
  }
}