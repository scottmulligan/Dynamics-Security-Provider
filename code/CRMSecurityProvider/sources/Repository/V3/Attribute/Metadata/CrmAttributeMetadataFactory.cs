// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeMetadataFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeMetadataFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm attribute metadata factory.
  /// </summary>
  internal class CrmAttributeMetadataFactory
  {
    /// <summary>
    /// The entity metadata
    /// </summary>
    private readonly EntityMetadata entityMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeMetadataFactory"/> class.
    /// </summary>
    /// <param name="entityMetadata">The entity metadata.</param>
    public CrmAttributeMetadataFactory(EntityMetadata entityMetadata)
    {
      this.entityMetadata = entityMetadata;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeMetadataFactory"/> class.
    /// </summary>
    public CrmAttributeMetadataFactory()
    {
    }

    /// <summary>
    /// Creates the specified attribute metadata.
    /// </summary>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    /// <returns>ICrmAttributeMetadata</returns>
    public ICrmAttributeMetadata Create(AttributeMetadata attributeMetadata)
    {
      if (attributeMetadata.Type == AttributeType.Lookup || attributeMetadata.Type == AttributeType.Customer)
      {
        if (this.entityMetadata == null)
        {
          return null;
        }

        return new CrmLookupAttributeMetadataAdapter(this.entityMetadata, attributeMetadata);
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