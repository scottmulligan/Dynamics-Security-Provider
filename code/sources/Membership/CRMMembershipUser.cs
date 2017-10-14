namespace CRMSecurityProvider
{
  /// <summary>
  /// Exposes and updates membership user information in the membership data store.
  /// </summary>
  public class CRMMembershipUser : System.Web.Security.MembershipUser
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CRMMembershipUser"/> class.
    /// </summary>
    /// <param name="providerName">The provider name.</param>
    /// <param name="user">The user.</param>
    public CRMMembershipUser(string providerName, CRMUser user)
      : base(providerName, user.Name, user.ID, user.Email, user.PasswordQuestion, user.Description, user.IsApproved, user.IsLockedOut, user.CreatedDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate)
    {
    }

    /// <summary>
    /// Gets or sets the e-mail address for the membership user.
    /// </summary>
    public override string Email
    {
      get
      {
        return base.Email;
      }
      set
      {
        if (base.Email != value)
        {
          this.EmailModified = true;
        }
        base.Email = value;
      }
    }

    public bool EmailModified { get; private set; }

    /// <summary>
    /// Gets or sets application-specific information for the membership user.
    /// </summary>
    public override string Comment
    {
      get
      {
        return base.Comment;
      }
      set
      {
        if (base.Comment != value)
        {
          this.CommentModified = true;
        }
        base.Comment = value;
      }
    }

    public bool CommentModified { get; private set; }
  }
}
