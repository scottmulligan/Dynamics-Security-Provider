// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmOptionsAttributeMetadata.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmOptionsAttributeMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute.Metadata
{
  using System.Collections.Generic;

  /// <summary>
  /// The CrmOptionsAttributeMetadata interface.
  /// </summary>
  public interface ICrmOptionsAttributeMetadata : ICrmAttributeMetadata
  {
    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    IEnumerable<KeyValuePair<int, string>> Options { get; }
  }
}