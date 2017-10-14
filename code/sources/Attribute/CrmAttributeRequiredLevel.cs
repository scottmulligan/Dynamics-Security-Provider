// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeRequiredLevel.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeRequiredLevel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  /// <summary>
  /// The crm attribute required level.
  /// </summary>
  public enum CrmAttributeRequiredLevel
  {
    None,
    SystemRequired,
    ApplicationRequired,
    Recommended,
    ReadOnly,

    Empty,
    Unknown
  }
}