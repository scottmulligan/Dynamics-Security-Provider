namespace CRMSecurityProvider.Caching
{
  using System;
  
  using Sitecore.Caching;
  using Sitecore.Reflection;

  /// <summary>
  /// Defines the CRM cache class.
  /// </summary>
  public class CrmCache : CustomCache
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmCache"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="maxSize">The max size.</param>
    /// <param name="expirationSpan">The expiration span.</param>
    public CrmCache(string name, long maxSize, TimeSpan expirationSpan)
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
    /// Adds the specified object.
    /// </summary>
    /// <param name="item">The object.</param>
    public void Add(ICrmCacheable item)
    {
      var date = DateTime.UtcNow.Add(this.ExpirationSpan);

      this.InnerCache.Add(item.Name, item.ID, date);
      this.InnerCache.Add(item.ID.ToString(), item, date);
    }

    /// <summary>
    /// Gets the object.
    /// </summary>
    /// <param name="objectName">Name of the object.</param>
    /// <returns>The object.</returns>
    public ICrmCacheable Get(string objectName)
    {
      var objectId = this.InnerCache[objectName];
      if (objectId != null)
      {
        return this.Get((Guid)objectId);
      }

      return null;
    }

    /// <summary>
    /// Gets the object.
    /// </summary>
    /// <param name="objectId">ID of the object.</param>
    /// <returns>The object.</returns>
    public ICrmCacheable Get(Guid objectId)
    {
      return (ICrmCacheable)this.InnerCache[objectId.ToString()];
    }

    /// <summary>
    /// Removes the object.
    /// </summary>
    /// <param name="objectName">Name of the object.</param>
    public void Remove(string objectName)
    {
      var objectId = this.InnerCache[objectName];
      if (objectId != null)
      {
        this.Remove((Guid)objectId);
      }
    }

    /// <summary>
    /// Removes the object.
    /// </summary>
    /// <param name="objectId">ID of the object.</param>
    public void Remove(Guid objectId)
    {
      var item = this.Get(objectId);
      if (item != null)
      {
        this.InnerCache.Remove(item.ID.ToString());
        this.InnerCache.Remove(item.Name);
      }
    }

    #endregion
  }
}
