namespace CRMSecurityProvider.Repository.V3
{
  using CRMSecurityProvider.crm3.metadataservice;

  /// <summary>
  /// CRM metadata service interface (API v3).
  /// </summary>
  public interface IMetadataServiceV3
  {
    Metadata RetrieveMetadata(MetadataFlags flags);

    EntityMetadata RetrieveEntityMetadata(string entityName, EntityFlags flags);

    AttributeMetadata RetrieveAttributeMetadata(string entityName, string attributeName);

    string GetTimestamp();
  }
}
