namespace CRMSecurityProvider
{
  using Sitecore.Caching;

  /// <summary>
  /// The contact attribute class.
  /// </summary>
  public class ContactAttribute : ICacheable
  {
    private bool cacheable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactAttribute"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public ContactAttribute(SupportedTypes type)
    {
      cacheable = true;
      Type = type;
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public SupportedTypes Type
    { get; set; }

    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns/>
    public virtual long GetDataLength()
    {
      return sizeof(byte);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is cacheable.
    /// </summary>
    /// <value>
    /// <c>true</c> if cacheable; otherwise, <c>false</c>.
    /// </value>
    public bool Cacheable
    {
      get
      {
        return cacheable;
      }
      set
      {
        cacheable = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is immutable.
    /// </summary>
    /// <value>
    /// <c>true</c> if immutable; otherwise, <c>false</c>.
    /// </value>
    public bool Immutable
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Data length changed event.
    /// </summary>
    public event DataLengthChangedDelegate DataLengthChanged;

    protected virtual void OnDataLengthChanged(ICacheable obj)
    {
      DataLengthChangedDelegate handler = this.DataLengthChanged;
      if (handler != null)
      {
        handler(obj);
      }
    }
  }
}
