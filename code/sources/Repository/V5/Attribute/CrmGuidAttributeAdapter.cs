// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmGuidAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmGuidAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System;
  using CRMSecurityProvider.Sources.Attribute;
  using Sitecore.Data;

  /// <summary>
  /// The crm guid attribute adapter.
  /// </summary>
  internal class CrmGuidAttributeAdapter : CrmValueTypeAttributeAdapter<Guid>, ICrmGuidAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmGuidAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmGuidAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, Guid internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString();
    }

    /// <summary>
    /// Tries the parse value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if parsing succeeded, otherwise - false.</returns>
    protected override bool TryParseValue(string value, out Guid result)
    {
      ID sitecoreId;
      if (ID.TryParse(value, out sitecoreId))
      {
        result = sitecoreId.Guid;
        return true;
      }

      return Guid.TryParse(value, out result);
    }
  }
}