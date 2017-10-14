namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Web.Profile;

  using Sitecore;
  using Sitecore.Diagnostics;

  public class CRMProfileProviderPerformanceWrapper : ProfileProvider
  {
    private string realProviderName;
    private ProfileProvider realProvider;

    public override string ApplicationName
    {
      get
      {
        return this.RealProvider.ApplicationName;
      }
      set
      {
        this.RealProvider.ApplicationName = value;
      }
    }

    [NotNull]
    private ProfileProvider RealProvider
    {
      get
      {
        if (this.realProvider == null)
        {
          Assert.IsNotNullOrEmpty(this.realProviderName, "Attempt to access the CRMProfileProviderPerformanceWrapper.RealProvider property with an uninitialized provider name.");
          this.realProvider = ProfileManager.Providers[this.realProviderName];
          Assert.IsNotNull(this.realProvider, "Unknown provider specified in CRMProfileProviderPerformanceWrapper: {0}", this.realProviderName);
        }

        return this.realProvider;
      }
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize(name, config);

      this.realProviderName = config["realProviderName"];
      Assert.IsNotNullOrEmpty(this.realProviderName, "Missing or empty 'realProviderName' attribute in CRMProfileProviderPerformanceWrapper configuration.");
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
    {
      using (CrmProfiler.Measure("Profile.GetPropertyValues"))
      {
        return this.RealProvider.GetPropertyValues(context, collection);
      }
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
      using (CrmProfiler.Measure("Profile.SetPropertyValues"))
      {
        this.RealProvider.SetPropertyValues(context, collection);
      }
    }

    public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      using (CrmProfiler.Measure("Profile.DeleteInactiveProfiles"))
      {
        return this.RealProvider.DeleteInactiveProfiles(authenticationOption, userInactiveSinceDate);
      }
    }

    public override int DeleteProfiles(ProfileInfoCollection profiles)
    {
      using (CrmProfiler.Measure("Profile.DeleteProfiles"))
      {
        return this.RealProvider.DeleteProfiles(profiles);
      }
    }

    public override int DeleteProfiles(string[] usernames)
    {
      using (CrmProfiler.Measure("Profile.DeleteProfiles (by user names)"))
      {
        return this.RealProvider.DeleteProfiles(usernames);
      }
    }

    public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Profile.FindInactiveProfilesByUserName"))
      {
        return this.RealProvider.FindInactiveProfilesByUserName(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
      }
    }

    public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Profile.FindProfilesByUserName"))
      {
        return this.RealProvider.FindProfilesByUserName(authenticationOption, usernameToMatch, pageIndex, pageSize, out totalRecords);
      }
    }

    public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Profile.GetAllInactiveProfiles"))
      {
        return this.RealProvider.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
      }
    }

    public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Profile.GetAllProfiles"))
      {
        return this.RealProvider.GetAllProfiles(authenticationOption, pageIndex, pageSize, out totalRecords);
      }
    }

    public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      using (CrmProfiler.Measure("Profile.GetNumberOfInactiveProfiles"))
      {
        return this.RealProvider.GetNumberOfInactiveProfiles(authenticationOption, userInactiveSinceDate);
      }
    }
  }
}
