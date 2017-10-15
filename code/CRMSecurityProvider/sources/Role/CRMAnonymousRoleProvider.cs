namespace CRMSecurityProvider
{
  using System;
  using System.Linq;
  using CRMSecurityProvider.Sources;
  using CRMSecurityProvider.Sources.Configuration;

  /// <summary>
  ///   The crm Anonymous role provider
  /// </summary>
  public class CRMAnonymousRoleProvider : CRMRoleProvider
  {
    #region Public methods

    /// <summary>
    /// Adds the specified user names to the specified roles for the configured applicationName.
    /// </summary>
    /// <param name="userNames">A string array of user names to be added to the specified roles.</param>
    /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
    public override void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      base.AddUsersToRoles(userNames.Where(AnonymousUser.UserIsNotAnonymous).ToArray(), roleNames);
    }

    /// <summary>
    /// Gets a list of the roles that a specified user is in for the configured applicationName.
    /// </summary>
    /// <param name="userName">The user to return a list of roles for.</param>
    /// <returns>
    /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
    /// </returns>
    public override string[] GetRolesForUser(string userName)
    {
      if (AnonymousUser.UserIsNotAnonymous(userName))
      {
        return base.GetRolesForUser(userName);
      }
      return EnumerableConstants.EmptyStrings;
    }

    /// <summary>
    /// Gets a list of contacts in the specified marketing list.
    /// </summary>
    /// <param name="roleName">The name of the marketing list to get the list of contacts for.</param>
    /// <returns>
    /// A string array containing the names of all the contacts who are members of the specified marketing list.
    /// </returns>
    public override string[] GetUsersInRole(string roleName)
    {
      return base.GetUsersInRole(roleName).Where(AnonymousUser.UserIsNotAnonymous).ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
    /// </summary>
    /// <param name="username">The user name to search for.</param>
    /// <param name="roleName">The role to search in.</param>
    /// <returns>
    /// true if the specified user is in the specified role for the configured applicationName; otherwise, false.
    /// </returns>
    public override bool IsUserInRole(string username, string roleName)
    {
      if (AnonymousUser.UserIsNotAnonymous(username))
      {
        return base.IsUserInRole(username, roleName);
      }
      return false;
    }

    /// <summary>
    /// Removes the specified user names from the specified roles for the configured applicationName.
    /// </summary>
    /// <param name="userNames">A string array of user names to be removed from the specified roles.</param>
    /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
    public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      base.RemoveUsersFromRoles(userNames.Where(AnonymousUser.UserIsNotAnonymous).ToArray(), roleNames);
    }

    /// <summary>
    /// Gets an array of user names in a role where the user name contains the specified user name to match.
    /// </summary>
    /// <param name="roleName">The role to search in.</param>
    /// <param name="usernameToMatch">The user name to search for.</param>
    /// <returns>
    /// A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch" /> and the user is a member of the specified role.
    /// </returns>
    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      return base.FindUsersInRole(roleName, usernameToMatch).Where(AnonymousUser.UserIsNotAnonymous).ToArray();
    }



    #endregion
  }
}