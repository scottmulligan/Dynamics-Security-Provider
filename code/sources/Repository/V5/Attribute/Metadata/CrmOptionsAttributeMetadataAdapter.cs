// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmOptionsAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmOptionsAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  /// <summary>
  /// The crm options attribute metadata adapter.
  /// </summary>
  /// <typeparam name="T">Type of adaptee options attribute metadata.</typeparam>
  internal class CrmOptionsAttributeMetadataAdapter<T> : CrmAttributeMetadataAdapter<T>, ICrmOptionsAttributeMetadata where T : EnumAttributeMetadata
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmOptionsAttributeMetadataAdapter{T}"/> class.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    public CrmOptionsAttributeMetadataAdapter(T attributeMetadata) : base(attributeMetadata)
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
        return this.Adaptee.OptionSet.Options.Select(o => new KeyValuePair<int, string>(o.Value.HasValue ? o.Value.Value : -1, o.Label.UserLocalizedLabel.Label));
      }
    }
  }
}