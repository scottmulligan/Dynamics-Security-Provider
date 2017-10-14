namespace CRMSecurityProvider.Sources.Repository.V5
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.ServiceModel;
  using System.Threading;
  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Repository.V5;
  using CRMSecurityProvider.Utils;
  using Microsoft.Crm.Sdk.Messages;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Client;
  using Sitecore.Diagnostics;

  public class OrganizationServiceCacheV5
  {

    private const int CacheExpiredMin = 5;

    public OrganizationServiceCacheV5(CrmServiceCreatorV5 crmServiceCreator, ConfigurationSettings settings)
    {
      Assert.ArgumentNotNull(crmServiceCreator, "crmServiceCreator");
      Assert.ArgumentNotNull(settings, "settings");
      this.crmServiceCreator = crmServiceCreator;
      this.settings = settings;
    }

    private readonly CrmServiceCreatorV5 crmServiceCreator;
    private readonly ConfigurationSettings settings;

    private readonly object locker = new object();
    private IOrganizationService cachedService;

    public IOrganizationService GetOrganizationService()
    {
      var service = this.GetOrganizationServiceFromCache();
      try
      {
        service.Execute(new WhoAmIRequest());
      }
      catch (CommunicationException)
      {
        lock (locker)
        {
          cachedService = null;
        }
        service = this.GetOrganizationServiceFromCache();
      }
      return service;
    }

    private DateTime cacheExpiredTime = DateTime.MinValue;

    private IOrganizationService GetOrganizationServiceFromCache()
    {
      IOrganizationService service;
      lock (this.locker)
      {
        if (this.cachedService == null || DateTime.Now > cacheExpiredTime)
        {
          this.cachedService = this.crmServiceCreator.CreateOrganizationService(this.settings);
        }
        cacheExpiredTime = DateTime.Now.Add(new TimeSpan(0, 0, CacheExpiredMin, 0));
        service = this.cachedService;
      }
      return service;
    }

    private IOrganizationService GetOrganizationServiceAlwaysNew()
    {
      return crmServiceCreator.CreateOrganizationService(settings);
    }

  }
}
