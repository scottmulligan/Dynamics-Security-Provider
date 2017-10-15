namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// User repository factory class.
  /// </summary>
  public class UserRepositoryFactory : RepositoryFactory, IUserRepositoryFactory
  {
    /// <summary>
    /// Gets user repository.
    /// </summary>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The user repository.</returns>
    public UserRepositoryBase GetRepository(ConfigurationSettings settings)
    {
      return this.Resolve<UserRepositoryBase>(settings);
    }
  }
}
