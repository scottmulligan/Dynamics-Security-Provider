namespace CRMSecurityProvider.Caching
{
  using Sitecore.Caching;

  /// <summary>
  /// The cache service interface.
  /// </summary>
  public interface ICacheService
  {
    /// <summary>
    /// Gets the role cache.
    /// </summary>
    CrmCache RoleCache { get; }

    /// <summary>
    /// Gets the user cache.
    /// </summary>
    CrmCache UserCache { get; }

    /// <summary>
    /// Gets the role members cache.
    /// </summary>
    StringCache MembersCache { get; }

    /// <summary>
    /// Gets the user roles cache.
    /// </summary>
    StringCache MemberOfCache { get; }

    /// <summary>
    /// Gets the metadata cache.
    /// </summary>
    ICache MetadataCache { get; }
  }
}
