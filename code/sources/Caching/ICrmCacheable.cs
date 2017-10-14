namespace CRMSecurityProvider.Caching
{
  using System;

  using Sitecore.Caching;

  /// <summary>
  /// Defines the basic properties and functionality of an CRM object that is cacheable.
  /// </summary>
  public interface ICrmCacheable : ICacheable
  {
    /// <summary>
    /// Gets ID of the object.
    /// </summary>
    Guid ID { get; }

    /// <summary>
    /// Gets the name of the object.
    /// </summary>
    string Name { get; }
  }
}
