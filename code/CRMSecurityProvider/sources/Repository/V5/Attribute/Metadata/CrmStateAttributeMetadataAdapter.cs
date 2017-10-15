// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmStateAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmStateAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  /// <summary>
  /// The crm state attribute metadata adapter.
  /// </summary>
  internal class CrmStateAttributeMetadataAdapter : CrmOptionsAttributeMetadataAdapter<StateAttributeMetadata>, ICrmStateAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmStateAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="attributeMetadata">
    /// The attribute metadata.
    /// </param>
    public CrmStateAttributeMetadataAdapter(StateAttributeMetadata attributeMetadata) : base(attributeMetadata)
    {
    }
  }
}