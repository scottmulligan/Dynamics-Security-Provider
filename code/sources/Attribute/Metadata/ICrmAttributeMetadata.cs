// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrmAttributeMetadata.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICrmAttributeMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute.Metadata
{
  // ReSharper disable UnusedMember.Global
  // ReSharper disable UnusedMemberInSuper.Global

  /// <summary>
  /// The CrmAttributeMetadata interface.
  /// </summary>
  public interface ICrmAttributeMetadata
  {
    /// <summary>
    /// Gets the display name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the logical name.
    /// </summary>
    string LogicalName { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for create.
    /// </summary>
    bool IsValidForCreate { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for read.
    /// </summary>
    bool IsValidForRead { get; }

    /// <summary>
    /// Gets a value indicating whether is valid for update.
    /// </summary>
    bool IsValidForUpdate { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is valid for advanced find.
    /// </summary>
    bool IsValidForAdvancedFind { get; }

    /// <summary>
    /// Gets the required level.
    /// </summary>
    CrmAttributeRequiredLevel RequiredLevel { get; }

    /// <summary>
    /// Gets the attribute type.
    /// </summary>
    CrmAttributeType AttributeType { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    string Title { get; }
  }

  // ReSharper restore UnusedMemberInSuper.Global
  // ReSharper restore UnusedMember.Global
}
