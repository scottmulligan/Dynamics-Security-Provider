// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepositoryFactory.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the IEntityRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.Factory
{
  using CRMSecurityProvider.Configuration;

  /// <summary>
  /// The EntityRepositoryFactory interface.
  /// </summary>
  public interface IEntityRepositoryFactory
  {
    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>The entity repository.</returns>
    EntityRepositoryBase GetRepository(ConfigurationSettings settings);

    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <returns>The entity repository.</returns>
    EntityRepositoryBase GetRepository();
  }
}
