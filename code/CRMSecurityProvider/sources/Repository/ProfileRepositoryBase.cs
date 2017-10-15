namespace CRMSecurityProvider.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Profile repository base class.
  /// </summary>
  public abstract class ProfileRepositoryBase : RepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileRepositoryBase"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    protected ProfileRepositoryBase(UserRepositoryBase userRepository, ICacheService cacheService)
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
    /// Creates the contact attribute.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="attributeType">Type of the attribute.</param>
    /// <param name="throwIfExists">if set to <c>true</c> throw an exception.</param>
    /// <returns>The contact attribute.</returns>
    public abstract bool CreateContactAttribute(string attributeName, SupportedTypes attributeType, bool throwIfExists);

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public virtual object GetPropertyValue(CRMUser user, string propertyName)
    {
      return user.GetPropertyValue(propertyName);
    }

    /// <summary>
    /// Gets the user properties.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns>The user properties.</returns>
    public virtual Dictionary<string, object> GetUserProperties(string userName, string[] propertyNames)
    {
      Assert.ArgumentNotNull(userName, "userName");
      Assert.ArgumentNotNull(propertyNames, "propertyNames");

      const string GetUserPropertiesKey = "getUserProperties";
      ConditionalLog.Info(String.Format("GetUserProperties({0}). Started.", userName), this, TimerAction.Start, GetUserPropertiesKey);

      try
      {
        var user = this.UserRepository.GetUser(userName, propertyNames);
        if (user != null)
        {
          return user.ProfilePropertyNames.Where(propertyNames.Contains).ToDictionary(p => p, p => this.GetPropertyValue(user, p));
        }

        return new Dictionary<string, object>();
      }
      finally
      {
        ConditionalLog.Info(String.Format("GetUserProperties({0}). Finished.", userName), this, TimerAction.Stop, GetUserPropertiesKey);
      }
    }

    /// <summary>
    /// Gets the user property.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The user property.</returns>
    public virtual object GetUserProperty(string userName, string propertyName)
    {
      Assert.ArgumentNotNullOrEmpty(propertyName, "propertyName");

      var properties = this.GetUserProperties(userName, new[] { propertyName });
      return properties.Where(p => p.Key == propertyName).Select(p => p.Value).FirstOrDefault();
    }

    /// <summary>
    /// Updates the user properties.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="properties">The user properties.</param>
    /// <returns><c>true</c> if update was successful; otherwise, <c>false</c>.</returns>
    public abstract bool UpdateUserProperties(string userName, Dictionary<string, object> properties);

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Type of the property.</returns>
    public abstract SupportedTypes GetPropertyType(string propertyName);

    /// <summary>
    /// Processes FullName property.
    /// </summary>
    /// <param name="fullName">The value of FullName property.</param>
    /// <param name="properties">The properties.</param>
    protected void ProcessFullNameProperty(string fullName, Dictionary<string, object> properties)
    {
      if (String.IsNullOrEmpty(fullName))
      {
        return;
      }

      properties.Remove("fullname");

      var firstName = fullName;
      if (fullName.Contains(" "))
      {
        firstName = fullName.Substring(0, fullName.IndexOf(' ')).Trim();
        var lastName = fullName.Substring(fullName.IndexOf(' ')).Trim();

        properties["lastname"] = lastName;
      }
      properties["firstname"] = firstName;
    }
  }
}
