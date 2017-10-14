namespace CRMSecurityProvider.Repository.V3
{
  using System;
  using System.Net;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.crm3.webservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// CRM service creator class (API v3).
  /// </summary>
  public class CrmServiceCreatorV3
  {
    /// <summary>
    /// Creates CRM service.
    /// </summary>
    /// <param name="settings">The configuration settings</param>
    /// <returns>The CRM service.</returns>
    public ICrmServiceV3 CreateService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      const string CreateServiceKey = "createService";
      ConditionalLog.Info("CreateService(settings). Started.", this, TimerAction.Start, CreateServiceKey);

      CrmService crmService = null;
      try
      {
        crmService = new CrmService();
        crmService.Url = settings.Url;
        crmService.Timeout = 360 * 1000;
        crmService.Credentials = CrmHelper.CreateNetworkCredential(settings.User, settings.Password);
        crmService.UnsafeAuthenticatedConnectionSharing = settings.UnsafeAuthenticatedConnectionSharing;
        crmService.PreAuthenticate = settings.PreAuthenticate;
        crmService.ConnectionGroupName = CrmHelper.GetConnectionGroupName((NetworkCredential)crmService.Credentials);

        crmService.Execute(new WhoAmIRequest());

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

      return crmService;
    }

    /// <summary>
    /// Creates CRM metadata service.
    /// </summary>
    /// <param name="settings">The configuration settings</param>
    /// <returns>The CRM metadata service.</returns>
    public IMetadataServiceV3 CreateMetadataService(ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(settings, "settings");

      const string CreateMetadataServiceKey = "createMetadataService";
      ConditionalLog.Info("CreateMetadataService(settings). Started.", this, TimerAction.Start, CreateMetadataServiceKey);

      MetadataService metadataService = null;
      try
      {
        metadataService = new MetadataService();
        metadataService.Url = settings.Url.Replace("crmservice.asmx", "metadataservice.asmx");
        metadataService.Timeout = 360 * 1000;
        metadataService.Credentials = CrmHelper.CreateNetworkCredential(settings.User, settings.Password);
        metadataService.UnsafeAuthenticatedConnectionSharing = settings.UnsafeAuthenticatedConnectionSharing;
        metadataService.PreAuthenticate = settings.PreAuthenticate;
        metadataService.ConnectionGroupName = CrmHelper.GetConnectionGroupName((NetworkCredential)metadataService.Credentials);

        metadataService.GetTimestamp();

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

      return metadataService;
    }
  }
}
