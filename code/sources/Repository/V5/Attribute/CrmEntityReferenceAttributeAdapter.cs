// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityReferenceAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmEntityReferenceAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System;
  using CRMSecurityProvider.Sources.Attribute;
  using Microsoft.Xrm.Sdk;

  /// <summary>
  /// The crm entity reference attribute adapter.
  /// </summary>
  internal class CrmEntityReferenceAttributeAdapter : CrmAttributeAdapter<EntityReference>, ICrmReferenceAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmEntityReferenceAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">
    /// The crm attribute collection.
    /// </param>
    /// <param name="internalAttribute">
    /// The internal attribute.
    /// </param>
    public CrmEntityReferenceAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, EntityReference internalAttribute) : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the referenced logical name.
    /// </summary>
    public string ReferencedLogicalName
    {
      get
      {
        return this.Adaptee.LogicalName;
      }
    }

    /// <summary>
    /// Gets the referenced entity id.
    /// </summary>
    public Guid ReferencedEntityId
    {
      get
      {
        return this.Adaptee.Id;
      }
    }

    /// <summary>
    /// The get stringified value.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string GetStringifiedValue()
    {
      return this.Adaptee.Name;
    }

    /// <summary>
    /// The set value.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    public override void SetValue(string value, params string[] data)
    {
      Guid id;
      if (!Guid.TryParse(value, out id) || string.IsNullOrEmpty(data[0]))
      {
        return;
      }

      this.Adaptee.Id = id;
      this.Adaptee.LogicalName = data[0];
    }
  }
}