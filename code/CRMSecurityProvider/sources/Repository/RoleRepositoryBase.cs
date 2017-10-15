// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleRepositoryBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Role repository base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Configuration.Provider;
  using System.Linq;
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Utils;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Role repository base class.
  /// </summary>
  public abstract class RoleRepositoryBase : RepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRepositoryBase"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    protected RoleRepositoryBase(UserRepositoryBase userRepository, ICacheService cacheService)
      : base(cacheService)
    {
      Assert.ArgumentNotNull(userRepository, "userRepository");

      this.UserRepository = userRepository;
    }

    /// <summary>
    /// Gets or sets the user repository.
    /// </summary>
    protected UserRepositoryBase UserRepository { get; private set; }

    /// <summary>
    /// Creates the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns><c>true</c> if the role has been created; otherwise, <c>false</c>.</returns>
    public abstract bool CreateRole(string roleName);

    /// <summary>
    /// Deactivates the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns><c>true</c> if the role has been deactivated; otherwise, <c>false</c>.</returns>
    public abstract bool DeactivateRole(string roleName);

    /// <summary>
    /// Determines whether the user is in the role.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="roleName">Name of the role.</param>
    /// <returns><c>true</c> if the user is in the role; otherwise, <c>false</c>.</returns>
    public abstract bool IsUserInRole(string userName, string roleName);

    /// <summary>
    /// Adds the users to the roles.
    /// </summary>
    /// <param name="userNames">The user names.</param>
    /// <param name="roleNames">The role names.</param>
    /// <exception cref="ArgumentException">List of users to add to roles is empty.</exception>
    /// <exception cref="ArgumentException">List of roles to add users to is empty.</exception>
    /// <exception cref="ArgumentException">The list of user names contains empty entries.</exception>
    /// <exception cref="ArgumentException">The list of role names contains empty entries.</exception>
    /// <exception cref="ProviderException">One or more contacts couldn't be found.</exception>
    /// <exception cref="ProviderException">One or more marketing lists couldn't be found.</exception>
    public virtual bool AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      Assert.ArgumentNotNull(userNames, "userNames");
      Assert.ArgumentNotNull(roleNames, "roleNames");

      if (userNames.Length == 0)
      {
        throw new ArgumentException("List of users to add to roles is empty.");
      }
      if (roleNames.Length == 0)
      {
        throw new ArgumentException("List of roles to add users to is empty.");
      }

      if (userNames.Any(string.IsNullOrEmpty))
      {
        throw new ArgumentException("The list of user names contains empty entries.");
      }

      if (roleNames.Any(string.IsNullOrEmpty))
      {
        throw new ArgumentException("The list of role names contains empty entries.");
      }

      var users = this.UserRepository.GetUsers(userNames);
      if (users.Count != userNames.Length)
      {
        throw new ProviderException("One or more contacts couldn't be found.");
      }

      var roles = this.GetRoles(roleNames);
      if (roles.Count != roleNames.Length)
      {
        throw new ProviderException("One or more marketing lists couldn't be found.");
      }

      return this.AddUsersToRoles(users, roles);
    }

    /// <summary>
    /// Adds the users to the roles.
    /// </summary>
    /// <param name="users">The users.</param>
    /// <param name="roles">The roles.</param>
    protected abstract bool AddUsersToRoles(List<CRMUser> users, List<CRMRole> roles);

    /// <summary>
    /// Removes the users from the roles.
    /// </summary>
    /// <param name="userNames">The user names.</param>
    /// <param name="roleNames">The role names.</param>
    /// <exception cref="ArgumentException">List of users to remove from roles is empty.</exception>
    /// <exception cref="ArgumentException">List of roles to remove users from is empty.</exception>
    /// <exception cref="ArgumentException">The list of user names contains empty entries.</exception>
    /// <exception cref="ArgumentException">The list of role names contains empty entries.</exception>
    /// <exception cref="ProviderException">One or more contacts couldn't be found.</exception>
    /// <exception cref="ProviderException">One or more marketing lists couldn't be found.</exception>
    /// <exception cref="ProviderException">User isn't a member of role.</exception>
    public virtual bool RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      Assert.ArgumentNotNull(userNames, "userNames");
      Assert.ArgumentNotNull(roleNames, "roleNames");

      if (userNames.Length == 0)
      {
        throw new ArgumentException("List of users to remove from roles is empty.");
      }

      if (roleNames.Length == 0)
      {
        throw new ArgumentException("List of roles to remove users from is empty.");
      }

      if (userNames.Any(string.IsNullOrEmpty))
      {
        throw new ArgumentException("The list of user names contains empty entries.");
      }

      if (roleNames.Any(string.IsNullOrEmpty))
      {
        throw new ArgumentException("The list of role names contains empty entries.");
      }

      var users = this.UserRepository.GetUsers(userNames);
      if (users.Count != userNames.Length)
      {
        throw new ProviderException("One or more contacts couldn't be found.");
      }

      var roles = this.GetRoles(roleNames);
      if (roles.Count != roleNames.Length)
      {
        throw new ProviderException("One or more marketing lists couldn't be found.");
      }

      if (roleNames.Any(roleName => userNames.Any(username => !this.IsUserInRole(username, roleName))))
      {
        throw new ProviderException("User isn't a member of role.");
      }

      return this.RemoveUsersFromRoles(users, roles);
    }

    /// <summary>
    /// Removes the users from the roles.
    /// </summary>
    /// <param name="users">The users.</param>
    /// <param name="roles">The roles.</param>
    protected abstract bool RemoveUsersFromRoles(List<CRMUser> users, List<CRMRole> roles);

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>The roles.</returns>
    public virtual List<CRMRole> GetAllRoles()
    {
      const string GetAllRolesKey = "getAllRoles";
      ConditionalLog.Info("GetAllRoles(). Started.", this, TimerAction.Start, GetAllRolesKey);

      var roles = this.GetAllRolesFromCrm();
      ConditionalLog.Info(string.Format("GetAllRoles(). Retrieved {0} role(s) from CRM.", roles.Count), this, TimerAction.Tick, GetAllRolesKey);

      var result = new Dictionary<string, CRMRole>();
      foreach (var role in roles)
      {
        if (!result.ContainsKey(role.Name))
        {
          result.Add(role.Name, role);
          this.CacheService.RoleCache.Add(role);
        }
      }

      ConditionalLog.Info("GetAllRoles(). Finished.", this, TimerAction.Stop, GetAllRolesKey);
      return result.Values.ToList();
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>The roles.</returns>
    protected abstract List<CRMRole> GetAllRolesFromCrm();

    /// <summary>
    /// Gets the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns>The role.</returns>
    public virtual CRMRole GetRole(string roleName)
    {
      Assert.ArgumentNotNull(roleName, "roleName");

      const string GetRoleKey = "getRole";
      ConditionalLog.Info(string.Format("GetRole({0}). Started.", roleName), this, TimerAction.Start, GetRoleKey);

      var role = (CRMRole)this.CacheService.RoleCache.Get(roleName);
      if (role != null)
      {
        ConditionalLog.Info(string.Format("GetRole({0}). Finished (retrieved role from cache).", roleName), this, TimerAction.Stop, GetRoleKey);
        return role;
      }

      role = this.GetRoleFromCrm(roleName);
      if (role != null)
      {
        ConditionalLog.Info(string.Format("GetRole({0}). Retrieved role from CRM.", roleName), this, TimerAction.Tick, GetRoleKey);
        this.CacheService.RoleCache.Add(role);
      }

      ConditionalLog.Info(string.Format("GetRole({0}). Finished.", roleName), this, TimerAction.Stop, GetRoleKey);
      return role;
    }

    /// <summary>
    /// Gets the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns>The role.</returns>
    protected abstract CRMRole GetRoleFromCrm(string roleName);

    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <param name="roleNames">The role names.</param>
    /// <returns>The roles.</returns>
    public virtual List<CRMRole> GetRoles(string[] roleNames)
    {
      Assert.ArgumentNotNull(roleNames, "roleNames");

      const string GetRolesKey = "getRoles";
      ConditionalLog.Info(string.Format("GetRoles({0}). Started.", string.Join(",", roleNames)), this, TimerAction.Start, GetRolesKey);

      var result = new Dictionary<string, CRMRole>();

      var roles = roleNames.Select(n => (CRMRole)this.CacheService.RoleCache.Get(n)).Where(r => r != null).ToList();
      foreach (var role in roles)
      {
        ConditionalLog.Info(string.Format("GetRoles({0}). Retrieved role {1} from cache.", string.Join(",", roleNames), role.Name), this, TimerAction.Tick, GetRolesKey);
        result.Add(role.Name, role);
      }

      if (result.Count != roleNames.Length)
      {
        roles = this.GetRolesFromCrm(roleNames.Except(result.Keys).ToArray());
        ConditionalLog.Info(string.Format("GetRoles({0}). Retrieved {1} role(s) from CRM.", string.Join(",", roleNames), roles.Count), this, TimerAction.Tick, GetRolesKey);

        foreach (var role in roles)
        {
          if (!result.ContainsKey(role.Name))
          {
            result.Add(role.Name, role);
            this.CacheService.RoleCache.Add(role);
          }
        }
      }

      ConditionalLog.Info(string.Format("GetRoles({0}). Finished.", string.Join(",", roleNames)), this, TimerAction.Stop, GetRolesKey);
      return result.Values.ToList();
    }

    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <param name="roleNames">The role names.</param>
    /// <returns>The roles.</returns>
    protected abstract List<CRMRole> GetRolesFromCrm(string[] roleNames);

    /// <summary>
    /// Gets the roles for the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>The roles for the user.</returns>
    public abstract string[] GetRolesForUser(string userName);

    /// <summary>
    /// Gets the users in the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns>The users in the role.</returns>
    public abstract string[] GetUsersInRole(string roleName);
    private string[] attributes;
    public string[] Attributes
    {
        get
        {
            var standardAttributes = new string[] { "listid", "listname", "type" };
            if (this.attributes != null)
            {
                standardAttributes = standardAttributes.Union(this.attributes).ToArray();
            }
            return standardAttributes;
        }
        set
        {
            this.attributes = value;
        }
    }
  }
}
