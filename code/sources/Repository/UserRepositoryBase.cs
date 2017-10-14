namespace CRMSecurityProvider.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// User repository base class.
  /// </summary>
  public abstract class UserRepositoryBase : RepositoryBase
  {
    private string[] attributes;
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepositoryBase"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    protected UserRepositoryBase(ICacheService cacheService)
      : base(cacheService)
    {
    }

    /// <summary>
    /// Creates the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="email">Email of the user.</param>
    /// <param name="providerUserKey">The provider user key.</param>
    /// <returns>The user.</returns>
    public abstract CRMUser CreateUser(string userName, string email, Guid providerUserKey);

    /// <summary>
    /// Deactivates the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns><c>true</c> if the user has been deactivated; otherwise, <c>false</c>.</returns>
    public abstract bool DeactivateUser(string userName);

    /// <summary>
    /// Finds the users.
    /// </summary>
    /// <param name="userEmail">The user email.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <returns>The users.</returns>
    public virtual List<CRMUser> FindUsersByEmail(string userEmail, int pageIndex, int pageSize, out int totalRecords)
    {
      Assert.ArgumentNotNull(userEmail, "userEmail");

      const string FindUsersByEmailKey = "findUsersByEmail";
      ConditionalLog.Info(String.Format("FindUsersByEmail({0}, {1}, {2}). Started.", userEmail, pageIndex, pageSize), this, TimerAction.Start, FindUsersByEmailKey);

      var users = this.FindUsersInCrmByEmail(userEmail, pageIndex, pageSize, out totalRecords);
      ConditionalLog.Info(String.Format("FindUsersByEmail({0}, {1}, {2}). Retrieved {3} user(s) from CRM.", userEmail, pageIndex, pageSize, users.Count), this, TimerAction.Tick, FindUsersByEmailKey);

      var result = new Dictionary<string, CRMUser>();
      foreach (var user in users)
      {
        if (!result.ContainsKey(user.Name))
        {
          result.Add(user.Name, user);
          this.CacheService.UserCache.Add(user);
        }
      }

      ConditionalLog.Info(String.Format("FindUsersByEmail({0}, {1}, {2}). Finished.", userEmail, pageIndex, pageSize), this, TimerAction.Stop, FindUsersByEmailKey);
      return result.Values.ToList();
    }

    /// <summary>
    /// Finds the users.
    /// </summary>
    /// <param name="userEmail">The user email.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <returns>
    /// The users.
    /// </returns>
    protected abstract List<CRMUser> FindUsersInCrmByEmail(string userEmail, int pageIndex, int pageSize, out int totalRecords);

    /// <summary>
    /// Finds the users.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <returns>The users.</returns>
    public virtual List<CRMUser> FindUsersByName(string userName, int pageIndex, int pageSize, out int totalRecords)
    {
      Assert.ArgumentNotNull(userName, "userName");

      const string FindUsersByNameKey = "findUsersByName";
      ConditionalLog.Info(String.Format("FindUsersByName({0}, {1}, {2}). Started.", userName, pageIndex, pageSize), this, TimerAction.Start, FindUsersByNameKey);

      var users = this.FindUsersInCrmByName(userName, pageIndex, pageSize, out totalRecords);
      ConditionalLog.Info(String.Format("FindUsersByName({0}, {1}, {2}). Retrieved {3} user(s) from CRM.", userName, pageIndex, pageSize, users.Count), this, TimerAction.Tick, FindUsersByNameKey);

      var result = new Dictionary<string, CRMUser>();
      foreach (var user in users)
      {
        if (!result.ContainsKey(user.Name))
        {
          result.Add(user.Name, user);
          this.CacheService.UserCache.Add(user);
        }
      }

      ConditionalLog.Info(String.Format("FindUsersByName({0}, {1}, {2}). Finished.", userName, pageIndex, pageSize), this, TimerAction.Stop, FindUsersByNameKey);
      return result.Values.ToList();
    }

    /// <summary>
    /// Finds the users.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <returns>
    /// The users.
    /// </returns>
    protected abstract List<CRMUser> FindUsersInCrmByName(string userName, int pageIndex, int pageSize, out int totalRecords);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <returns>The users.</returns>
    public virtual List<CRMUser> GetAllUsers(Microsoft.Xrm.Sdk.Query.PagingInfo pagingInfo, out int totalRecords)
    {
      const string GetAllUsersKey = "getAllUsers";
      ConditionalLog.Info(String.Format("GetAllUsers({0}, {1}). Started.", pagingInfo.PageNumber, pagingInfo.Count), this, TimerAction.Start, GetAllUsersKey);

      var users = this.GetAllUsersFromCrm(pagingInfo, out totalRecords);
      if (totalRecords == -1)
      {
        ConditionalLog.Info("GetAllUsers returned more than 5k users. A precise count cannot be determined.", this);
        totalRecords = 5000;
      }
      ConditionalLog.Info(String.Format("GetAllUsers({0}, {1}). Retrieved {2} user(s) from CRM.", pagingInfo.PageNumber, pagingInfo.Count, users.Count), this, TimerAction.Tick, GetAllUsersKey);

      var result = new Dictionary<string, CRMUser>();
      foreach (var user in users)
      {
        if (!result.ContainsKey(user.Name))
        {
          result.Add(user.Name, user);
          this.CacheService.UserCache.Add(user);
        }
      }
      
      ConditionalLog.Info(String.Format("GetAllUsers({0}, {1}). Finished.", pagingInfo.PageNumber, pagingInfo.Count), this, TimerAction.Stop, GetAllUsersKey);
      return result.Values.ToList();
    }

    protected abstract List<CRMUser> GetAllUsersFromCrm(Microsoft.Xrm.Sdk.Query.PagingInfo pagingInfo, out int totalRecords);

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>The user.</returns>
    public virtual CRMUser GetUser(string userName, string[] attributes = null)
    {
      Assert.ArgumentNotNull(userName, "userName");
      this.Attributes = attributes;

      const string GetUserKey = "getUserByName";
      ConditionalLog.Info(String.Format("GetUser({0}). Started.", userName), this, TimerAction.Start, GetUserKey);

      var user = (CRMUser)this.CacheService.UserCache.Get(userName);
      if (user != null)
      {
        if (attributes == null || !user.ProfilePropertyNames.Except(attributes.AsEnumerable()).Any())        
        {
          ConditionalLog.Info(String.Format("GetUser({0}). Finished (retrieved user from cache).", userName), this, TimerAction.Stop, GetUserKey);
          return user;
        }
        else
        {
          this.CacheService.UserCache.Remove(user.Name);
        }
      }

      user = this.GetUserFromCrm(userName);
      if (user != null)
      {
        ConditionalLog.Info(String.Format("GetUser({0}). Retrieved user from CRM.", userName), this, TimerAction.Tick, GetUserKey);
        this.CacheService.UserCache.Add(user);
      }

      ConditionalLog.Info(String.Format("GetUser({0}). Finished.", userName), this, TimerAction.Stop, GetUserKey);
      return user;
    }

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>The user.</returns>
    protected abstract CRMUser GetUserFromCrm(string userName);

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <returns>The user.</returns>
    public virtual CRMUser GetUser(Guid userId)
    {
      this.Attributes = null;
      const string GetUserKey = "getUserById";
      ConditionalLog.Info(String.Format("GetUser({0}). Started.", userId), this, TimerAction.Start, GetUserKey);

      var user = (CRMUser)this.CacheService.UserCache.Get(userId);
      if (user != null)
      {
        ConditionalLog.Info(String.Format("GetUser({0}). Finished (retrieved user from cache).", userId), this, TimerAction.Stop, GetUserKey);
        return user;
      }          
      
      user = this.GetUserFromCrm(userId);
      if (user != null)
      {
        ConditionalLog.Info(String.Format("GetUser({0}). Retrieved user from CRM.", userId), this, TimerAction.Tick, GetUserKey);
        this.CacheService.UserCache.Add(user);
      }

      ConditionalLog.Info(String.Format("GetUser({0}). Finished.", userId), this, TimerAction.Stop, GetUserKey);
      return user;
    }

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <returns>The user.</returns>
    protected abstract CRMUser GetUserFromCrm(Guid userId);

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <param name="userNames">The user names.</param>
    /// <returns>The users.</returns>
    public virtual List<CRMUser> GetUsers(string[] userNames)
    {
      Assert.ArgumentNotNull(userNames, "userNames");
      this.Attributes = null;
      const string GetUsersKey = "getUsers";
      ConditionalLog.Info(String.Format("GetUsers({0}). Started.", String.Join(",", userNames)), this, TimerAction.Start, GetUsersKey);

      var result = new Dictionary<string, CRMUser>();

      var users = userNames.Select(n => (CRMUser)this.CacheService.UserCache.Get(n)).Where(u => u != null).ToList();
      foreach (var user in users)
      {
        ConditionalLog.Info(String.Format("GetUsers({0}). Retrieved user {1} from cache.", String.Join(",", userNames), user.Name), this, TimerAction.Tick, GetUsersKey);
        result.Add(user.Name, user);
      }

      if (result.Count != userNames.Length)
      {
        users = this.GetUsersFromCrm(userNames.Except(result.Keys).ToArray());
        ConditionalLog.Info(String.Format("GetUsers({0}). Retrieved {1} user(s) from CRM.", String.Join(",", userNames), users.Count), this, TimerAction.Tick, GetUsersKey);

        foreach (var user in users)
        {
          if (!result.ContainsKey(user.Name))
          {
            result.Add(user.Name, user);
            this.CacheService.UserCache.Add(user);
          }
        }
      }

      ConditionalLog.Info(String.Format("GetUsers({0}). Finished.", String.Join(",", userNames)), this, TimerAction.Stop, GetUsersKey);
      return result.Values.ToList();
    }

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <param name="userNames">The user names.</param>
    /// <returns>The users.</returns>
    protected abstract List<CRMUser> GetUsersFromCrm(string[] userNames);

    /// <summary>
    /// Gets the users number.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="propertyValue">Value the property.</param>
    /// <returns>The number of users.</returns>
    protected abstract int GetUsersNumber(string propertyName, string propertyValue);

    public string[] Attributes
    {
      get
      {
        var standardAttributes = new string[] { "contactid", "fullname", "firstname", "lastname", Configuration.Settings.UniqueKeyProperty };
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
