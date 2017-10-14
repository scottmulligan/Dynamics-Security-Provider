// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmReferenceAttribute.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmReferenceAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  using System;

  // ReSharper disable UnusedMember.Global

  /// <summary>
  /// The CrmReferenceAttribute interface.
  /// </summary>
  public interface ICrmReferenceAttribute : ICrmAttribute
  {
    /// <summary>
    /// Gets the name of the referenced logical.
    /// </summary>
    /// <value>
    /// The name of the referenced logical.
    /// </value>
    string ReferencedLogicalName { get; }

    /// <summary>
    /// Gets the referenced entity id.
    /// </summary>
    /// <value>
    /// The referenced entity id.
    /// </value>
    Guid ReferencedEntityId { get; }
  }

  // ReSharper restore UnusedMember.Global
}