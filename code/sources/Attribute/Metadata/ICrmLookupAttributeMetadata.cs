// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmLookupAttributeMetadata.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmLookupAttributeMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute.Metadata
{
  // ReSharper disable UnusedMember.Global

  /// <summary>
  /// The CrmLookupAttributeMetadata interface.
  /// </summary>
  public interface ICrmLookupAttributeMetadata : ICrmAttributeMetadata
  {
    /// <summary>
    /// Gets the targets.
    /// </summary>
    /// <value>
    /// The targets.
    /// </value>
    string[] Targets { get; }
  }

  // ReSharper restore UnusedMember.Global
}