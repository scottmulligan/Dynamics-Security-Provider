namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  class CrmPicklistAttributeMetadataAdapter : CrmOptionsAttributeMetadataAdapter<PicklistAttributeMetadata>, ICrmPicklistAttributeMetadata
  {
    public CrmPicklistAttributeMetadataAdapter(PicklistAttributeMetadata attributeMetadata)
      : base(attributeMetadata)
    {
    }
  }
}