// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmPicklistAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmPicklistAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm picklist attribute metadata adapter.
  /// </summary>
  internal class CrmPicklistAttributeMetadataAdapter : CrmAttributeMetadataAdapter<PicklistAttributeMetadata>, ICrmPicklistAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmPicklistAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    public CrmPicklistAttributeMetadataAdapter(PicklistAttributeMetadata attributeMetadata)
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