namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Linq;
  using System.Web.Security;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Repository;
  using CRMSecurityProvider.Repository.Factory;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// CRM role provider class.
  /// </summary>
  public class CRMRoleProvider : RoleProvider
  {
    private readonly IRoleRepositoryFactory roleRepositoryFactory;
    private RoleRepositoryBase roleRepository;
    private bool initialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMRoleProvider"/> class.
    /// </summary>
    public CRMRoleProvider()
      : this(new RoleRepositoryFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMRoleProvider"/> class.
    /// </summary>
    /// <param name="roleRepositoryFactory">The role repository factory.</param>
    public CRMRoleProvider(IRoleRepositoryFactory roleRepositoryFactory)
    {
      Assert.ArgumentNotNull(roleRepositoryFactory, "roleRepositoryFactory");

      this.roleRepositoryFactory = roleRepositoryFactory;
    }

    /// <summary>
    /// Gets or sets the name of the application to store and retrieve role information for.
    /// </summary>
    public override string ApplicationName { get; set; }

    /// <summary>
    /// true if the provider is in read-only mode; otherwise, false.
    /// </summary>
    public bool ReadOnly { get; private set; }

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="name">The friendly name of the provider.</param>
    /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
    /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
    /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
    /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize(name, config);

      try
      {
        Error.AssertLicense("Sitecore.MSCRM", "Microsoft Dynamics CRM security provider.");

        this.ApplicationName = config["applicationName"];
        this.ReadOnly = (String.IsNullOrEmpty(config["readOnly"]) || config["readOnly"] == "true");

        var connectionStringName = config["connectionStringName"];
        var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

        this.roleRepository = this.roleRepositoryFactory.GetRepository(Configuration.ConfigurationSettings.Parse(connectionString));
        this.initialized = true;
      }
      catch (Exception e)
      {
        this.initialized = false;
        ConditionalLog.Error("The CRM provider couldn't be initialized.", e, this);
      }
    }

    /// <summary>
    /// Adds the specified user names to the specified roles for the configured applicationName.
    /// </summary>
    /// <param name="userNames">A string array of user names to be added to the specified roles.</param>
    /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
    /// <exception cref="NotSupportedException">Couldn't add users to roles as the CRM provider is in read-only mode.</exception>
    /// <exception cref="ApplicationException">Couldn't add user(s) to role(s) in CRM. Check log file for details.</exception>
    public override void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't add users to roles as the CRM provider is in read-only mode.");
        }

        if (!this.roleRepository.AddUsersToRoles(userNames, roleNames))
        {
          throw new ApplicationException("Couldn't add user(s) to role(s) in CRM. Check log file for details.");
        }
      }
    }

    /// <summary>
    /// Adds a new role to the data source for the configured applicationName.
    /// </summary>
    /// <param name="roleName">The name of the role to create.</param>
    /// <exception cref="NotSupportedException">Couldn't add role as the CRM provider is in read-only mode.</exception>
    /// <exception cref="NotSupportedException">The role with {0} name already exists.</exception>
    /// <exception cref="ApplicationException">Couldn't add role to CRM. Check log file for details.</exception>
    public override void CreateRole(string roleName)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't add role as the CRM provider is in read-only mode.");
        }

        if (this.roleRepository.GetRole(roleName) != null)
        {
          throw new NotSupportedException(String.Format("The role with {0} name already exists.", roleName));
        }

        if (!this.roleRepository.CreateRole(roleName))
        {
          throw new ApplicationException("Couldn't add role to CRM. Check log file for details.");
        }
      }
    }

    /// <summary>
    /// Removes a role from the data source for the configured applicationName.
    /// </summary>
    /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
    /// <param name="roleName">The name of the role to delete.</param>
    /// <param name="throwOnPopulatedRole">If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.</param>
    /// <exception cref="NotSupportedException">Couldn't delete role as the CRM provider is in read-only mode.</exception>
    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't delete role as the CRM provider is in read-only mode.");
        }

        return this.roleRepository.DeactivateRole(roleName);
      }

      return false;
    }

    /// <summary>
    /// Gets an array of user names in a role where the user name contains the specified user name to match.
    /// </summary>
    /// <returns>A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch"/> and the user is a member of the specified role.</returns>
    /// <param name="roleName">The role to search in.</param>
    /// <param name="usernameToMatch">The user name to search for.</param>
    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      if (this.initialized)
      {
        Assert.ArgumentNotNull(usernameToMatch, "usernameToMatch");

        return this.roleRepository.GetUsersInRole(roleName).Where(userName => userName.Contains(usernameToMatch.Trim('%'))).ToArray();
      }

      return new string[0];
    }

    /// <summary>
    /// Gets a list of all the roles for the configured applicationName.
    /// </summary>
    /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
    public override string[] GetAllRoles()
    {
      if (this.initialized)
      {
        return this.roleRepository.GetAllRoles().Select(role => role.Name).ToArray();
      }

      return new string[0];
    }

    /// <summary>
    /// Gets a list of the roles that a specified user is in for the configured applicationName.
    /// </summary>
    /// <returns> A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
    /// <param name="userName">The user to return a list of roles for.</param>
    public override string[] GetRolesForUser(string userName)
    {
      if (this.initialized)
      {
        return this.roleRepository.GetRolesForUser(userName);
      }

      return new string[0];
    }

    /// <summary>
    /// Gets a list of contacts in the specified marketing list.
    /// </summary>
    /// <returns>A string array containing the names of all the contacts who are members of the specified marketing list.</returns>
    /// <param name="roleName">The name of the marketing list to get the list of contacts for.</param>
    public override string[] GetUsersInRole(string roleName)
    {
      if (this.initialized)
      {
        return this.roleRepository.GetUsersInRole(roleName);
      }

      return new string[0];
    }

    /// <summary>
    /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
    /// </summary>
    /// <returns>true if the specified user is in the specified role for the configured applicationName; otherwise, false.</returns>
    /// <param name="username">The user name to search for.</param>
    /// <param name="roleName">The role to search in.</param>
    public override bool IsUserInRole(string username, string roleName)
    {
      if (this.initialized)
      {
        return this.roleRepository.IsUserInRole(username, roleName);
      }

      return false;
    }

    /// <summary>
    /// Removes the specified user names from the specified roles for the configured applicationName.
    /// </summary>
    /// <param name="userNames">A string array of user names to be removed from the specified roles.</param>
    /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
    /// <exception cref="NotSupportedException">Couldn't remove users from roles as the CRM provider is in read-only mode.</exception>
    /// <exception cref="ApplicationException">Couldn't remove user(s) from role(s) in CRM. Check log file for details.</exception>
    public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't remove users from roles as the CRM provider is in read-only mode.");
        }

        if (!this.roleRepository.RemoveUsersFromRoles(userNames, roleNames))
        {
          throw new ApplicationException("Couldn't remove user(s) from role(s) in CRM. Check log file for details.");
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
    /// </summary>
    /// <returns>true if the role name already exists in the data source for the configured applicationName; otherwise, false.</returns>
    /// <param name="roleName">The name of the role to search for in the data source.</param>
    public override bool RoleExists(string roleName)
    {
      if (this.initialized)
      {
        return this.roleRepository.GetRole(roleName) != null;
      }

      return false;
    }
  }
}