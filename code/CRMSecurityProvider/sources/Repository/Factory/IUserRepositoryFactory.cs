namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// User repository factory interface.
  /// </summary>
  public interface IUserRepositoryFactory
  {
    /// <summary>
    /// Gets user repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The user repository.</returns>
    UserRepositoryBase GetRepository(ConfigurationSettings settings);
  }
}
