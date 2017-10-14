namespace CRMSecurityProvider.Repository.Factory
{
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.Repository.V3;
  using CRMSecurityProvider.Repository.V4;
  using CRMSecurityProvider.Repository.V5;
  using CRMSecurityProvider.Sources.Repository;
  using CRMSecurityProvider.Sources.Repository.V5;
  using Microsoft.Practices.Unity;

  /// <summary>
  /// Repository factory class.
  /// </summary>
  public abstract class RepositoryFactory
  {
    private static IUnityContainer unityContainer;
    private static readonly object locker = new object();

    /// <summary>
    /// Resolve an instance of the default requested type from the container.
    /// </summary>
    /// <typeparam name="T"><see cref="T:System.Type"/> of object to get from the container.</typeparam>
    /// <param name="settings">The configuration settings.</param>
    /// <returns>The retrieved object.</returns>
    protected T Resolve<T>(ConfigurationSettings settings)
    {
      if (unityContainer == null)
      {
        lock (locker)
        {
          if (unityContainer == null)
          {
            var container = new UnityContainer();
            switch (settings.ApiVersion)
            {
              case ApiVersion.V3:
                var crmServiceCreatorV3 = new CrmServiceCreatorV3();
                container.RegisterInstance(crmServiceCreatorV3.CreateService(settings), new ContainerControlledLifetimeManager())
                  .RegisterInstance(crmServiceCreatorV3.CreateMetadataService(settings), new ContainerControlledLifetimeManager())
                  .RegisterType<ICacheService, CacheService>(new ContainerControlledLifetimeManager())
                  .RegisterType<IContactToUserConverterV3, ContactToUserConverterV3>(new ContainerControlledLifetimeManager())
                  .RegisterType<IMarketingListToRoleConverterV3, MarketingListToRoleConverterV3>(new ContainerControlledLifetimeManager())
                  .RegisterType<UserRepositoryBase, UserRepositoryV3>(new ContainerControlledLifetimeManager())
                  .RegisterType<RoleRepositoryBase, RoleRepositoryV3>(new ContainerControlledLifetimeManager())
                  .RegisterType<ProfileRepositoryBase, ProfileRepositoryV3>(new ContainerControlledLifetimeManager())
                  .RegisterType<EntityRepositoryBase, Sources.Repository.V3.EntityRepository>(new ContainerControlledLifetimeManager());

                break;
              case ApiVersion.V4:
                var crmServiceCreator4 = new CrmServiceCreatorV4();
                container.RegisterInstance(crmServiceCreator4.CreateService(settings), new ContainerControlledLifetimeManager())
                  .RegisterInstance(crmServiceCreator4.CreateMetadataService(settings), new ContainerControlledLifetimeManager())
                  .RegisterType<ICacheService, CacheService>(new ContainerControlledLifetimeManager())
                  .RegisterType<IContactToUserConverterV4, ContactToUserConverterV4>(new ContainerControlledLifetimeManager())
                  .RegisterType<IMarketingListToRoleConverterV4, MarketingListToRoleConverterV4>(new ContainerControlledLifetimeManager())
                  .RegisterType<UserRepositoryBase, UserRepositoryV4>(new ContainerControlledLifetimeManager())
                  .RegisterType<RoleRepositoryBase, RoleRepositoryV4>(new ContainerControlledLifetimeManager())
                  .RegisterType<ProfileRepositoryBase, ProfileRepositoryV4>(new ContainerControlledLifetimeManager())
                  .RegisterType<EntityRepositoryBase, Sources.Repository.V4.EntityRepository>(new ContainerControlledLifetimeManager());
                break;
              case ApiVersion.V5:
                var crmServiceCreatorV5 = new CrmServiceCreatorV5();
                container.RegisterInstance(new OrganizationServiceCacheV5(crmServiceCreatorV5, settings), new ContainerControlledLifetimeManager())
                  .RegisterType<ICacheService, CacheService>(new ContainerControlledLifetimeManager())
                  .RegisterType<IContactToUserConverterV5, ContactToUserConverterV5>(new ContainerControlledLifetimeManager())
                  .RegisterType<IMarketingListToRoleConverterV5, MarketingListToRoleConverterV5>(new ContainerControlledLifetimeManager())
                  .RegisterType<UserRepositoryBase, UserRepositoryV5>(new ContainerControlledLifetimeManager())
                  .RegisterType<RoleRepositoryBase, RoleRepositoryV5>(new ContainerControlledLifetimeManager())
                  .RegisterType<ProfileRepositoryBase, ProfileRepositoryV5>(new ContainerControlledLifetimeManager())
                  .RegisterType<EntityRepositoryBase, EntityRepository>(new ContainerControlledLifetimeManager());
                break;
            }

            unityContainer = container;
          }
        }
      }

      return unityContainer.Resolve<T>();
    }
  }
}
