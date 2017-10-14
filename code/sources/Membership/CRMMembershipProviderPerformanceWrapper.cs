namespace CRMSecurityProvider
{
  using System.Collections.Specialized;
  using System.Web.Security;

  using Sitecore;
  using Sitecore.Diagnostics;

  public class CRMMembershipProviderPerformanceWrapper : MembershipProvider
  {
    #region Fields

    private string realProviderName;
    private MembershipProvider realProvider;

    #endregion

    #region Properties

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

    public override bool EnablePasswordReset
    {
      get
      {
        return this.RealProvider.EnablePasswordReset;
      }
    }

    public override bool EnablePasswordRetrieval
    {
      get
      {
        return this.RealProvider.EnablePasswordRetrieval;
      }
    }

    public override int MaxInvalidPasswordAttempts
    {
      get
      {
        return this.RealProvider.MaxInvalidPasswordAttempts;
      }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get
      {
        return this.RealProvider.MinRequiredNonAlphanumericCharacters;
      }
    }

    public override int MinRequiredPasswordLength
    {
      get
      {
        return this.RealProvider.MinRequiredPasswordLength;
      }
    }

    public override int PasswordAttemptWindow
    {
      get
      {
        return this.RealProvider.PasswordAttemptWindow;
      }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
      get
      {
        return this.RealProvider.PasswordFormat;
      }
    }

    public override string PasswordStrengthRegularExpression
    {
      get
      {
        return this.RealProvider.PasswordStrengthRegularExpression;
      }
    }

    public override bool RequiresQuestionAndAnswer
    {
      get
      {
        return this.RealProvider.RequiresQuestionAndAnswer;
      }
    }

    public override bool RequiresUniqueEmail
    {
      get
      {
        return this.RealProvider.RequiresUniqueEmail;
      }
    }

    [NotNull]
    private MembershipProvider RealProvider
    {
      get
      {
        if (this.realProvider == null)
        {
          Assert.IsNotNullOrEmpty(this.realProviderName, "Attempt to access the CRMMembershipProviderPerformanceWrapper.RealProvider property with an uninitialized provider name.");
          this.realProvider = Membership.Providers[this.realProviderName];
          Assert.IsNotNull(this.realProvider, "Unknown provider specified in CRMMembershipProviderPerformanceWrapper: {0}", this.realProviderName);
        }

        return this.realProvider;
      }
    }

    #endregion

    #region Methods

    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize(name, config);

      this.realProviderName = config["realProviderName"];
      Assert.IsNotNullOrEmpty(this.realProviderName, "Missing or empty 'realProviderName' attribute in CRMMembershipProviderPerformanceWrapper configuration.");
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      using (CrmProfiler.Measure("Membership.ChangePassword"))
      {
        return this.RealProvider.ChangePassword(username, oldPassword, newPassword);
      }
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      using (CrmProfiler.Measure("Membership.ChangePasswordQuestionAndAnswer"))
      {
        return this.RealProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
      }
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      using (CrmProfiler.Measure("Membership.CreateUser"))
      {
        return this.RealProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
      }
    }

    public override bool DeleteUser(string userName, bool deleteAllRelatedData)
    {
      using (CrmProfiler.Measure("Membership.DeleteUser"))
      {
        return this.RealProvider.DeleteUser(userName, deleteAllRelatedData);
      }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Membership.FindUsersByEmail"))
      {
        return this.RealProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
      }
    }

    public override MembershipUserCollection FindUsersByName(string userNameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Membership.FindUsersByName"))
      {
        return this.RealProvider.FindUsersByName(userNameToMatch, pageIndex, pageSize, out totalRecords);
      }
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      using (CrmProfiler.Measure("Membership.GetAllUsers"))
      {
        return this.RealProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
      }
    }

    public override int GetNumberOfUsersOnline()
    {
      using (CrmProfiler.Measure("Membership.GetNumberOfUsersOnline"))
      {
        return this.RealProvider.GetNumberOfUsersOnline();
      }
    }

    public override string GetPassword(string username, string answer)
    {
      using (CrmProfiler.Measure("Membership.GetPassword"))
      {
        return this.RealProvider.GetPassword(username, answer);
      }
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      using (CrmProfiler.Measure("Membership.GetUser (by user key)"))
      {
        return this.RealProvider.GetUser(providerUserKey, userIsOnline);
      }
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      using (CrmProfiler.Measure("Membership.GetUser (by user name)"))
      {
        return this.RealProvider.GetUser(username, userIsOnline);
      }
    }

    public override string GetUserNameByEmail(string email)
    {
      using (CrmProfiler.Measure("Membership.GetUserNameByEmail"))
      {
        return this.RealProvider.GetUserNameByEmail(email);
      }
    }

    public override string ResetPassword(string username, string answer)
    {
      using (CrmProfiler.Measure("Membership.ResetPassword"))
      {
        return this.RealProvider.ResetPassword(username, answer);
      }
    }

    public override bool UnlockUser(string userName)
    {
      using (CrmProfiler.Measure("Membership.UnlockUser"))
      {
        return this.RealProvider.UnlockUser(userName);
      }
    }

    public override void UpdateUser(MembershipUser user)
    {
      using (CrmProfiler.Measure("Membership.UpdateUser"))
      {
        this.RealProvider.UpdateUser(user);
      }
    }

    public override bool ValidateUser(string username, string password)
    {
      using (CrmProfiler.Measure("Membership.ValidateUser"))
      {
        return this.RealProvider.ValidateUser(username, password);
      }
    }

    #endregion
  }
}
