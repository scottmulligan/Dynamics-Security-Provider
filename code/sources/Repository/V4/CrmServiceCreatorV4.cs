namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Net;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.crm4.metadataservice;
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// CRM service creator class (API v4).
  /// </summary>
  public class CrmServiceCreatorV4
  {
    /// <summary>
    /// Creates CRM service.
    /// </summary>
    /// <param name="settings">The configuration settings</param>
    /// <returns>The CRM service.</returns>
    public ICrmServiceV4 CreateService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      const string CreateServiceKey = "createService";
      ConditionalLog.Info("CreateService(settings). Started.", this, TimerAction.Start, CreateServiceKey);

      ManagedTokenCrmService wrapper = null;
      try
      {
        var crmService = new CrmService();
        crmService.Url = settings.Url;
        crmService.Timeout = 360 * 1000;
        crmService.Credentials = CrmHelper.CreateNetworkCredential(settings.User, settings.Password);
        crmService.UnsafeAuthenticatedConnectionSharing = settings.UnsafeAuthenticatedConnectionSharing;
        crmService.PreAuthenticate = settings.PreAuthenticate;
        crmService.ConnectionGroupName = CrmHelper.GetConnectionGroupName((NetworkCredential)crmService.Credentials);
        crmService.CrmAuthenticationTokenValue = new crm4.webservice.CrmAuthenticationToken
        {
          AuthenticationType = (int)settings.AuthenticationType,
          OrganizationName = settings.Organization
        };

        wrapper = new ManagedTokenCrmService(crmService, settings);
        wrapper.Execute(new WhoAmIRequest());

        ConditionalLog.Info("CreateService(settings). CRM service has been created.", this, TimerAction.Tick, CreateServiceKey);
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't create CRM service.", e, this);
        return null;
      }
      finally
      {
        ConditionalLog.Info("CreateService(settings). Finished.", this, TimerAction.Stop, CreateServiceKey);
      }

      return wrapper;
    }

    /// <summary>
    /// Creates CRM metadata service.
    /// </summary>
    /// <param name="settings">The configuration settings</param>
    /// <returns>The CRM metadata service.</returns>
    public IMetadataServiceV4 CreateMetadataService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      const string CreateMetadataServiceKey = "createMetadataService";
      ConditionalLog.Info("CreateMetadataService(settings). Started.", this, TimerAction.Start, CreateMetadataServiceKey);

      ManagedTokenMetadataService wrapper = null;
      try
      {
        var metadataService = new MetadataService();
        metadataService.Url = settings.Url.Replace("crmservice.asmx", "metadataservice.asmx");
        metadataService.Timeout = 360 * 1000;
        metadataService.Credentials = CrmHelper.CreateNetworkCredential(settings.User, settings.Password);
        metadataService.UnsafeAuthenticatedConnectionSharing = settings.UnsafeAuthenticatedConnectionSharing;
        metadataService.PreAuthenticate = settings.PreAuthenticate;
        metadataService.ConnectionGroupName = CrmHelper.GetConnectionGroupName((NetworkCredential)metadataService.Credentials);
        metadataService.CrmAuthenticationTokenValue = new crm4.metadataservice.CrmAuthenticationToken
        {
          AuthenticationType = (int)settings.AuthenticationType,
          OrganizationName = settings.Organization
        };

        wrapper = new ManagedTokenMetadataService(metadataService, settings);
        wrapper.Execute(new RetrieveTimestampRequest());

        ConditionalLog.Info("CreateMetadataService(settings). CRM metadata service has been created.", this, TimerAction.Tick, CreateMetadataServiceKey);
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't create CRM metadata service.", e, this);
        return null;
      }
      finally
      {
        ConditionalLog.Info("CreateMetadataService(settings). Finished.", this, TimerAction.Stop, CreateMetadataServiceKey);
      }

      return wrapper;
    }
  }
}
