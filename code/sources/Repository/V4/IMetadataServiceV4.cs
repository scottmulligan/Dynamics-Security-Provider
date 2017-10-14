namespace CRMSecurityProvider.Repository.V4
{
  using CRMSecurityProvider.crm4.metadataservice;

  /// <summary>
  /// CRM metadata service interface (API v4).
  /// </summary>
  public interface IMetadataServiceV4
  {
    MetadataServiceResponse Execute(MetadataServiceRequest request);
  }
}
