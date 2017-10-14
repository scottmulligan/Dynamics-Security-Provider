namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// Profile repository factory class.
  /// </summary>
  public class ProfileRepositoryFactory : RepositoryFactory, IProfileRepositoryFactory
  {
    /// <summary>
    /// Gets profile repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The profile repository.</returns>
    public ProfileRepositoryBase GetRepository(ConfigurationSettings settings)
    {
      return this.Resolve<ProfileRepositoryBase>(settings);
    }
  }
}
