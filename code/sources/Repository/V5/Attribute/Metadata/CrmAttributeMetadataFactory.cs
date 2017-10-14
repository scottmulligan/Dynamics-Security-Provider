// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  /// <summary>
  /// The CRM attribute metadata factory.
  /// </summary>
  internal class CrmAttributeMetadataFactory
  {
    /// <summary>
    /// Creates the specified attribute metadata.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    /// <returns>ICrmAttributeMetadata</returns>
    public ICrmAttributeMetadata Create(AttributeMetadata attributeMetadata)
    {
      if (attributeMetadata is LookupAttributeMetadata)
      {
        return new CrmLookupAttributeMetadataAdapter(attributeMetadata as LookupAttributeMetadata);
      }

      if (attributeMetadata is StateAttributeMetadata)
      {
        return new CrmStateAttributeMetadataAdapter(attributeMetadata as StateAttributeMetadata);
      }

      if (attributeMetadata is StatusAttributeMetadata)
      {
        return new CrmStatusAttributeMetadataAdapter(attributeMetadata as StatusAttributeMetadata);
      }

      if (attributeMetadata is PicklistAttributeMetadata)
      {
        return new CrmPicklistAttributeMetadataAdapter(attributeMetadata as PicklistAttributeMetadata);
      }

      return new CrmAttributeMetadataAdapter<AttributeMetadata>(attributeMetadata);
    }
  }
}
