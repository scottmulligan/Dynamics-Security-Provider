namespace CRMSecurityProvider
{
  using System.Web.Security;
  using CRMSecurityProvider.Sources;

  /// <summary>
  ///   Crm anonymous membership provider
  /// </summary>
  public class CRMAnonymousMembershipProvider : CRMMembershipProvider
  {
    #region Public methods

    /// <summary>
    /// Processes a request to update the password for a contact.
    /// </summary>
    /// <param name="username">The contact to update the password for.</param>
    /// <param name="oldPassword">The current password for the specified contact.</param>
    /// <param name="newPassword">The new password for the specified contact.</param>
    /// <returns>
    /// true if the password was updated successfully; otherwise, false.
    /// </returns>
    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.ChangePassword(username, oldPassword, newPassword);
      }
      return false;
    }

    /// <summary>
    /// Processes a request to update the password question and answer for a contact.
    /// </summary>
    /// <param name="username">The contact to change the password question and answer for.</param>
    /// <param name="password">The password for the specified contact.</param>
    /// <param name="newPasswordQuestion">The new password question for the specified contact.</param>
    /// <param name="newPasswordAnswer">The new password answer for the specified contact.</param>
    /// <returns>
    /// true if the password question and answer are updated successfully; otherwise, false.
    /// </returns>
    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
      }
      return false;
    }

    /// <summary>
    /// Adds a new contact to the CRM system.
    /// </summary>
    /// <param name="username">The name for the new contact.</param>
    /// <param name="password">The password for the new contact.</param>
    /// <param name="email">The e-mail address for the new contact.</param>
    /// <param name="passwordQuestion">The password question for the new contact.</param>
    /// <param name="passwordAnswer">The password answer for the new contact.</param>
    /// <param name="isApproved">Whether or not the new contact is approved to be validated.</param>
    /// <param name="providerUserKey">The unique identifier for the contact.</param>
    /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value indicating whether the contact was created successfully.</param>
    /// <returns>
    /// A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the information for the newly created contact.
    /// </returns>
    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
      }

      status = MembershipCreateStatus.UserRejected;
      return null;
    }

    /// <summary>
    /// Removes a user from the membership data source.
    /// </summary>
    /// <param name="username">The name of the user to delete.</param>
    /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
    /// <returns>
    /// true if the user was successfully deleted; otherwise, false.
    /// </returns>
    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.DeleteUser(username, deleteAllRelatedData);
      }
      return false;
    }

    /// <summary>
    /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
    /// </summary>
    /// <param name="username">The name of the user to get information for.</param>
    /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
    /// <returns>
    /// A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.
    /// </returns>
    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.GetUser(username, userIsOnline);
      }
      return null;
    }

    /// <summary>
    /// Clears a lock so that the membership user can be validated. The locked users isn't supported by the provider.
    /// </summary>
    /// <param name="userName">The membership user whose lock status you want to clear.</param>
    /// <returns>
    /// true if the membership user was successfully unlocked; otherwise, false.
    /// </returns>
    public override bool UnlockUser(string userName)
    {
      if (AnonymousUser.UserIsNotAnonymous(userName))
      {
        return base.UnlockUser(userName);
      }
      return false;
    }

    /// <summary>
    /// Updates information about a user in the data source.
    /// </summary>
    /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser" /> object that represents the user to update and the updated information for the user.</param>
    public override void UpdateUser(MembershipUser user)
    {
      if (AnonymousUser.UserIsNotAnonymous(user.UserName))
      {
        base.UpdateUser(user);
      }
    }

    /// <summary>
    /// Verifies that the specified contact name and password exist in the CRM system.
    /// </summary>
    /// <param name="username">The name of the contact to validate.</param>
    /// <param name="password">The password for the specified contact.</param>
    /// <returns>
    /// true if the specified contact name and password are valid; otherwise, false.
    /// </returns>
    public override bool ValidateUser(string username, string password)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.ValidateUser(username, password);
      }
      return false;
    }

    #endregion
  }
}