// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmEntity.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmEntity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Entity
{
  using CRMSecurityProvider.Sources.Attribute;

  // ReSharper disable UnusedMemberInSuper.Global
  // ReSharper disable UnusedMember.Global

  /// <summary>
  /// The ICrmEntity interface.
  /// </summary>
  public interface ICrmEntity
  {
    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    ICrmAttributeCollection Attributes { get; }

    /// <summary>
    /// Gets or sets the name of the logical.
    /// </summary>
    /// <value>
    /// The name of the logical.
    /// </value>
    string LogicalName { get; set; }

  }

  // ReSharper restore UnusedMember.Global
  // ReSharper restore UnusedMemberInSuper.Global
}
