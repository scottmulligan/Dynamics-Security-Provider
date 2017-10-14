// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmIntegerAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmIntegerAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm integer attribute adapter.
  /// </summary>
  internal class CrmIntegerAttributeAdapter : CrmValueTypeAttributeAdapter<int>, ICrmIntegerAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmIntegerAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmIntegerAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, int internalAttribute) : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Tries the parse value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if parsing succeeded, otherwise - false.</returns>
    protected override bool TryParseValue(string value, out int result)
    {
      return int.TryParse(value, out result);
    }
  }
}