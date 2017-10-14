// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLookupAttributeMetadataAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmLookupAttributeMetadataAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata
{
  using System.Linq;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm lookup attribute metadata adapter.
  /// </summary>
  internal class CrmLookupAttributeMetadataAdapter : CrmAttributeMetadataAdapter<AttributeMetadata>, ICrmLookupAttributeMetadata
  {
    /// <summary>
    /// The entity metadata
    /// </summary>
    private readonly EntityMetadata entityMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmLookupAttributeMetadataAdapter"/> class.
    /// </summary>
    /// <param name="entityMetadata">The entity metadata.</param>
    /// <param name="attributeMetadata">The attribute metadata.</param>
    public CrmLookupAttributeMetadataAdapter(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata)
      : base(attributeMetadata)
    {
      this.entityMetadata = entityMetadata;
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
        if (this.entityMetadata == null || this.entityMetadata.ReferencesFrom == null || this.entityMetadata.ReferencesFrom.Length == 0)
        {
          return new string[] { };
        }

        return this.entityMetadata.ReferencesFrom.Where(r => r.ReferencingAttribute == this.Adaptee.Name).Select(r => r.ReferencedEntity).ToArray();
      }
    }
  }
}