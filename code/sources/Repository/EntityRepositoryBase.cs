// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityRepositoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository
{
  using System;
  using System.Collections.Generic;
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Repository;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.PagingInfo;

  // ReSharper disable UnusedMember.Global
  // Handled by unity.

  /// <summary>
  /// The entity repository base.
  /// </summary>
  public abstract class EntityRepositoryBase : RepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepositoryBase"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    protected EntityRepositoryBase(ICacheService cacheService)
      : base(cacheService)
    {
    }

    /// <summary>
    /// Gets all entities metadata.
    /// </summary>
    /// <returns>Crm entity metadata array</returns>
    public abstract CrmEntityMetadata[] GetAllEntitiesMetadata();

    /// <summary>
    /// Gets the entity metadata.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <returns>Crm entity metadata</returns>
    public abstract CrmEntityMetadata GetEntityMetadata(string logicalName);

    /// <summary>
    /// Gets the attribute metadata.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns>Crm attribute metadata</returns>
    public abstract ICrmAttributeMetadata GetAttributeMetadata(string entityName, string attributeName);

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="pagingInfo">The paging info.</param>
    /// <param name="orderExpressions">The order expressions.</param>
    /// <param name="onlyActive">The only Active.</param>
    /// <returns>Crm entities array</returns>
    public abstract ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions, bool onlyActive);

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="pagingInfo">The paging information.</param>
    /// <param name="orderExpressions">The order expressions.</param>
    /// <param name="filteredFields">The filtered fields.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns></returns>
    public abstract ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions,
      Dictionary<string, string> filteredFields, bool onlyActive);

    /// <summary>
    /// Gets the entity.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="value">The value.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <param name="columns">The columns.</param>
    /// <returns>Crm entity</returns>
    public abstract ICrmEntity GetEntity(string logicalName, string fieldName, string value, bool onlyActive, string[] columns);

    /// <summary>
    /// Gets the entities count.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="primaryKey">The primary key.</param>
    /// <param name="filteredFields">The filtered fields.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns></returns>
    public abstract int GetEntitiesCount(string logicalName, string primaryKey,
      Dictionary<string, string> filteredFields, bool onlyActive);
    
    /// <summary>
    /// Gets the entities count.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="primaryKey">The primary key.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns>Entities count</returns>
    public abstract int GetEntitiesCount(string logicalName, string primaryKey, bool onlyActive);

    /// <summary>
    /// Creates new entity.
    /// </summary>
    /// <param name="logicalName">Entity logical name.</param>
    /// <returns>Crm entity</returns>
    public abstract ICrmEntity NewEntity(string logicalName);

    /// <summary>
    /// Creates the specified CRM entity.
    /// </summary>
    /// <param name="crmEntity">The CRM entity.</param>
    /// <returns>Inserted entity key</returns>
    public abstract Guid Insert(ICrmEntity crmEntity);

    /// <summary>
    /// Updates the specified CRM entity.
    /// </summary>
    /// <param name="crmEntity">The CRM entity.</param>
    public abstract void Update(ICrmEntity crmEntity);

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="id">The GUID.</param>
    public abstract void Delete(string entityName, Guid id);

  }
  // ReSharper restore UnusedMember.Global
}
