namespace CRMSecurityProvider.Configuration
{
  /// <summary>
  /// List of configuration keys.
  /// </summary>
  public static class ConfigKey
  {
    public const string UniqueKeyProperty = "Crm.UniqueKeyProperty";
    public const string RoleCacheSize = "Crm.Caching.RoleCacheSize";
    public const string RoleCacheLifetime = "Crm.Caching.RoleCacheLifetime";
    public const string UserCacheSize = "Crm.Caching.UserCacheSize";
    public const string UserCacheLifetime = "Crm.Caching.UserCacheLifetime";
    public const string MemberOfCacheSize = "Crm.Caching.MemberOfCacheSize";
    public const string MembersCacheSize = "Crm.Caching.MembersCacheSize";
    public const string MembersCacheLifetime = "Crm.Caching.MembersCacheLifetime";
    public const string MetadataCacheSize = "Crm.Caching.MetadataCacheSize";
    public const string FetchThrottlingPageSize = "Crm.FetchThrottlingPageSize";
    public const string CrmAccessProfiling = "Crm.CrmAccessProfiling";
    public const string LoggingLevel = "Crm.LoggingLevel";
    public const string ConnectionStringName = "Crm.ConnectionStringName";
  }
}
