// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmObjectAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   The crm object attribute adapter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  /// <summary>
  /// The crm object attribute adapter.
  /// </summary>
  internal class CrmObjectAttributeAdapter : CrmAttributeAdapter<object>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmObjectAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmObjectAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, object internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value</returns>
    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(); 
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public override void SetValue(string value, params string[] data)
    {
      this.AttributeCollection.SetValue(this.Name, value);
    }
  }
}
