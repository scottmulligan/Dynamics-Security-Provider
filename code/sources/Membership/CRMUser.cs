namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Caching;
  using Sitecore.Reflection;

  using CRMSecurityProvider.Caching;

  /// <summary>
  /// Represents CRM contact as a user.
  /// </summary>
  public class CRMUser : ICrmCacheable
  {
    private const int SizeOfDatetime = 8;

    #region Events

    /// <summary>
    /// Data length changed.
    /// </summary>
    public event DataLengthChangedDelegate DataLengthChanged;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMUser"/> class.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="id">ID of the user.</param>
    /// <param name="email">Email of the user.</param>
    /// <param name="passwordQuestion">Password question of the user.</param>
    /// <param name="description">Description of the user.</param>
    /// <param name="isApproved">if set to <c>true</c> user is approved; otherwise, <c>false</c>.</param>
    /// <param name="isLockedOut">if set to <c>true</c> user is locked out; otherwise, <c>false</c>.</param>
    /// <param name="createdDate">The date when user was created.</param>
    /// <param name="lastLoginDate">The last login date of the user.</param>
    /// <param name="lastActivityDate">The last activity date of the user.</param>
    /// <param name="lastPasswordChangedDate">The last password changed date of the user.</param>
    /// <param name="lastLockoutDate">The last locked out date of the user.</param>
    public CRMUser(string name, Guid id, string email, string passwordQuestion, string description, bool isApproved, bool isLockedOut,
      DateTime createdDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
    {
      this.Name = name;
      this.ID = id;
      this.Email = email;
      this.PasswordQuestion = passwordQuestion;
      this.Description = description;
      this.IsApproved = isApproved;
      this.IsLockedOut = isLockedOut;
      this.CreatedDate = createdDate;
      this.LastLoginDate = lastLoginDate;
      this.LastActivityDate = lastActivityDate;
      this.LastPasswordChangedDate = lastPasswordChangedDate;
      this.LastLockoutDate = lastLockoutDate;

      this.Cacheable = true;
      this.Properties = new Dictionary<string, object>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    public Guid ID { get; protected set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; protected set; }

    /// <summary>
    /// Gets or sets the password question.
    /// </summary>
    public string PasswordQuestion { get; protected set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether user is approved.
    /// </summary>
    public bool IsApproved { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether user is locked out.
    /// </summary>
    public bool IsLockedOut { get; protected set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedDate { get; protected set; }

    /// <summary>
    /// Gets or sets the last login date.
    /// </summary>
    public DateTime LastLoginDate { get; protected set; }

    /// <summary>
    /// Gets or sets the last activity date.
    /// </summary>
    public DateTime LastActivityDate { get; protected set; }

    /// <summary>
    /// Gets or sets the last password changed date.
    /// </summary>
    public DateTime LastPasswordChangedDate { get; protected set; }

    /// <summary>
    /// Gets or sets the last lockout date.
    /// </summary>
    public DateTime LastLockoutDate { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is cacheable.
    /// </summary>
    /// <value><c>true</c> if cacheable; otherwise, <c>false</c>.</value>
    public bool Cacheable { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is immutable.
    /// </summary>
    /// <value><c>true</c> if immutable; otherwise, <c>false</c>.</value>
    public bool Immutable
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    protected IDictionary<string, object> Properties { get; set; }

    /// <summary>
    /// Gets the profile property names.
    /// </summary>
    public IEnumerable<string> ProfilePropertyNames
    {
      get
      {
        return this.Properties.Keys;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the property value.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    public void SetPropertyValue(string name, object value)
    {
      if (this.Properties.Keys.Contains(name))
      {
        this.Properties[name] = value;
      }
      else
      {
        this.Properties.Add(name, value);
      }

      this.InvokeDataLengthChanged();
    }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <returns>The value of the property.</returns>
    public object GetPropertyValue(string name)
    {
      if (this.Properties.ContainsKey(name))
      {
        return this.Properties[name];
      }

      return null;
    }

    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns>Size of the data to use when caching.</returns>
    public long GetDataLength()
    {
      long result = TypeUtil.SizeOfString(this.Name)
        + TypeUtil.SizeOfGuid()
        + TypeUtil.SizeOfString(this.Email)
        + TypeUtil.SizeOfString(this.PasswordQuestion)
        + TypeUtil.SizeOfString(this.Description)
        + sizeof (bool)
        + sizeof (bool)
        + SizeOfDatetime
        + SizeOfDatetime
        + SizeOfDatetime
        + SizeOfDatetime
        + SizeOfDatetime
        + this.Properties.Values.Sum(k => this.GetPropertyValueLength(k));

      return result;
    }

    /// <summary>
    /// Gets length of the value of the property.
    /// </summary>
    /// <param name="value">The value of the property.</param>
    /// <returns>Length of the value of the property.</returns>
    protected int GetPropertyValueLength(object value)
    {
      if (value is string)
      {
        return TypeUtil.SizeOfString((string)value);
      }

      if (value is bool)
      {
        return sizeof (bool);
      }

      if (value is DateTime)
      {
        return SizeOfDatetime;
      }

      if (value is float)
      {
        return sizeof (float);
      }

      if (value is decimal)
      {
        return sizeof (decimal);
      }

      if (value is int)
      {
        return sizeof (int);
      }

      return 0;
    }

    /// <summary>
    /// Invokes the data length changed event.
    /// </summary>
    protected void InvokeDataLengthChanged()
    {
      var handler = this.DataLengthChanged;
      if (handler != null)
      {
        handler(this);
      }
    }

        #endregion
    /// <summary>
    /// Provides consistency with standard entities.
    /// </summary>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public object this[string attributeName]
    {
        get
        {
            return this.GetPropertyValue(attributeName);
        }
        set
        {
            this.SetPropertyValue(attributeName, value);
        }
    }
  }
}
