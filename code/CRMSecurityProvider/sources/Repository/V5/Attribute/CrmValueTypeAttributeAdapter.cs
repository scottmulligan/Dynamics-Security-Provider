// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmValueTypeAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmValueTypeAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm value type attribute adapter.
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute.</typeparam>
  internal abstract class CrmValueTypeAttributeAdapter<T> : CrmAttributeAdapter<T>, ICrmAttribute<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmValueTypeAttributeAdapter{T}"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    protected CrmValueTypeAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, T internalAttribute) : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public T Value
    {
      get
      {
        return this.Adaptee;
      }
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public override void SetValue(string value, params string[] data)
    {
      T valueToBeSet;
      if (!this.TryParseValue(value, out valueToBeSet))
      {
        return;
      }

      this.AttributeCollection.SetValue(this.Name, valueToBeSet);
    }

    /// <summary>
    /// Tries the parse value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if parsing succeeded, otherwise - false.</returns>
    protected abstract bool TryParseValue(string value, out T result);
  }
}