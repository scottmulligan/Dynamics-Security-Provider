namespace CRMSecurityProvider.Caching
{
  using System;

  using Sitecore.Caching;
  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// The cache service class.
  /// </summary>
  public class CacheService : ICacheService
  {
    #region Fields

    /// <summary>
    /// The role cache.
    /// </summary>
    private CrmCache roleCache;

    /// <summary>
    /// The user cache.
    /// </summary>
    private CrmCache userCache;

    /// <summary>
    /// The role members cache.
    /// </summary>
    private StringCache membersCache;

    /// <summary>
    /// The user roles cache.
    /// </summary>
    private StringCache memberOfCache;

    /// <summary>
    /// The metadata cache.
    /// </summary>
    private ICache metadataCache;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheService"/> class.
    /// </summary>
    /// <param name="configurationSettings">The configuration settings.</param>
    public CacheService(ConfigurationSettings configurationSettings)
    {
      Assert.ArgumentNotNull(configurationSettings, "configurationSettings");

      this.ConfigurationSettings = configurationSettings;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the configuration settings.
    /// </summary>
    protected ConfigurationSettings ConfigurationSettings { get; private set; }

    /// <summary>
    /// Gets the role cache.
    /// </summary>
    public CrmCache RoleCache
    {
      get
      {
        var roleCacheName = String.Join(":", new[] { "CRM[Roles]", this.ConfigurationSettings.User, this.ConfigurationSettings.Organization });
        return this.roleCache ?? (this.roleCache = new CrmCache(roleCacheName, Settings.RoleCacheSize, Settings.RoleCacheLifetime));
      }
    }

    /// <summary>
    /// Gets the user cache.
    /// </summary>
    public CrmCache UserCache
    {
      get
      {
        var userCacheName = String.Join(":", new[] { "CRM[Users]", this.ConfigurationSettings.User, this.ConfigurationSettings.Organization });
        return this.userCache ?? (this.userCache = new CrmCache(userCacheName, Settings.UserCacheSize, Settings.UserCacheLifetime));
      }
    }

    /// <summary>
    /// Gets the role members cache.
    /// </summary>
    public StringCache MembersCache
    {
      get
      {
        var membersCacheName = String.Join(":", new[] { "CRM[Members]", this.ConfigurationSettings.User, this.ConfigurationSettings.Organization });
        return this.membersCache ?? (this.membersCache = new StringCache(membersCacheName, Settings.MembersCacheSize, Settings.MembersCacheLifetime));
      }
    }

    /// <summary>
    /// Gets the user roles cache.
    /// </summary>
    public StringCache MemberOfCache
    {
      get
      {
        var memberOfCacheName = String.Join(":", new[] { "CRM[MemberOf]", this.ConfigurationSettings.User, this.ConfigurationSettings.Organization });
        return this.memberOfCache ?? (this.memberOfCache = new StringCache(memberOfCacheName, Settings.MemberOfCacheSize, Settings.MembersCacheLifetime));
      }
    }

    /// <summary>
    /// Gets the metadata cache.
    /// </summary>
    public ICache MetadataCache
    {
      get
      {
        var metadataCacheName = String.Join(":", new[] { "CRM[Metadata]", this.ConfigurationSettings.User, this.ConfigurationSettings.Organization });
        return this.metadataCache ?? (this.metadataCache = CacheManager.GetNamedInstance(metadataCacheName, Configuration.Settings.MetadataCacheSize, true));
      }
    }

    #endregion
  }
}
