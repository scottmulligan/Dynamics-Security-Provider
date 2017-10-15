// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLookupAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmLookupAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute.Metadata
{
  using CRMSecurityProvider.crm4.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm lookup attribute metadata adapter.
  /// </summary>
  internal class CrmLookupAttributeMetadataAdapter : CrmAttributeMetadataAdapter<LookupAttributeMetadata>, ICrmLookupAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmLookupAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="lookupAttributeMetadata">The lookup attribute metadata.</param>
    public CrmLookupAttributeMetadataAdapter(LookupAttributeMetadata lookupAttributeMetadata) : base(lookupAttributeMetadata)
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
      get
      {
        return this.Adaptee.Targets;
      }
    }
  }
}