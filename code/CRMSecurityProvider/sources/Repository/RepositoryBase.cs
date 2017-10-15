namespace CRMSecurityProvider.Repository
{
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;

  /// <summary>
  /// Repository base class.
  /// </summary>
  public abstract class RepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    protected RepositoryBase(ICacheService cacheService)
    {
      Assert.ArgumentNotNull(cacheService, "cacheService");

      this.CacheService = cacheService;
    }

    /// <summary>
    /// Gets or sets the cache service.
    /// </summary>
    protected ICacheService CacheService { get; private set; }
  }
}
