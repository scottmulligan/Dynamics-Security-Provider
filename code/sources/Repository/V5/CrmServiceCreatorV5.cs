namespace CRMSecurityProvider.Repository.V5
{
  using System;

  using Microsoft.Crm.Sdk.Messages;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Client;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// CRM service creator class (API v5).
  /// </summary>
  public class CrmServiceCreatorV5
  {
    /// <summary>
    /// Creates CRM organization service.
    /// </summary>
    /// <param name="settings">The configuration settings</param>
    /// <returns>The CRM organization service.</returns>
    public IOrganizationService CreateOrganizationService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      const string CreateOrganizationServiceKey = "createOrganizationService";
      ConditionalLog.Info("CreateOrganizationService(settings). Started.", this, TimerAction.Start, CreateOrganizationServiceKey);

      IOrganizationService organizationService = null;
      try
      {
        var serviceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(settings.Url));

        var authenticationCredentials = new AuthenticationCredentials();
        switch (serviceManagement.AuthenticationType)
        {
          case AuthenticationProviderType.ActiveDirectory:
            authenticationCredentials.ClientCredentials.Windows.ClientCredential = CrmHelper.CreateNetworkCredential(settings.User, settings.Password);

            organizationService = new OrganizationServiceProxy(serviceManagement, authenticationCredentials.ClientCredentials);
            break;
          case AuthenticationProviderType.LiveId:
          case AuthenticationProviderType.Federation:
          case AuthenticationProviderType.OnlineFederation:
            authenticationCredentials.ClientCredentials.UserName.UserName = settings.User;
            authenticationCredentials.ClientCredentials.UserName.Password = settings.Password;

            var tokenCredentials = serviceManagement.Authenticate(authenticationCredentials);
            organizationService = new ManagedTokenOrganizationServiceProxy(tokenCredentials.SecurityTokenResponse, serviceManagement, authenticationCredentials.ClientCredentials);
            break;
        }

        organizationService.Execute(new WhoAmIRequest());

        ConditionalLog.Info("CreateOrganizationService(settings). CRM organization service has been created.", this, TimerAction.Tick, CreateOrganizationServiceKey);
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't create CRM organization service.", e, this);
        return null;
      }
      finally
      {
        ConditionalLog.Info("CreateOrganizationService(settings). Finished.", this, TimerAction.Stop, CreateOrganizationServiceKey);
      }

      return organizationService;
    }
  }
}
