namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Configuration.Provider;
  using System.Linq;
  using System.Web.Configuration;
  using System.Web.Profile;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Repository;
  using CRMSecurityProvider.Repository.Factory;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Manages storage of profile information for an ASP.NET application in a CRM system.
  /// </summary>
  public class CRMProfileProvider : ProfileProvider
  {
    private readonly IProfileRepositoryFactory profileRepositoryFactory;
    private readonly IUserRepositoryFactory userRepositoryFactory;
    private ProfileRepositoryBase profileRepository;
    private UserRepositoryBase userRepository;
    private bool initialized;

    private Dictionary<string, string> propertyNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMProfileProvider"/> class.
    /// </summary>
    public CRMProfileProvider()
      : this(new ProfileRepositoryFactory(), new UserRepositoryFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMProfileProvider"/> class.
    /// </summary>
    /// <param name="profileRepositoryFactory">The profile repository factory.</param>
    /// <param name="userRepositoryFactory">The user repository factory.</param>
    public CRMProfileProvider(IProfileRepositoryFactory profileRepositoryFactory, IUserRepositoryFactory userRepositoryFactory)
    {
      Assert.ArgumentNotNull(profileRepositoryFactory, "profileRepositoryFactory");
      Assert.ArgumentNotNull(userRepositoryFactory, "userRepositoryFactory");

      this.profileRepositoryFactory = profileRepositoryFactory;
      this.userRepositoryFactory = userRepositoryFactory;
    }

    /// <summary>
    /// Gets or sets the name of the currently running application.
    /// </summary>
    public override string ApplicationName { get; set; }

    /// <summary>
    /// true if the provider is in read-only-mode; otherwise, false.
    /// </summary>
    public bool ReadOnly { get; private set; }

    #region Private Methods

    /// <summary>
    /// Setups the custom property names.
    /// </summary>
    private void SetupCustomPropertyNames()
    {
      this.propertyNames = new Dictionary<string, string>();

      var configuration = WebConfigurationManager.OpenWebConfiguration("/aspnet");
      var profileSection = (System.Web.Configuration.ProfileSection)configuration.GetSection("system.web/profile");

      this.SetPropertiesList(profileSection.PropertySettings);
    }

    /// <summary>
    /// Sets the properties list.
    /// </summary>
    /// <param name="settings">The settings.</param>
    private void SetPropertiesList(RootProfilePropertySettingsCollection settings)
    {
      foreach (ProfilePropertySettings propertyDefinition in settings)
      {
        string customProviderData = propertyDefinition.CustomProviderData.Trim();
        if ((!string.IsNullOrEmpty(customProviderData)) && customProviderData.StartsWith("crm|"))
        {
          string crmPropertyName = customProviderData.Split('|')[1];
          if (!string.IsNullOrEmpty(crmPropertyName))
          {
            crmPropertyName = crmPropertyName.Trim();
            var crmPropertyType = this.profileRepository.GetPropertyType(crmPropertyName).ToString();
            this.propertyNames.Add(crmPropertyName, crmPropertyType);
          }
        }
      }
    }

    /// <summary>
    /// Gets the last active date.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>
    /// The last active date.
    /// </returns>
    private static DateTime GetLastActiveDate(string userName)
    {
      //note: lastactivitydate or lastmodifiedon properties should be used
      return DateTime.Now;
    }

    /// <summary>
    /// Determines whether the specified user name has profile.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns>
    ///   <c>true</c> if the specified user name has profile; otherwise, <c>false</c>.
    /// </returns>
    private bool HasProfile(string userName)
    {
      if (this.initialized)
      {
        return this.propertyNames.Keys.Select(p => this.profileRepository.GetUserProperty(userName, p)).Any(p => p != null);
      }

      return false;
    }

    /// <summary>
    /// Determines whether the specified custom provider data is relevant.
    /// </summary>
    /// <param name="customProviderData">The custom provider data.</param>
    /// <returns>
    ///   <c>true</c> if the specified custom provider data is relevant; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsRelevant(string customProviderData)
    {
      return !String.IsNullOrEmpty(customProviderData) && customProviderData.StartsWith("crm|");
    }

    #endregion Private Methods

    #region CrmProvider specific

    /// <summary>
    /// Gets the relevant properties.
    /// </summary>
    /// <param name="allProperties">All properties.</param>
    /// <returns>
    /// The relevant properties.
    /// </returns>
    protected virtual SettingsPropertyCollection GetRelevantProperties(SettingsPropertyCollection allProperties)
    {
      var relevantProperties = new SettingsPropertyCollection();
      foreach (SettingsProperty property in allProperties)
      {
        CrmSettingsProperty crmProperty;
        if (this.GetIfRelevant(property, out crmProperty))
        {
          relevantProperties.Add(crmProperty);
        }
      }

      return relevantProperties;
    }

    /// <summary>
    /// Gets if relevant.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="crmProperty">The CRM property.</param>
    /// <returns>
    /// The if relevant.
    /// </returns>
    private bool GetIfRelevant(SettingsProperty property, out CrmSettingsProperty crmProperty)
    {
      var customData = property.Attributes["CustomProviderData"] as string;
      if (IsRelevant(customData))
      {
        string crmName;
        string crmType;
        this.ParseCustomData(customData, out crmName, out crmType);
        if ((!string.IsNullOrEmpty(crmName)) && (!string.IsNullOrEmpty(crmType)))
        {
          crmProperty = new CrmSettingsProperty(crmName, crmType, property);
          return true;
        }
      }

      if (property.Name == "FullName")
      {
        crmProperty = new CrmSettingsProperty("fullname", "String", property);
        return true;
      }

      if (property.Name == "Comment")
      {
        crmProperty = new CrmSettingsProperty("description", "String", property);
        return true;
      }

      if (property.Name == "Email")
      {
        crmProperty = new CrmSettingsProperty("emailaddress1", "String", property);
        return true;
      }

      crmProperty = null;
      return false;
    }

    /// <summary>
    /// Parses the custom data.
    /// </summary>
    /// <param name="customData">The custom data.</param>
    /// <param name="crmName">Name of the CRM.</param>
    /// <param name="crmType">Type of the CRM.</param>
    private void ParseCustomData(string customData, out string crmName, out string crmType)
    {
      Assert.IsNotNullOrEmpty(customData, "customData can't be null or empty.");
      crmName = string.Empty;
      crmType = string.Empty;
      string[] customDataElements = customData.Split(new[]
      {
        '|'
      }, StringSplitOptions.RemoveEmptyEntries);
      if ((customDataElements.Length >= 2) && (customDataElements[0] == "crm"))
      {
        crmName = customDataElements[1].Trim();
        if (!string.IsNullOrEmpty(crmName))
        {
          crmType = this.propertyNames[crmName];
        }
      }
    }

    /// <summary>
    /// Gets the property values relevant to the CRM system from the collection.
    /// </summary>
    /// <param name="allPropertyValues">The collection of the property values to filter the CRM relevant property values.</param>
    /// <returns>
    /// The collection of the CRM relevant property values.
    /// </returns>
    protected virtual SettingsPropertyValueCollection GetRelevantPropertyValues(SettingsPropertyValueCollection allPropertyValues)
    {
      var propertyValues = new SettingsPropertyValueCollection();

      foreach (SettingsPropertyValue propertyValue in allPropertyValues)
      {
        CrmSettingsProperty crmProperty;
        if (this.GetIfRelevant(propertyValue.Property, out crmProperty))
        {
          var crmPropertyValue = new CrmSettingsPropertyValue(propertyValue, crmProperty);
          propertyValues.Add(crmPropertyValue);
        }
      }

      return propertyValues;
    }

    /// <summary>
    /// The delegate defines which attribute value to be retrieved from property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns></returns>
    protected delegate string GetAttribute(SettingsProperty property);

    /// <summary>
    /// Processes the property collection.
    /// </summary>
    /// <param name="properties">The collection of the properties to be processed.</param>
    /// <param name="getAttribute">The method which defines how the properties to be processed.</param>
    /// <returns>
    /// The array of the string values retrieved from the properties while processing.
    /// </returns>
    protected virtual string[] ProcessProperties(SettingsPropertyCollection properties, GetAttribute getAttribute)
    {
      return (from SettingsProperty property in properties
              select getAttribute(property)).ToArray();
    }

    /// <summary>
    /// Processes the property.
    /// </summary>
    /// <param name="property">The property to process.</param>
    /// <param name="getAttribute">The method which defines how the property to be processed.</param>
    /// <returns>
    /// The string value retrieved from the property while processing
    /// </returns>
    protected virtual string ProcessProperty(SettingsProperty property, GetAttribute getAttribute)
    {
      return getAttribute(property);
    }

    #endregion CrmProvider specific

    #region Overrides

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

        var settings = Configuration.ConfigurationSettings.Parse(connectionString);

        this.profileRepository = this.profileRepositoryFactory.GetRepository(settings);
        this.userRepository = this.userRepositoryFactory.GetRepository(settings);

        this.SetupCustomPropertyNames();

        this.initialized = true;
      }
      catch (Exception e)
      {
        this.initialized = false;
        ConditionalLog.Error("The CRM provider couldn't be initialized.", e, this);
      }
    }

    ///// <summary>
    ///// Deletes all contact-profile data for profiles in which the last activity date occurred before the specified date.
    ///// </summary>
    ///// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are deleted.</param>
    ///// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which contact profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  value of a contact profile occurs on or before this date and time, the profile is considered inactive.</param>
    ///// <returns>The number of profiles deleted from the CRM system.</returns>
    //public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    //{
    //  if (!this.initialized || this.ReadOnly)
    //  {
    //    return 0;
    //  }

    //  int totalUsers;

    //  var profiles = (from user in this.userRepository.GetAllUsers(0, Int32.MaxValue, out totalUsers)
    //                  where this.HasProfile(user.Name) && (user.LastActivityDate <= userInactiveSinceDate)
    //                  select user.Name).ToArray();

    //  return this.DeleteProfiles(profiles);
    //}
    public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      if (!this.initialized || this.ReadOnly)
      {
        return 0;
      }

      int totalUsers;
      var pagingInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo
      {
          Count = Int32.MaxValue,
          PageNumber = 1
      };
      var profiles = (from user in this.userRepository.GetAllUsers(pagingInfo, out totalUsers)
                      where this.HasProfile(user.Name) && (user.LastActivityDate <= userInactiveSinceDate)
                      select user.Name).ToArray();

      return this.DeleteProfiles(profiles);
    }

    /// <summary>
    /// Deletes profile properties and information for profiles that match the supplied list of contact names.
    /// </summary>
    /// <param name="usernames">A string array of contact names for profiles to be deleted.</param>
    /// <returns>The number of profiles deleted from the CRM system.</returns>
    public override int DeleteProfiles(string[] usernames)
    {
      if (!this.initialized || this.ReadOnly)
      {
        return 0;
      }

      int numberOfDeletedProfiles = 0;
      foreach (string userName in usernames)
      {
        if (!this.HasProfile(userName))
        {
          continue;
        }

        var properties = new Dictionary<string, object>();
        this.propertyNames.Keys.ToList().ForEach(propertyName => properties.Add(propertyName, null));

        if (properties.Count != 0)
        {
          if (this.profileRepository.UpdateUserProperties(userName, properties))
          {
            numberOfDeletedProfiles++;
          }
          else
          {
            ConditionalLog.Error(String.Format("Couldn't delete profile for the {0} user.", userName), this);
          }
        }
      }

      return numberOfDeletedProfiles;
    }

    /// <summary>
    /// Deletes profile properties and information for the supplied list of profiles.
    /// </summary>
    /// <param name="profiles">A <see cref="T:System.Web.Profile.ProfileInfoCollection"/>  of information about profiles that are to be deleted.</param>
    /// <returns>The number of profiles deleted from the CRM system.</returns>
    public override int DeleteProfiles(ProfileInfoCollection profiles)
    {
      if (!this.initialized || this.ReadOnly)
      {
        return 0;
      }

      var list = (from ProfileInfo profile in profiles
                  select profile.UserName).ToArray();
      return this.DeleteProfiles(list);
    }

    /// <summary>
    /// Retrieves profile information for profiles in which the last activity date occurred on or before the specified date and the user name matches the specified contact name.
    /// </summary>
    /// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
    /// <param name="usernameToMatch">The contact name to search for.</param>
    /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which contact profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/> value of a contact profile occurs on or before this date and time, the profile is considered inactive.</param>
    /// <param name="pageIndex">The index of the page of results to return.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
    /// <returns>
    /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing contact profile information for inactive profiles where the contact name matches the supplied <paramref name="usernameToMatch"/> parameter.
    /// </returns>
    public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
    {
      if (!this.initialized)
      {
        totalRecords = 0;
        return new ProfileInfoCollection();
      }

      var result = new ProfileInfoCollection();
      int pageNumber = pageIndex;
      do
      {
        int matchingProfilesCount;
        ProfileInfoCollection matchingProfiles = this.FindProfilesByUserName(authenticationOption, usernameToMatch,
          pageNumber,
          pageSize,
          out matchingProfilesCount);

        foreach (var profile in matchingProfiles.OfType<ProfileInfo>().Where(p => GetLastActiveDate(p.UserName) < userInactiveSinceDate))
        {
          result.Add(profile);
        }

        if (((pageNumber + 1) * pageSize) >= matchingProfilesCount)
        {
          break;
        }

        pageNumber++;
      }
      while (result.Count < pageSize);
      totalRecords = result.Count;
      return result;
    }

    /// <summary>
    /// Retrieves profile information for profiles in which the contact name matches the specified contact names.
    /// </summary>
    /// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
    /// <param name="usernameToMatch">The contact name to search for.</param>
    /// <param name="pageIndex">The index of the page of results to return.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
    /// <returns>
    /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing contact-profile information for profiles where the contact name matches the supplied <paramref name="usernameToMatch"/> parameter.
    /// </returns>
    public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      if (!this.initialized)
      {
        totalRecords = 0;
        return new ProfileInfoCollection();
      }

      var result = new ProfileInfoCollection();
      int pageNumber = pageIndex;
      do
      {
        int matchingUsersNumber;
        var users = this.userRepository.FindUsersByName(usernameToMatch, pageNumber, pageSize, out matchingUsersNumber);
        foreach (var user in users.Where(user => this.HasProfile(user.Name)))
        {
          //Note: profile size should be counted.
          result.Add(new ProfileInfo(user.Name, false, DateTime.Now, DateTime.Now, 0));
        }

        if (((pageNumber + 1) * pageSize) >= matchingUsersNumber)
        {
          break;
        }

        pageNumber++;
      }
      while (result.Count < pageSize);

      totalRecords = result.Count;
      return result;
    }

    /// <summary>
    /// Retrieves contact-profile data from the CRM system for profiles in which the last activity date occurred on or before the specified date.
    /// </summary>
    /// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
    /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which contact profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of a contact profile occurs on or before this date and time, the profile is considered inactive.</param>
    /// <param name="pageIndex">The index of the page of results to return.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
    /// <returns>
    /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing contact-profile information about the inactive profiles.
    /// </returns>
    public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
    {
      if (!this.initialized)
      {
        totalRecords = 0;
        return new ProfileInfoCollection();
      }

      var result = new ProfileInfoCollection();
      int pageNumber = pageIndex + 1;
      do
      {
        int totalUsers;
        var pagingInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo
        {
            Count = pageSize,
            PageNumber = pageNumber
        };
        var users = this.userRepository.GetAllUsers(pagingInfo, out totalUsers);
        foreach (var user in users.Where(user => this.HasProfile(user.Name) && (user.LastActivityDate <= userInactiveSinceDate)))
        {
          //Note: profile size should be counted.
          result.Add(new ProfileInfo(user.Name, false, DateTime.Now, DateTime.Now, 0));
        }

        if (((pageNumber + 1) * pageSize) >= totalUsers)
        {
          break;
        }

        pageNumber++;
      }
      while (result.Count < pageSize);

      totalRecords = result.Count;
      return result;
    }

    /// <summary>
    /// Retrieves contact profile data for all profiles in the CRM system.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing contact-profile information for all profiles in the CRM system.
    /// </returns>
    /// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
    /// <param name="pageIndex">The index of the page of results to return.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
    public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
    {
      if (!this.initialized)
      {
        totalRecords = 0;
        return new ProfileInfoCollection();
      }

      var profiles = new ProfileInfoCollection();
      int pageNumber = pageIndex + 1;
      do
      {
        var pagingInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo
        {
            Count = pageSize,
            PageNumber = pageNumber
        };
        var users = this.userRepository.GetAllUsers(pagingInfo, out totalRecords);
        foreach (var user in users.Where(user => this.HasProfile(user.Name)))
        {
          //Note: profile size should be counted.
          profiles.Add(new ProfileInfo(user.Name, false, DateTime.Now, DateTime.Now, 0));
        }

        if (((pageNumber + 1) * pageSize) >= totalRecords)
        {
          break;
        }

        pageNumber++;
      }
      while (profiles.Count < pageSize);

      totalRecords = profiles.Count;
      return profiles;
    }

    /// <summary>
    /// Returns the number of profiles in which the last activity date occurred on or before the specified date.
    /// </summary>
    /// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
    /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> that identifies which contact profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of a contact profile occurs on or before this date and time, the profile is considered inactive.</param>
    /// <returns>The number of profiles in which the last activity date occurred on or before the specified date.</returns>
    public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      if (!this.initialized)
      {
        return 0;
      }

      int inactiveProfileNumber;
      this.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, 0, 1, out inactiveProfileNumber);
      return inactiveProfileNumber;
    }

    /// <summary>
    /// Returns the collection of settings property values for the specified application instance and settings property group.
    /// </summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application use.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyCollection"/> containing the settings property group whose values are to be retrieved.</param>
    /// <returns>
    /// A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> containing the values for the specified settings property group.
    /// </returns>
    /// <exception cref="ProviderException">Couldn't resolve CRM property type</exception>
    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
    { 
      if (this.initialized)
      {
        var spvc = new SettingsPropertyValueCollection();
        var userName = (string)context["UserName"];
        if (userName != string.Empty)
        {
          if ((!string.IsNullOrEmpty(userName)) && (!userName.Contains("\\")))
          {
            var attributes = new List<string>();
            SettingsPropertyCollection relevantProperties = this.GetRelevantProperties(collection);
            if (relevantProperties.Count > 0)
            {
              attributes.AddRange(this.ProcessProperties(relevantProperties, p => (p as CrmSettingsProperty).CrmName));
              var propertyValues = this.profileRepository.GetUserProperties(userName, attributes.ToArray());
              foreach (CrmSettingsProperty property in relevantProperties)
              {
                if (collection[property.Name] == null)
                {
                  continue;
                }

                var crmPropertyValue = new CrmSettingsPropertyValue(collection[property.Name])
                {
                  PropertyValue = string.Empty
                };
                collection.Remove(property.Name);
                crmPropertyValue.PropertyValue = propertyValues.ContainsKey(property.CrmName)
                  ? propertyValues[property.CrmName]
                  : string.Empty;
                crmPropertyValue.IsDirty = false;
                spvc.Add(crmPropertyValue);
              }
            }
          }
        }

        return spvc;
      }

      return new SettingsPropertyValueCollection();
    }

    /// <summary>
    /// Sets the values of the specified group of property settings.
    /// </summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext"/> describing the current application usage.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyValueCollection"/> representing the group of property settings to set.</param>
    /// <exception cref="FormatException">The profile property couldn't be parsed.</exception>
    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
      if (this.initialized)
      {
        bool showMessage = false;
        var userName = (string)context["UserName"];
        if (string.IsNullOrEmpty(userName) || userName.Contains("\\"))
        {
          return;
        }

        SettingsPropertyValueCollection relevantPropertyValues = this.GetRelevantPropertyValues(collection);
        var newPropertyValues = new Dictionary<string, object>();

        foreach (CrmSettingsPropertyValue property in relevantPropertyValues)
        {
          string propertyName = this.ProcessProperty(property.Property, p => (p as CrmSettingsProperty).CrmName);
          object propertyValue = property.PropertyValue;
          if ((propertyName != "contactid") && (property.PropertyValue != null) && !showMessage)
          {
            if (this.ReadOnly)
            {
              showMessage = property.IsDirty;
            }
            else
            {
              if (property.IsDirty && this.initialized)
              {
                newPropertyValues.Add(propertyName, propertyValue);
              }
            }
          }

          collection.Remove(property.Name);
        }

        if (!this.ReadOnly && (newPropertyValues.Count != 0))
        {
          if (!this.profileRepository.UpdateUserProperties(userName, newPropertyValues))
          {
            ConditionalLog.Error(String.Format("Couldn't save profile changes for the {0} contact in CRM", userName), this);
          }
        }

        if (this.ReadOnly && showMessage)
        {
          CrmHelper.ShowMessage("Couldn't update contact properties as the CRM provider is in read-only mode");
        }
      }
    }

    #endregion Overrides

    #region Internal classes

    /// <summary>
    /// Used internally as the class that represents metadata about an individual CRM configuration property.
    /// </summary>
    internal class CrmSettingsProperty : SettingsProperty
    {
      /// <summary>
      /// The name of the attribute in CRM entity.
      /// </summary>
      /// <value>
      /// The name of the CRM.
      /// </value>
      public string CrmName { get; private set; }

      /// <summary>
      /// The type of the attribute in CRM entity.
      /// </summary>
      /// <value>
      /// The type of the CRM.
      /// </value>
      public string CrmType { get; private set; }

      /// <summary>
      /// The property value size.
      /// </summary>
      public int Size
      {
        get
        {
          return 0;
        }
      }

      /// <summary>
      /// Constructor of the CRM settings property.
      /// </summary>
      /// <param name="crmName">The name of the attribute in CRM entity.</param>
      /// <param name="crmType">The type of the attribute in CRM entity.</param>
      /// <param name="property">The base settings property object.</param>
      public CrmSettingsProperty(string crmName, string crmType, SettingsProperty property)
        : base(property)
      {
        this.CrmName = crmName;
        this.CrmType = crmType;
      }
    }

    /// <summary>
    /// Defines the CRM settings property value class.
    /// </summary>
    internal class CrmSettingsPropertyValue : SettingsPropertyValue
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="CrmSettingsPropertyValue"/> class.
      /// </summary>
      /// <param name="property">Specifies a <see cref="T:System.Configuration.SettingsProperty"/> object.</param>
      public CrmSettingsPropertyValue(SettingsProperty property)
        : base(property)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CrmSettingsPropertyValue"/> class.
      /// </summary>
      /// <param name="propertyValue">The property value.</param>
      /// <param name="property">The property.</param>
      public CrmSettingsPropertyValue(SettingsPropertyValue propertyValue, SettingsProperty property)
        : base(property)
      {
        PropertyValue = propertyValue.PropertyValue;
        Deserialized = propertyValue.Deserialized;
        IsDirty = propertyValue.IsDirty;
        SerializedValue = propertyValue.SerializedValue;
      }
    }

    #endregion Internal classes
  }
}