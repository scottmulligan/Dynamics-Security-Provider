// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmStatusAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmStatusAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute
{
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm status attribute adapter.
  /// </summary>
  internal class CrmStatusAttributeAdapter : CrmAttributeAdapter<StatusProperty>, ICrmKeyValueAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmStatusAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmStatusAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, StatusProperty internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public int Key
    {
      get
      {
        return this.Adaptee.Value != null ? this.Adaptee.Value.Value : -1;
      }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value
    {
      get
      {
        return this.Adaptee.Value != null ? this.Adaptee.Value.name : string.Empty;
      }
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    public override string GetStringifiedValue()
    {
      return this.Value;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public override void SetValue(string value, params string[] data)
    {
      int intState;
      if (!int.TryParse(value, out intState))
      {
        return;
      }

      if (this.Adaptee.Value == null)
      {
        this.Adaptee.Value = new Status();
      }

      this.Adaptee.Value.Value = intState;
    }
  }
}