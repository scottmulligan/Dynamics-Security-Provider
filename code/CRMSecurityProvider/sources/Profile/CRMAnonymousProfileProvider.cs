namespace CRMSecurityProvider
{
  using System.Configuration;
  using System.Linq;
  using CRMSecurityProvider.Sources;

  /// <summary>
  /// The anonymous profile provider
  /// </summary>
  public class CRMAnonymousProfileProvider : CRMProfileProvider
  {
    /// <summary>
    /// Deletes profile properties and information for profiles that match the supplied list of contact names.
    /// </summary>
    /// <param name="usernames">A string array of contact names for profiles to be deleted.</param>
    /// <returns>
    /// The number of profiles deleted from the CRM system.
    /// </returns>
    public override int DeleteProfiles(string[] usernames)
    {
      return base.DeleteProfiles(usernames.Where(AnonymousUser.UserIsNotAnonymous).ToArray());
    }

    /// <summary>
    /// Returns the collection of settings property values for the specified application instance and settings property group.
    /// </summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application use.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
    /// <returns>
    /// A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> containing the values for the specified settings property group.
    /// </returns>
    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
    {
      var userName = (string)context["UserName"];
      if (AnonymousUser.UserIsNotAnonymous(userName))
      {
        return base.GetPropertyValues(context, collection);
      }
      return new SettingsPropertyValueCollection();
    }

    /// <summary>
    /// Sets the values of the specified group of property settings.
    /// </summary>
    /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
    /// <param name="collection">A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> representing the group of property settings to set.</param>
    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
      var userName = (string)context["UserName"];
      if (AnonymousUser.UserIsNotAnonymous(userName))
      {
        base.SetPropertyValues(context, collection);
      }
    }
  }
}
