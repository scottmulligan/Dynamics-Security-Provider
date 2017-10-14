// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmAttributeCollection.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmAttributeCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  using System.Collections.Generic;

  // ReSharper disable UnusedMember.Global

  /// <summary>
  /// The CrmAttributeCollection interface.
  /// </summary>
  public interface ICrmAttributeCollection : IEnumerable<KeyValuePair<string, ICrmAttribute>>
  {
    /// <summary>
    /// Gets the <see cref="System.Object"/> with the specified key.
    /// </summary>
    /// <value>
    /// The <see cref="System.Object"/>.
    /// </value>
    /// <param name="key">The key.</param>
    /// <returns>The <see cref="object"/>.</returns>
    ICrmAttribute this[string key] { get; }

    /// <summary>
    /// Creates the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    /// <returns>Created crm attribute</returns>
    ICrmAttribute Create(string name, CrmAttributeType type, string value, params string[] data);

    /// <summary>
    /// Removes the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    void Remove(string name);
  }
  
  // ReSharper restore UnusedMember.Global
}