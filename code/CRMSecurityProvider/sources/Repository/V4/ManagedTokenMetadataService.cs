namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Globalization;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.crm4.metadataservice;

  /// <summary>
  /// Managed token CRM metadata service wrapper class.
  /// </summary>
  public class ManagedTokenMetadataService : ManagedTokenService, IMetadataServiceV4
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedTokenMetadataService"/> class.
    /// </summary>
    /// <param name="metadataService">The CRM metadata service.</param>
    /// <param name="settings">The configuration settings.</param>
    public ManagedTokenMetadataService(MetadataService metadataService, ConfigurationSettings settings)
      : base(settings)
    {
      Assert.ArgumentNotNull(metadataService, "metadataService");

      this.MetadataService = metadataService;
    }

    /// <summary>
    /// Gets or sets CRM metadata service.
    /// </summary>
    protected MetadataService MetadataService { get; private set; }

    /// <summary>
    /// Gets or sets expiration date.
    /// </summary>
    protected DateTime ExpirationDate { get; private set; }

    public MetadataServiceResponse Execute(MetadataServiceRequest request)
    {
      this.Validate();
      return this.MetadataService.Execute(request);
    }

    private void Validate()
    {
      if (DateTime.UtcNow.AddMinutes(5) >= this.ExpirationDate)
      {
        var ticket = this.GetTicket();

        this.MetadataService.CrmAuthenticationTokenValue.CrmTicket = ticket.CrmTicket;
        this.ExpirationDate = DateTime.Parse(ticket.ExpirationDate, null, DateTimeStyles.AdjustToUniversal);
      }
    }
  }
}
