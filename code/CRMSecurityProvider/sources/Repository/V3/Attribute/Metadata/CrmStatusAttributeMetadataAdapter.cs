// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmStatusAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmStatusAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm status attribute metadata adapter.
  /// </summary>
  internal class CrmStatusAttributeMetadataAdapter : CrmAttributeMetadataAdapter<StatusAttributeMetadata>, ICrmStatusAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmStatusAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    public CrmStatusAttributeMetadataAdapter(StatusAttributeMetadata attributeMetadata)
      : base(attributeMetadata)
    {
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    public IEnumerable<KeyValuePair<int, string>> Options
    {
      get
      {
        return this.Adaptee.Options.Select(o => new KeyValuePair<int, string>(o.OptionValue, o.Description));
      }
    }
  }
}