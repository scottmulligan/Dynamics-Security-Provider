namespace CRMSecurityProvider
{
  using System.Collections.Specialized;
  using System.Web.Security;

  using Sitecore;
  using Sitecore.Diagnostics;

  public class CRMRoleProviderPerformanceWrapper : RoleProvider
  {
    private string realProviderName;
    private RoleProvider realProvider;

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
    private RoleProvider RealProvider
    {
      get
      {
        if (this.realProvider == null)
        {
          Assert.IsNotNullOrEmpty(this.realProviderName, "Attempt to access the CRMRoleProviderPerformanceWrapper.RealProvider property with an uninitialized provider name.");
          this.realProvider = Roles.Providers[this.realProviderName];
          Assert.IsNotNull(this.realProvider, "Unknown provider specified in CRMRoleProviderPerformanceWrapper: {0}", this.realProviderName);
        }

        return this.realProvider;
      }
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize(name, config);

      this.realProviderName = config["realProviderName"];
      Assert.IsNotNullOrEmpty(this.realProviderName, "Missing or empty 'realProviderName' attribute in CRMRoleProviderPerformanceWrapper configuration.");
    }

    public override void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      using (CrmProfiler.Measure("Role.AddUsersToRoles"))
      {
        this.RealProvider.AddUsersToRoles(userNames, roleNames);
      }
    }

    public override void CreateRole(string roleName)
    {
      using (CrmProfiler.Measure("Role.CreateRole"))
      {
        this.RealProvider.CreateRole(roleName);
      }
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      using (CrmProfiler.Measure("Role.DeleteRole"))
      {
        return this.RealProvider.DeleteRole(roleName, throwOnPopulatedRole);
      }
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      using (CrmProfiler.Measure("Role.FindUsersInRole"))
      {
        return this.RealProvider.FindUsersInRole(roleName, usernameToMatch);
      }
    }

    public override string[] GetAllRoles()
    {
      using (CrmProfiler.Measure("Role.GetAllRoles"))
      {
        return this.RealProvider.GetAllRoles();
      }
    }

    public override string[] GetRolesForUser(string username)
    {
      using (CrmProfiler.Measure("Role.GetRolesForUser"))
      {
        return this.RealProvider.GetRolesForUser(username);
      }
    }

    public override string[] GetUsersInRole(string roleName)
    {
      using (CrmProfiler.Measure("Role.GetUsersInRole"))
      {
        return this.RealProvider.GetUsersInRole(roleName);
      }
    }

    public override bool IsUserInRole(string username, string roleName)
    {
      using (CrmProfiler.Measure("Role.IsUserInRole"))
      {
        return this.RealProvider.IsUserInRole(username, roleName);
      }
    }

    public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      using (CrmProfiler.Measure("Role.RemoveUsersFromRoles"))
      {
        this.RealProvider.RemoveUsersFromRoles(userNames, roleNames);
      }
    }

    public override bool RoleExists(string roleName)
    {
      using (CrmProfiler.Measure("Role.RoleExists"))
      {
        return this.RealProvider.RoleExists(roleName);
      }
    }
  }
}
