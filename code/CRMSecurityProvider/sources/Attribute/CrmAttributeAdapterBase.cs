// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeAdapterBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeAdapterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  /// <summary>
  /// The crm attribute adapter base
  /// </summary>
  /// <typeparam name="T">Type of adaptee attribute.</typeparam>
  public abstract class CrmAttributeAdapterBase<T> : AdapterBase<T>, ICrmAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeAdapterBase{T}"/> class.
    /// </summary>
    /// <param name="internalAttribute">The internal attribute.</param>
    protected CrmAttributeAdapterBase(T internalAttribute)
      : base(internalAttribute)
    {
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public abstract string Name { get; set; }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>Stringified value</returns>
    public abstract string GetStringifiedValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public abstract void SetValue(string value, params string[] data);

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this.GetStringifiedValue();
    }
  }
}