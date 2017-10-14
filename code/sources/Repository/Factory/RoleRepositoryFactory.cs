namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// Role repository factory class.
  /// </summary>
  public class RoleRepositoryFactory : RepositoryFactory, IRoleRepositoryFactory
  {
    /// <summary>
    /// Gets role repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The role repository.</returns>
    public RoleRepositoryBase GetRepository(ConfigurationSettings settings)
    {
      return this.Resolve<RoleRepositoryBase>(settings);
    }
  }
}
