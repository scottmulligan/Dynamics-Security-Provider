namespace CRMSecurityProvider.Configuration
{
  using System;
  using Sitecore;

  using CRMSecurityProvider.Utils;

  using ScConfig = Sitecore.Configuration.Settings;

  /// <summary>
  /// Settings class.
  /// </summary>
  public static class Settings
  {
    /// <summary>
    /// Gets unique key property.
    /// </summary>
    public static string UniqueKeyProperty
    {
      get
      {
        return ScConfig.GetSetting(ConfigKey.UniqueKeyProperty, "emailaddress1");
      }
    }

    /// <summary>
    /// Gets role cache size.
    /// </summary>
    public static long RoleCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(ScConfig.GetSetting(ConfigKey.RoleCacheSize, "2MB"));
      }
    }

    /// <summary>
    /// Gets role cache lifetime.
    /// </summary>
    public static TimeSpan RoleCacheLifetime
    {
      get
      {
        return ScConfig.GetTimeSpanSetting(ConfigKey.RoleCacheLifetime, TimeSpan.FromMinutes(2));
      }
    }

    /// <summary>
    /// Gets user cache size.
    /// </summary>
    public static long UserCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(ScConfig.GetSetting(ConfigKey.UserCacheSize, "2MB"));
      }
    }

    /// <summary>
    /// Gets user cache lifetime.
    /// </summary>
    public static TimeSpan UserCacheLifetime
    {
      get
      {
        return ScConfig.GetTimeSpanSetting(ConfigKey.UserCacheLifetime, TimeSpan.FromMinutes(2));
      }
    }

    /// <summary>
    /// Gets member of cache size.
    /// </summary>
    public static long MemberOfCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(ScConfig.GetSetting(ConfigKey.MemberOfCacheSize, "2MB"));
      }
    }

    /// <summary>
    /// Gets members cache size.
    /// </summary>
    public static long MembersCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(ScConfig.GetSetting(ConfigKey.MembersCacheSize, "2MB"));
      }
    }

    /// <summary>
    /// Gets members cache lifetime.
    /// </summary>
    public static TimeSpan MembersCacheLifetime
    {
      get
      {
        return ScConfig.GetTimeSpanSetting(ConfigKey.MembersCacheLifetime, TimeSpan.FromHours(1));
      }
    }

    /// <summary>
    /// Gets metadata cache size.
    /// </summary>
    public static long MetadataCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(ScConfig.GetSetting(ConfigKey.MetadataCacheSize, "2MB"));
      }
    }

    /// <summary>
    /// Gets the size of the fetch throttling page.
    /// </summary>
    public static int FetchThrottlingPageSize
    {
      get
      {
        return ScConfig.GetIntSetting(ConfigKey.FetchThrottlingPageSize, 5000);
      }
    }

    /// <summary>
    /// Gets CRM access profiling flag.
    /// </summary>
    public static bool CrmAccessProfiling
    {
      get
      {
        return ScConfig.GetBoolSetting(ConfigKey.CrmAccessProfiling, false);
      }
    }

    /// <summary>
    /// Gets the logging level.
    /// </summary>
    public static LoggingLevel LoggingLevel
    {
      get
      {
        switch (ScConfig.GetSetting(ConfigKey.LoggingLevel))
        {
          case "None":
            return LoggingLevel.None;
          case "Error":
            return LoggingLevel.Error;
          case "Warning":
            return LoggingLevel.Warning;
          case "Info":
            return LoggingLevel.Info;
          case "Details":
            return LoggingLevel.Details;
        }

        return LoggingLevel.Warning;
      }
    }

    /// <summary>
    /// Gets CRM connection string name.
    /// </summary>
    public static string ConnectionStringName
    {
      get
      {
        return ScConfig.GetSetting(ConfigKey.ConnectionStringName);
      }
    }
  }
}
