// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmStateAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmStateAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm state attribute metadata adapter.
  /// </summary>
  internal class CrmStateAttributeMetadataAdapter : CrmAttributeMetadataAdapter<StateAttributeMetadata>, ICrmStateAttributeMetadata
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

    /// <summary>
    /// Gets the options.
    /// </summary>
    public IEnumerable<KeyValuePair<int, string>> Options
    {
      get
      {
        return this.Adaptee.Options.Select(o => new KeyValuePair<int, string>(o.OptionValue, o.Description));
      }
    }
  }
}