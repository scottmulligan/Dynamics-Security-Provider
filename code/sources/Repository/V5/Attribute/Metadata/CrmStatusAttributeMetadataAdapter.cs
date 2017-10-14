namespace CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata
{
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk.Metadata;

  class CrmStatusAttributeMetadataAdapter : CrmOptionsAttributeMetadataAdapter<StatusAttributeMetadata>, ICrmStatusAttributeMetadata
  {
    public CrmStatusAttributeMetadataAdapter(StatusAttributeMetadata attributeMetadata) : base(attributeMetadata)
    {
    }
  }
}