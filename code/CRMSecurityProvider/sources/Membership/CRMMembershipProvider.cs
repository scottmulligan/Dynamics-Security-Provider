namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Configuration.Provider;
  using System.Security.Cryptography;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web.Security;

  using Sitecore;
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Repository;
  using CRMSecurityProvider.Repository.Factory;
  using CRMSecurityProvider.Repository.V4;
  using CRMSecurityProvider.Repository.V5;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// Manages storage of membership information for an ASP.NET application in a CRM system.
  /// </summary>
  public class CRMMembershipProvider : MembershipProvider
  {
    #region Fields

    private readonly IUserRepositoryFactory userRepositoryFactory;
    private readonly IProfileRepositoryFactory profileRepositoryFactory;
    private UserRepositoryBase userRepository;
    private ProfileRepositoryBase profileRepository;
    private bool initialized;

    private int minRequiredPasswordLength;
    private int minRequiredNonalphanumericCharacters;
    private int maxInvalidPasswordAttempts;
    private int passwordAttemptWindow;
    private string passwordStrengthRegularExpression;
    private bool requiresUniqueEmail;
    private bool enablePasswordReset;
    private bool requiresQuestionAndAnswer;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMMembershipProvider"/> class.
    /// </summary>
    public CRMMembershipProvider()
      : this(new UserRepositoryFactory(), new ProfileRepositoryFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMMembershipProvider"/> class.
    /// </summary>
    /// <param name="userRepositoryFactory">The user repository factory.</param>
    /// <param name="profileRepositoryFactory">The profile repository factory.</param>
    public CRMMembershipProvider(IUserRepositoryFactory userRepositoryFactory, IProfileRepositoryFactory profileRepositoryFactory)
    {
      Assert.ArgumentNotNull(userRepositoryFactory, "userRepositoryFactory");
      Assert.ArgumentNotNull(profileRepositoryFactory, "profileRepositoryFactory");

      this.userRepositoryFactory = userRepositoryFactory;
      this.profileRepositoryFactory = profileRepositoryFactory;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The name of the application using the custom membership provider.
    /// </summary>
    public override string ApplicationName { get; set; }

    /// <summary>
    /// true if the provider is in read-only mode; otherwise, false.
    /// </summary>
    public bool ReadOnly { get; private set; }

    /// <summary>
    /// Gets a value indicating whether password field should be created automatically in CRM.
    /// </summary>
    public bool AutoCreatePasswordField { get; private set; }

    /// <summary>
    /// The name of the contact field where the password is stored.
    /// </summary>
    public string PasswordFieldName { get; private set; }

    /// <summary>
    /// Gets a value indicating the format for storing passwords in the membership data store.
    /// </summary>
    public override MembershipPasswordFormat PasswordFormat
    {
      get
      {
        return MembershipPasswordFormat.Hashed;
      }
    }

    /// <summary>
    /// Gets the minimum length required for a password.
    /// </summary>
    public override int MinRequiredPasswordLength
    {
      get
      {
        return this.minRequiredPasswordLength;
      }
    }

    /// <summary>
    /// Gets the minimum number of special characters that must be present in a valid password.
    /// </summary>
    public override int MinRequiredNonAlphanumericCharacters
    {
      get
      {
        return this.minRequiredNonalphanumericCharacters;
      }
    }

    /// <summary>
    /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
    /// </summary>
    public override int MaxInvalidPasswordAttempts
    {
      get
      {
        return this.maxInvalidPasswordAttempts;
      }
    }

    /// <summary>
    /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
    /// </summary>
    public override int PasswordAttemptWindow
    {
      get
      {
        return this.passwordAttemptWindow;
      }
    }

    /// <summary>
    /// Gets the regular expression used to evaluate a password.
    /// </summary>
    public override string PasswordStrengthRegularExpression
    {
      get
      {
        return this.passwordStrengthRegularExpression;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
    /// </summary>
    public override bool RequiresUniqueEmail
    {
      get
      {
        return this.requiresUniqueEmail;
      }
    }

    /// <summary>
    /// Indicates whether the membership provider is configured to allow users to reset their passwords.
    /// </summary>
    public override bool EnablePasswordReset
    {
      get
      {
        return this.enablePasswordReset;
      }
    }

    /// <summary>
    /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
    /// </summary>
    public override bool EnablePasswordRetrieval
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
    /// </summary>
    public override bool RequiresQuestionAndAnswer
    {
      get
      {
        return this.requiresQuestionAndAnswer;
      }
    }

    /// <summary>
    /// The name of the contact field where the password question is stored.
    /// </summary>
    public string PasswordQuestionFieldName { get; private set; }

    /// <summary>
    /// The name of the contact field where password answer is stored.
    /// </summary>
    public string PasswordAnswerFieldName { get; private set; }

    #endregion

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
        this.AutoCreatePasswordField = MainUtil.GetBool(config["autoCreatePasswordField"], false);
        this.PasswordFieldName = StringUtil.GetString((object)config["passwordFieldName"]);
        this.minRequiredPasswordLength = MainUtil.GetInt(config["minRequiredPasswordLength"], 7);
        this.minRequiredNonalphanumericCharacters = MainUtil.GetInt(config["minRequiredNonalphanumericCharacters"], 1);
        this.maxInvalidPasswordAttempts = MainUtil.GetInt(config["maxInvalidPasswordAttempts"], 5);
        this.passwordAttemptWindow = MainUtil.GetInt(config["passwordAttemptWindow"], 0);
        this.passwordStrengthRegularExpression = StringUtil.GetString((object)config["passwordStrengthRegularExpression"]).Trim();
        this.requiresUniqueEmail = MainUtil.GetBool(config["requiresUniqueEmail"], false);
        this.enablePasswordReset = MainUtil.GetBool(config["enablePasswordReset"], true);
        this.requiresQuestionAndAnswer = MainUtil.GetBool(config["requiresQuestionAndAnswer"], false);
        if (this.RequiresQuestionAndAnswer)
        {
          this.PasswordQuestionFieldName = config["passwordQuestionFieldName"];
          this.PasswordAnswerFieldName = config["passwordAnswerFieldName"];
        }

        if (!String.IsNullOrEmpty(this.passwordStrengthRegularExpression))
        {
          new Regex(this.passwordStrengthRegularExpression);
        }

        var connectionStringName = config["connectionStringName"];
        var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

        var settings = Configuration.ConfigurationSettings.Parse(connectionString);

        this.userRepository = this.userRepositoryFactory.GetRepository(settings);
        this.profileRepository = this.profileRepositoryFactory.GetRepository(settings);
        this.initialized = true;
      }
      catch (Exception e)
      {
        this.initialized = false;
        ConditionalLog.Error("The CRM provider couldn't be initialized.", e, this);

        return;
      }

      try
      {
        if (!String.IsNullOrEmpty(this.PasswordFieldName))
        {
          var passwordFieldType = this.profileRepository.GetPropertyType(this.PasswordFieldName);
          if (passwordFieldType != SupportedTypes.String)
          {
            this.PasswordFieldName = String.Empty;
            ConditionalLog.Warn("The attribute for the password storage isn't of String type. Password storage feature disabled.", this);
          }
        }
      }
      catch (ProviderException)
      {
        if ((this.userRepository is UserRepositoryV4 || this.userRepository is UserRepositoryV5) && this.AutoCreatePasswordField)
        {
          if (!this.profileRepository.CreateContactAttribute(this.PasswordFieldName, SupportedTypes.String, false))
          {
            this.PasswordFieldName = String.Empty;
            ConditionalLog.Warn("The attribute for password storage couldn't be created. Password storage feature disabled.", this);
          }
        }
        else
        {
          this.PasswordFieldName = String.Empty;
          ConditionalLog.Warn("The attribute for the password storage doesn't exist. Password storage feature disabled.", this);
        }
      }
    }

    /// <summary>
    /// Processes a request to update the password for a contact.
    /// </summary>
    /// <returns>true if the password was updated successfully; otherwise, false.</returns>
    /// <param name="username">The contact to update the password for. </param>
    /// <param name="oldPassword">The current password for the specified contact. </param>
    /// <param name="newPassword">The new password for the specified contact. </param>
    /// <exception cref="NotSupportedException">Couldn't change password as the CRM provider is in read-only mode.</exception>
    /// <exception cref="NotSupportedException">The CRM provider is not configured to store password.</exception>
    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't change password as the CRM provider is in read-only mode.");
        }

        if (String.IsNullOrEmpty(this.PasswordFieldName))
        {
          throw new NotSupportedException("The CRM provider is not configured to store password.");
        }

        var password = this.profileRepository.GetUserProperty(username, this.PasswordFieldName) as string;
        if (String.IsNullOrEmpty(password))
        {
          ConditionalLog.Warn(String.Format("CrmSecurityProvider: user \"{0}\" doesn't have a password. You should reset it before changing.", username), this);
          return false;
        }

        SHA1Managed sha1 = new SHA1Managed();
        byte[] oldPasswordBytes = Encoding.UTF8.GetBytes(oldPassword);
        byte[] hashedOldPasswordBytes = sha1.ComputeHash(oldPasswordBytes);
        string hashedOldPassword = System.Convert.ToBase64String(hashedOldPasswordBytes);

        if (password != hashedOldPassword)
        {
          return false;
        }

        if (!IsPasswordValid(newPassword))
        {
          return false;
        }

        byte[] newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
        byte[] hashedNewPasswordBytes = sha1.ComputeHash(newPasswordBytes);
        string hashedNewPassword = System.Convert.ToBase64String(hashedNewPasswordBytes);

        var properties = new Dictionary<string, object>();
        properties.Add(PasswordFieldName, hashedNewPassword);

        return this.profileRepository.UpdateUserProperties(username, properties);
      }

      return false;
    }

    private bool IsPasswordValid(string password)
    {
      if (password.Length < MinRequiredPasswordLength)
        return false;
      int nonAlphaNumericChars = 0;
      foreach (char passwordChar in password)
      {
        if (!char.IsLetterOrDigit(passwordChar))
          nonAlphaNumericChars++;
      }
      if (nonAlphaNumericChars < MinRequiredNonAlphanumericCharacters)
        return false;
      if ((this.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
        return false;

      return true;
    }

    /// <summary>
    /// Processes a request to update the password question and answer for a contact.
    /// </summary>
    /// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
    /// <param name="username">The contact to change the password question and answer for. </param>
    /// <param name="password">The password for the specified contact. </param>
    /// <param name="newPasswordQuestion">The new password question for the specified contact. </param>
    /// <param name="newPasswordAnswer">The new password answer for the specified contact. </param>
    /// <exception cref="NotSupportedException">Couldn't change password question or(and) password answer as the CRM provider is in read-only mode.</exception>
    /// <exception cref="NotSupportedException">The CRM provider is not configured to store password.</exception>
    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't change password question or(and) password answer as the CRM provider is in read-only mode.");
        }

        if (String.IsNullOrEmpty(this.PasswordFieldName))
        {
          throw new NotSupportedException("The CRM provider is not configured to store password.");
        }

        var validPassword = this.profileRepository.GetUserProperty(username, this.PasswordFieldName) as string;
        if (String.IsNullOrEmpty(validPassword))
        {
          ConditionalLog.Warn(String.Format("CrmSecurityProvider: user \"{0}\" doesn't have a password.", username), this);
          return false;
        }

        SHA1Managed sha1 = new SHA1Managed();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashedPasswordBytes = sha1.ComputeHash(passwordBytes);
        string hashedPassword = System.Convert.ToBase64String(hashedPasswordBytes);

        if (validPassword != hashedPassword)
        {
          return false;
        }

        var properties = new Dictionary<string, object>();
        properties.Add(this.PasswordQuestionFieldName, newPasswordQuestion);
        properties.Add(this.PasswordAnswerFieldName, newPasswordAnswer);

        return this.profileRepository.UpdateUserProperties(username, properties);
      }

      return false;
    }

    /// <summary>
    /// Adds a new contact to the CRM system.
    /// </summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created contact.</returns>
    /// <param name="username">The name for the new contact. </param>
    /// <param name="password">The password for the new contact. </param>
    /// <param name="email">The e-mail address for the new contact.</param>
    /// <param name="passwordQuestion">The password question for the new contact.</param>
    /// <param name="passwordAnswer">The password answer for the new contact.</param>
    /// <param name="isApproved">Whether or not the new contact is approved to be validated.</param>
    /// <param name="providerUserKey">The unique identifier for the contact.</param>
    /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the contact was created successfully.</param>
    /// <exception cref="NotSupportedException">Couldn't create user as the CRM provider is in read-only mode.</exception>
    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      if (!this.initialized)
      {
        status = MembershipCreateStatus.UserRejected;
        return null;
      }

      if (this.ReadOnly)
      {
        status = MembershipCreateStatus.UserRejected;
        throw new NotSupportedException("Couldn't create user as the CRM provider is in read-only mode.");
      }

      username = username.Trim();
      string userName = username;

      if (this.userRepository.GetUser(userName) != null)
      {
        status = MembershipCreateStatus.DuplicateUserName;
        return null;
      }
      Guid contactid = Guid.Empty;
      if (providerUserKey != null)
      {
        if (!(providerUserKey is Guid))
        {
          status = MembershipCreateStatus.InvalidProviderUserKey;
          return null;
        }
        contactid = (Guid)providerUserKey;
        if (this.userRepository.GetUser(contactid) != null)
        {
          status = MembershipCreateStatus.DuplicateProviderUserKey;
          return null;
        }
      }

      if ((string.IsNullOrEmpty(username)) || (username.Length > 256))
      {
        status = MembershipCreateStatus.InvalidUserName;
        return null;
      }

      // #Hotfix for issue 392091
      if (string.IsNullOrEmpty(email))
      {
        email = (Settings.UniqueKeyProperty == "emailaddress1") ? username : "mail@local.host";
      }

      if ((Settings.UniqueKeyProperty == "emailaddress1") && (email != null) && (userName != email))
      {
        CrmHelper.ShowMessage(
          "UserName and Email fields are not the same.\nThe CRM provider is configured to use emailaddress1 field as unique key.");
        status = MembershipCreateStatus.UserRejected;
        return null;
      }

      if (!string.IsNullOrEmpty(PasswordFieldName))
      {
        password = password.Trim();
        if ((password == string.Empty) || (password.Length > 128) || !IsPasswordValid(password))
        {
          status = MembershipCreateStatus.InvalidPassword;
          return null;
        }
        SHA1Managed sha1 = new SHA1Managed();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashedPasswordBytes = sha1.ComputeHash(passwordBytes);
        string hashedPassword = System.Convert.ToBase64String(hashedPasswordBytes);
        password = hashedPassword;
      }
      if (!string.IsNullOrEmpty(passwordQuestion))
      {
        passwordQuestion = passwordQuestion.Trim();
        if ((string.IsNullOrEmpty(passwordQuestion)) || (passwordQuestion.Length > 256))
        {
          status = MembershipCreateStatus.InvalidQuestion;
          return null;
        }
      }
      if (!string.IsNullOrEmpty(passwordAnswer))
      {
        passwordAnswer = passwordAnswer.Trim();
        if ((string.IsNullOrEmpty(passwordAnswer)) || (passwordAnswer.Length > 128))
        {
          status = MembershipCreateStatus.InvalidAnswer;
          return null;
        }
      }

      var crmUser = this.userRepository.CreateUser(userName, email, contactid);
      if (crmUser != null)
      {
        var propertiesToUpdate = new Dictionary<string, object>();
        if (!String.IsNullOrEmpty(this.PasswordFieldName))
        {
          propertiesToUpdate.Add(this.PasswordFieldName, password);
        }
        if (!String.IsNullOrEmpty(passwordQuestion) && this.RequiresQuestionAndAnswer)
        {
          propertiesToUpdate.Add(this.PasswordQuestionFieldName, passwordQuestion);
        }
        if (!String.IsNullOrEmpty(passwordAnswer) && this.RequiresQuestionAndAnswer)
        {
          propertiesToUpdate.Add(this.PasswordAnswerFieldName, passwordAnswer);
        }
        if (propertiesToUpdate.Count != 0)
        {
          this.profileRepository.UpdateUserProperties(userName, propertiesToUpdate);
        }

        status = MembershipCreateStatus.Success;
        return new CRMMembershipUser(this.Name, crmUser);
      }

      status = MembershipCreateStatus.UserRejected;
      return null;
    }

    /// <summary>
    /// Removes a user from the membership data source.
    /// </summary>
    /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
    /// <param name="username">The name of the user to delete.</param>
    /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
    /// <exception cref="NotSupportedException">Couldn't delete user as the CRM provider is in read-only mode.</exception>
    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't delete user as the CRM provider is in read-only mode.");
        }

        return this.userRepository.DeactivateUser(username);
      }

      return false;
    }

    /// <summary>
    /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
    /// </summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.</returns>
    /// <param name="emailToMatch">The e-mail address to search for.</param>
    /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">The total number of matched users.</param>
    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      if (this.initialized)
      {
        var result = new MembershipUserCollection();

        var crmUsers = this.userRepository.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        foreach (var crmUser in crmUsers)
        {
          result.Add(new CRMMembershipUser(this.Name, crmUser));
        }

        return result;
      }

      totalRecords = 0;
      return new MembershipUserCollection();
    }

    /// <summary>
    /// Gets a collection of membership users where the user name contains the specified user name to match.
    /// </summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.</returns>
    /// <param name="usernameToMatch">The user name to search for.</param>
    /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
    /// <param name="pageSize">The size of the page of results to return.</param>
    /// <param name="totalRecords">The total number of matched users.</param>
    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      if (this.initialized)
      {
        var result = new MembershipUserCollection();

        var crmUsers = this.userRepository.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        foreach (var crmUser in crmUsers)
        {
          result.Add(new CRMMembershipUser(this.Name, crmUser));
        }

        return result;
      }

      totalRecords = 0;
      return new MembershipUserCollection();
    }

    ///// <summary>
    ///// Gets a collection of all the users in the data source in pages of data.
    ///// </summary>
    ///// <returns>A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.</returns>
    ///// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
    ///// <param name="pageSize">The size of the page of results to return.</param>
    ///// <param name="totalRecords">The total number of matched users.</param>
    //public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    //{
    //  if (this.initialized)
    //  {
    //    var result = new MembershipUserCollection();

    //    var crmUsers = this.userRepository.GetAllUsers(pageIndex, pageSize, out totalRecords);
    //    foreach (var crmUser in crmUsers)
    //    {
    //      result.Add(new CRMMembershipUser(this.Name, crmUser));
    //    }

    //    return result;
    //  }

    //  totalRecords = 0;
    //  return new MembershipUserCollection();
    //}
    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      if (this.initialized)
      {
        var pageNumber = pageIndex + 1;
        var pagingInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo
        {
            PageNumber = pageNumber,
            Count = pageSize
        };
        var result = new MembershipUserCollection();

        var crmUsers = this.userRepository.GetAllUsers(pagingInfo, out totalRecords);
        foreach (var crmUser in crmUsers)
        {
          result.Add(new CRMMembershipUser(this.Name, crmUser));
        }

        return result;
      }

      totalRecords = 0;
      return new MembershipUserCollection();
    }

    /// <summary>
    /// Gets the number of users currently accessing the application.
    /// </summary>
    /// <returns>The number of users currently accessing the application.</returns>
    /// <exception cref="NotSupportedException">CRMMembership_OnlineUsers_not_supported.</exception>
    public override int GetNumberOfUsersOnline()
    {
      throw new NotSupportedException("CRMMembership_OnlineUsers_not_supported.");
    }

    /// <summary>
    /// Gets the password for the specified contact from the CRM system. This method isn't supported as password stored as HASH value and can't be retrieved.
    /// </summary>
    /// <param name="username">The contact to retrieve the password for. </param>
    /// <param name="answer">The password answer for the contact. </param>
    /// <returns>The password for the specified contact.</returns>
    /// <exception cref="NotSupportedException">The password can't be retrieved as it is stored as HASH value.</exception>
    public override string GetPassword(string username, string answer)
    {
      throw new NotSupportedException("CRMMembership_PasswordRetrieval_not_supported");
    }

    /// <summary>
    /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
    /// </summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.</returns>
    /// <param name="username">The name of the user to get information for. </param>
    /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      if (this.initialized)
      {
        if (String.IsNullOrEmpty(username) || username.Contains("\\"))
        {
          return null;
        }

        var crmUser = this.userRepository.GetUser(username);
        if (crmUser != null)
        {
          return new CRMMembershipUser(this.Name, crmUser);
        }
      }

      return null;
    }

    /// <summary>
    /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
    /// </summary>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.</returns>
    /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
    /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      if (this.initialized)
      {
        if (providerUserKey is Guid)
        {
          var crmUser = this.userRepository.GetUser((Guid)providerUserKey);
          if (crmUser != null)
          {
            return new CRMMembershipUser(Name, crmUser);
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the contact name associated with the specified e-mail address.
    /// </summary>
    /// <param name="email">The e-mail address to search for. </param>
    /// <returns>The contact name associated with the specified e-mail address. If no match is found, return null.</returns>
    public override string GetUserNameByEmail(string email)
    {
      if (this.initialized)
      {
        int total;
        var users = this.FindUsersByEmail(email, 0, 1, out total);
        foreach (MembershipUser user in users)
        {
          return user.UserName;
        }
      }

      return null;
    }

    /// <summary>
    /// Resets a contact's password to a new, automatically generated password.
    /// </summary>
    /// <param name="username">The contact to reset the password for.</param>
    /// <param name="answer">The password answer for the specified contact.</param>
    /// <returns>The new password for the specified contact.</returns>
    /// <exception cref="NotSupportedException">The CRM provider isn't initialized.</exception>
    /// <exception cref="NotSupportedException">Couldn't reset password as the CRM provider is in read-only mode.</exception>
    /// <exception cref="NotSupportedException">The CRM provider is not configured to store password.</exception>
    /// <exception cref="NotSupportedException">The CRM provider is not configured to reset password.</exception>
    /// <exception cref="MembershipPasswordException">The password answer is wrong.</exception>
    /// <exception cref="MembershipPasswordException">Couldn't reset password in CRM. Check log file for details.</exception>
    public override string ResetPassword(string username, string answer)
    {
      if (!this.initialized)
      {
        throw new NotSupportedException("The CRM provider isn't initialized.");
      }

      if (this.ReadOnly)
      {
        throw new NotSupportedException("Couldn't reset password as the CRM provider is in read-only mode.");
      }

      if (String.IsNullOrEmpty(this.PasswordFieldName))
      {
        throw new NotSupportedException("The CRM provider is not configured to store password.");
      }

      if (!this.EnablePasswordReset)
      {
        throw new NotSupportedException("The CRM provider is not configured to reset password.");
      }

      if (this.RequiresQuestionAndAnswer)
      {
        var correctAnswer = this.profileRepository.GetUserProperty(username, this.PasswordAnswerFieldName) as string;
        if (answer != null)
        {
          answer = answer.Trim();
        }

        if (String.IsNullOrEmpty(answer) || (answer != correctAnswer))
        {
          throw new MembershipPasswordException("The password answer is wrong.");
        }
      }

      var newPassword = Membership.GeneratePassword(this.MinRequiredPasswordLength < 14 ? 14 : this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);
      
      var sha1 = new SHA1Managed();
      var passwordBytes = Encoding.UTF8.GetBytes(newPassword);
      var hashedPasswordBytes = sha1.ComputeHash(passwordBytes);
      var hashedPassword = System.Convert.ToBase64String(hashedPasswordBytes);

      var properties = new Dictionary<string, object>();
      properties.Add(this.PasswordFieldName, hashedPassword);

      if (!this.profileRepository.UpdateUserProperties(username, properties))
      {
        throw new MembershipPasswordException("Couldn't reset password in CRM. Check log file for details.");
      }

      return newPassword;
    }

    /// <summary>
    /// Clears a lock so that the membership user can be validated. The locked users isn't supported by the provider.
    /// </summary>
    /// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
    /// <param name="userName">The membership user whose lock status you want to clear.</param>
    public override bool UnlockUser(string userName)
    {
      return true;
    }

    /// <summary>
    /// Updates information about a user in the data source.
    /// </summary>
    /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
    /// <exception cref="NotSupportedException">Couldn't update user as the CRM provider is in read-only mode.</exception>
    /// <exception cref="ApplicationException">Couldn't update user in CRM. Check log file for details.</exception>
    public override void UpdateUser(MembershipUser user)
    {
      if (this.initialized)
      {
        if (this.ReadOnly)
        {
          throw new NotSupportedException("Couldn't update user as the CRM provider is in read-only mode.");
        }

        var crmUser = user as CRMMembershipUser;
        if (crmUser != null)
        {
          var properties = new Dictionary<string, object>();
          if (crmUser.EmailModified)
          {
            properties.Add("emailaddress1", crmUser.Email);
          }
          if (crmUser.CommentModified)
          {
            properties.Add("description", crmUser.Comment);
          }
          if (properties.Count != 0)
          {
            if (!this.profileRepository.UpdateUserProperties(user.UserName, properties))
            {
              throw new ApplicationException("Couldn't update user in CRM. Check log file for details.");
            }
          }
        }
      }
    }

    /// <summary>
    /// Verifies that the specified contact name and password exist in the CRM system.
    /// </summary>
    /// <returns>true if the specified contact name and password are valid; otherwise, false.</returns>
    /// <param name="username">The name of the contact to validate. </param>
    /// <param name="password">The password for the specified contact. </param>
    public override bool ValidateUser(string username, string password)
    {
      if (!this.initialized || String.IsNullOrEmpty(this.PasswordFieldName))
      {
        return false;
      }

      var userPassword = this.profileRepository.GetUserProperty(username, this.PasswordFieldName) as string;
      if (String.IsNullOrEmpty(userPassword))
      {
        return false;
      }

      var sha1 = new SHA1Managed();
      var passwordBytes = Encoding.UTF8.GetBytes(password);
      var hashedPasswordBytes = sha1.ComputeHash(passwordBytes);
      var hashedPassword = System.Convert.ToBase64String(hashedPasswordBytes);

      return (hashedPassword == userPassword);
    }
  }
}
