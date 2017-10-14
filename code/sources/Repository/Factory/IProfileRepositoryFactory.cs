namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// Profile repository factory interface.
  /// </summary>
  public interface IProfileRepositoryFactory
  {
    /// <summary>
    /// Gets profile repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The profile repository.</returns>
    ProfileRepositoryBase GetRepository(ConfigurationSettings settings);
  }
}
