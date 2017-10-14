// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmAttribute.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  // ReSharper disable UnusedMemberInSuper.Global

  /// <summary>
  /// The CrmAttribute interface.
  /// </summary>
  /// <typeparam name="T">Type of attribute value.</typeparam>
  public interface ICrmAttribute<out T> : ICrmAttribute
  {
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    T Value { get; }
  }

  /// <summary>
  /// The CrmAttribute interface.
  /// </summary>
  public interface ICrmAttribute
  {
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    string Name { get; set; }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    string GetStringifiedValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    void SetValue(string value, params string[] data);
  }

  // ReSharper restore UnusedMemberInSuper.Global
}
