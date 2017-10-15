namespace CRMSecurityProvider.Sources
{
  using System;

  public class AnonymousUser
  {
    #region Fields

    /// <summary>
    ///   The Anonymous
    /// </summary>
    public static readonly string Name = "Anonymous";

    #endregion

    #region Public methods

    /// <summary>
    /// Users the is not Anonymous.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns></returns>
    public static bool UserIsNotAnonymous(string userName)
    {
      return string.Compare(userName, Name, StringComparison.InvariantCultureIgnoreCase) != 0;
    }

    #endregion
  }
}