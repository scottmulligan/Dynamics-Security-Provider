// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.Factory
{
  using System.Configuration;
  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Repository.Factory;

  /// <summary>
  /// The entity repository factory.
  /// </summary>
  public class EntityRepositoryFactory : RepositoryFactory, IEntityRepositoryFactory
  {
    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>The entity repository.</returns>
    public EntityRepositoryBase GetRepository(CRMSecurityProvider.Configuration.ConfigurationSettings settings)
    {
      return this.Resolve<EntityRepositoryBase>(settings);
    }

    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <returns>The entity repository.</returns>
    public EntityRepositoryBase GetRepository()
    {
      var connectionStringSettings = ConfigurationManager.ConnectionStrings[Settings.ConnectionStringName];

      if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
      {
        return null;
      }

      var configurationSettings = CRMSecurityProvider.Configuration.ConfigurationSettings.Parse(connectionStringSettings.ConnectionString);
      return this.GetRepository(configurationSettings);
    }
  }
}