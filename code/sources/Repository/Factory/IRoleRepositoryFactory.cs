namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// Role repository factory interface.
  /// </summary>
  public interface IRoleRepositoryFactory
  {
    /// <summary>
    /// Gets role repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The role repository.</returns>
    RoleRepositoryBase GetRepository(ConfigurationSettings settings);
  }
}
