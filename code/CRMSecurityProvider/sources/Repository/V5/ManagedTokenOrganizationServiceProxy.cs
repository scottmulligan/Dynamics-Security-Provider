namespace CRMSecurityProvider.Repository.V5
{
    using System;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using System.ServiceModel.Description;

    /// <summary>
    /// Managed token OrganizationServiceProxy wrapper class.
    /// </summary>
    public class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedTokenOrganizationServiceProxy"/> class.
    /// </summary>
    /// <param name="serviceManagement">The service management.</param>
    /// <param name="securityTokenResponse">The security token response.</param>
    public ManagedTokenOrganizationServiceProxy(SecurityTokenResponse securityTokenResponse, IServiceManagement<IOrganizationService> serviceManagement, ClientCredentials clientCredentials)
      : base(serviceManagement, clientCredentials)
    {
            base.SecurityTokenResponse = securityTokenResponse;
    }

    /// <summary>
    /// Authenticates the client and creates a new service channel.
    /// </summary>
    protected override void ValidateAuthentication()
    {
      if (DateTime.UtcNow.AddMinutes(5) >= this.SecurityTokenResponse.Response.Lifetime.Expires)
      {
        this.Authenticate();
      }

      base.ValidateAuthentication();
    }
  }
}
