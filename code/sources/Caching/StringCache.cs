namespace CRMSecurityProvider.Caching
{
  using System;

  using Sitecore.Caching;
  using Sitecore.Reflection;

  /// <summary>
  /// Defines the members cache class.
  /// </summary>
  public class StringCache : CustomCache
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringCache"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="maxSize">The max size.</param>
    /// <param name="expirationSpan">The expiration span.</param>
    public StringCache(string name, long maxSize, TimeSpan expirationSpan)
      : base(name, maxSize)
    {
      this.ExpirationSpan = expirationSpan;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the expiration span.
    /// </summary>
    protected TimeSpan ExpirationSpan { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the specified string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The string.</param>
    public void Add(string key, string value)
    {
      this.InnerCache.Add(key, value, DateTime.UtcNow.Add(this.ExpirationSpan));
    }

    /// <summary>
    /// Gets the specified string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The string.</returns>
    public string Get(string key)
    {
      return (string)this.InnerCache[key];
    }

    /// <summary>
    /// Removes the specified string.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Remove(string key)
    {
      this.InnerCache.Remove(key);
    }

    #endregion
  }
}
