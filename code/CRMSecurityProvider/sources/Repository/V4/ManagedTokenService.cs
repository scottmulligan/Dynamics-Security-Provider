namespace CRMSecurityProvider.Repository.V4
{
  using System;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.crm4.discoveryservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Managed token service wrapper class.
  /// </summary>
  public abstract class ManagedTokenService
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedTokenService"/> class.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    protected ManagedTokenService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      this.Settings = settings;
    }

    /// <summary>
    /// Gets or sets configuration settings.
    /// </summary>
    protected ConfigurationSettings Settings { get; private set; }

    /// <summary>
    /// Gets ticket.
    /// </summary>
    /// <returns>The ticket.</returns>
    protected RetrieveCrmTicketResponse GetTicket()
    {
      switch (this.Settings.AuthenticationType)
      {
        case AuthenticationType.AD:
          return this.GetAdTicket();
        case AuthenticationType.Passport:
          return this.GetPassportTicket();
        case AuthenticationType.SPLA:
          return this.GetSplaTicket();
      }

      return null;
    }

    private RetrieveCrmTicketResponse GetSplaTicket()
    {
      var crmDiscoveryService = new CrmDiscoveryService();
      crmDiscoveryService.Url = this.Settings.Url.ToLower().Replace("crmservice.asmx", "SPLA/crmdiscoveryservice.asmx");
      crmDiscoveryService.Credentials = CrmHelper.CreateNetworkCredential(this.Settings.User, this.Settings.Password);

      var crmTicketRequest = new RetrieveCrmTicketRequest();
      crmTicketRequest.OrganizationName = this.Settings.Organization;

      return (RetrieveCrmTicketResponse)crmDiscoveryService.Execute(crmTicketRequest);
    }

    private RetrieveCrmTicketResponse GetAdTicket()
    {
      var crmDiscoveryService = new CrmDiscoveryService();
      crmDiscoveryService.Url = this.Settings.Url.ToLower().Replace("crmservice.asmx", "AD/crmdiscoveryservice.asmx");
      crmDiscoveryService.Credentials = CrmHelper.CreateNetworkCredential(this.Settings.User, this.Settings.Password);

      var crmTicketRequest = new RetrieveCrmTicketRequest();
      crmTicketRequest.OrganizationName = this.Settings.Organization;

      return (RetrieveCrmTicketResponse)crmDiscoveryService.Execute(crmTicketRequest);
    }

    private RetrieveCrmTicketResponse GetPassportTicket()
    {
      var crmDiscoveryService = new CrmDiscoveryService();
      crmDiscoveryService.Url = String.Format("https://dev.{0}/mscrmservices/2007/Passport/crmdiscoveryservice.asmx", this.Settings.Partner);

      var policyResponse = (RetrievePolicyResponse)crmDiscoveryService.Execute(new RetrievePolicyRequest());
      var passportTicket = LiveIdTicketManager.RetrieveTicket(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), this.Settings.Partner, this.Settings.User, this.Settings.Password, policyResponse.Policy, LiveIdTicketManager.LiveIdEnvironment.PROD);

      var crmTicketRequest = new RetrieveCrmTicketRequest();
      crmTicketRequest.OrganizationName = this.Settings.Organization;
      crmTicketRequest.PassportTicket = passportTicket;

      return (RetrieveCrmTicketResponse)crmDiscoveryService.Execute(crmTicketRequest);
    }
  }
}
