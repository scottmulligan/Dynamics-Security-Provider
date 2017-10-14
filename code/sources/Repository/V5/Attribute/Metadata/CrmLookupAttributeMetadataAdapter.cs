// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLookupAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmLookupAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  /// <summary>
  /// The CRM lookup attribute metadata adapter.
  /// </summary>
  internal class CrmLookupAttributeMetadataAdapter : CrmAttributeMetadataAdapter<LookupAttributeMetadata>, ICrmLookupAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmLookupAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="lookupAttributeMetadata">The lookup attribute metadata.</param>
    public CrmLookupAttributeMetadataAdapter(LookupAttributeMetadata lookupAttributeMetadata)
      : base(lookupAttributeMetadata)
    {
    }

    /// <summary>
    /// Gets the targets.
    /// </summary>
    /// <value>
    /// The targets.
    /// </value>
    public string[] Targets
    {
      get { return this.Adaptee.Targets; }
    }
  }
}