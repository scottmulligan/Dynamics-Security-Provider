// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepository.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.PagingInfo;
  using CRMSecurityProvider.Sources.Repository;
  using CRMSecurityProvider.Sources.Repository.V5.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Repository.V5.Entity;
  using CRMSecurityProvider.Sources.Repository.V5.Extensions;
  using CRMSecurityProvider.Utils;
  using Microsoft.Crm.Sdk.Messages;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Messages;
  using Microsoft.Xrm.Sdk.Metadata;
  using Microsoft.Xrm.Sdk.Query;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;

  /// <summary>
  /// The entity repository.
  /// </summary>
  public class EntityRepository : EntityRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepository"/> class.
    /// </summary>
    /// <param name="organizationServiceCache"></param>
    /// <param name="cacheService">The cache service.</param>
    public EntityRepository(OrganizationServiceCacheV5 organizationServiceCache, ICacheService cacheService)
      : base(cacheService)
    {
      Assert.ArgumentNotNull(organizationServiceCache, "organizationServiceCache");
      this.organizationServiceCache = organizationServiceCache;
    }

    private readonly OrganizationServiceCacheV5 organizationServiceCache;

    /// <summary>
    /// Gets all entities metadata.
    /// </summary>
    /// <returns>
    /// The <see cref="CrmEntityMetadata"/> array.
    /// </returns>
    public override CrmEntityMetadata[] GetAllEntitiesMetadata()
    {
      var retrieveAllEntitiesRequest = new RetrieveAllEntitiesRequest
      {
        EntityFilters = EntityFilters.Entity
      };

      var response = (RetrieveAllEntitiesResponse)this.organizationServiceCache.GetOrganizationService().Execute(retrieveAllEntitiesRequest);
      return response.EntityMetadata.Select(e => new CrmEntityMetadataAdapter(e)).ToArray();
    }

    /// <summary>
    /// Gets the entity metadata.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <returns>The <see cref="CrmEntityMetadata"/></returns>
    public override CrmEntityMetadata GetEntityMetadata(string logicalName)
    {
      var retrieveEntityRequest = new RetrieveEntityRequest
      {
        EntityFilters = EntityFilters.All,
        LogicalName = logicalName
      };

      var response = (RetrieveEntityResponse)this.organizationServiceCache.GetOrganizationService().Execute(retrieveEntityRequest);

      return new CrmEntityMetadataAdapter(response.EntityMetadata);
    }

    /// <summary>
    /// Gets the attribute metadata.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns>
    /// Crm attribute metadata
    /// </returns>
    public override ICrmAttributeMetadata GetAttributeMetadata(string entityName, string attributeName)
    {
      var crmAttributeMetadataFactory = new CrmAttributeMetadataFactory();

      var attributeResponse = (RetrieveAttributeResponse)this.organizationServiceCache.GetOrganizationService().Execute(new RetrieveAttributeRequest
      {
        EntityLogicalName = entityName,
        LogicalName = attributeName,
        RetrieveAsIfPublished = true
      });

      return crmAttributeMetadataFactory.Create(attributeResponse.AttributeMetadata);
    }

    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    /// <param name="filteredFields">The filtered fields.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns></returns>
    private FilterExpression GetFilterExpression(Dictionary<string, string> filteredFields, bool onlyActive)
    {
      var innerFilters = new FilterExpression();
      if (filteredFields != null)
      {
        foreach (var filteredField in filteredFields)
        {
          innerFilters.AddCondition(
            new ConditionExpression(filteredField.Key, ConditionOperator.Like, string.Format("%{0}%", filteredField.Value)));
        }
        innerFilters.FilterOperator = LogicalOperator.Or;
      }
      if (onlyActive)
      {
        var filterExpression = new FilterExpression();
        if (innerFilters.Conditions != null && innerFilters.Conditions.Count > 0)
        {
          filterExpression.Filters.Add(innerFilters);
          filterExpression.FilterOperator = LogicalOperator.And;
          filterExpression.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));
        }
        return filterExpression;
      }
      return innerFilters;
    }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="pagingInfo">The paging info.</param>
    /// <param name="orderExpressions">The order expressions.</param>
    /// <param name="onlyActive">The only Active.</param>
    /// <returns>
    /// The <see cref="CrmEntityMetadata" /> array.
    /// </returns>
    public override ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions, bool onlyActive)
    {
      return GetEntities(logicalName, pagingInfo, orderExpressions, null, onlyActive);
    }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="pagingInfo">The paging information.</param>
    /// <param name="orderExpressions">The order expressions.</param>
    /// <param name="filteredFields">The filtered fields.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns></returns>
    public override ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions, Dictionary<string, string> filteredFields, bool onlyActive)
    {

      var queryExpression = new QueryExpression
      {
        EntityName = logicalName,
        ColumnSet = new ColumnSet(true),
        PageInfo = pagingInfo,
        Criteria = this.GetFilterExpression(filteredFields, onlyActive)
      };

      queryExpression.Orders.AddRange(orderExpressions.Cast<OrderExpression>());

      var response = this.organizationServiceCache.GetOrganizationService().RetrieveMultiple(queryExpression);

      return response.Entities.Select(e => new CrmEntityAdapter(this, e)).ToArray();
    }

    /// <summary>
    /// Gets the entities count.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="primaryKey">The primary key.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns>
    /// The number of entities.
    /// </returns>
    public override int GetEntitiesCount(string logicalName, string primaryKey, bool onlyActive)
    {
      return GetEntitiesCount(logicalName, primaryKey, null, onlyActive);
    }

    /// <summary>
    /// Gets the entities count.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="primaryKey">The primary key.</param>
    /// <param name="filteredFields">The filtered fields.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <returns></returns>
    public override int GetEntitiesCount(string logicalName, string primaryKey, Dictionary<string, string> filteredFields, bool onlyActive)
    {

      int count = 0;
      var filterExpression = this.GetFilterExpression(filteredFields, onlyActive);

      try
      {

        var request = "<fetch mapping='logical' aggregate='true'><entity name='{0}'><attribute name='{1}' aggregate='count' alias='count' />{2}</entity></fetch>".FormatWith(logicalName, primaryKey, filterExpression.ToFetchXml());

        var fetchExpression = new FetchExpression(request);

        var result = this.organizationServiceCache.GetOrganizationService().RetrieveMultiple(fetchExpression);
        if (result.Entities.Count == 0)
        {
          return 0;
        }

        var entity = result.Entities[0];
        count = (int)((AliasedValue)entity["count"]).Value;
      }
      catch (Exception e)
      {
        if (e.Message.Contains("AggregateQueryRecordLimit"))
        {
          count = this.GetImplicitEntitiesCount(logicalName, filterExpression);
        }
      }

      return count;
    }

    /// <summary>
    /// Gets the implicit entities count. When explicit count can't be retrieved due to AggregateQueryRecordLimit setting.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="filterExpression">The filter expression.</param>
    /// <returns>The number of entities.</returns>
    private int GetImplicitEntitiesCount(string logicalName, FilterExpression filterExpression)
    {
      var entityCollection = this.organizationServiceCache.GetOrganizationService().RetrieveMultiple(
            new QueryExpression
            {
              EntityName = logicalName,
              Criteria = filterExpression,
              PageInfo = new PagingInfo
              {
                PageNumber = 1,
                Count = 1,
                ReturnTotalRecordCount = true
              }
            });

      if (entityCollection.TotalRecordCountLimitExceeded)
      {
        ConditionalLog.Error(
              string.Format(
                "The number of '{0}' entities is exceeding AggregrateQueryRecordLimit. Only first {1} can be shown.",
                logicalName,
                entityCollection.TotalRecordCount),
              this);
      }

      return entityCollection.TotalRecordCount;
    }

    /// <summary>
    /// Creates new entity.
    /// </summary>
    /// <param name="logicalName">Entity logical name.</param>
    public override ICrmEntity NewEntity(string logicalName)
    {
      return new CrmEntityAdapter(this, new Microsoft.Xrm.Sdk.Entity(logicalName));
    }

    /// <summary>
    /// Creates the specified CRM entity.
    /// </summary>
    /// <param name="crmEntity">The CRM entity.</param>
    public override Guid Insert(ICrmEntity crmEntity)
    {
      var entityAdapter = crmEntity as CrmEntityAdapter;
      if (entityAdapter == null)
      {
        return Guid.Empty;
      }

      var systemProperties = entityAdapter.AttributeCollectionAdapter.StripSystem();

      var createdEntity = entityAdapter.Id = this.organizationServiceCache.GetOrganizationService().Create(entityAdapter.Adaptee);

      entityAdapter.AttributeCollectionAdapter.AddRange(systemProperties);

      if (entityAdapter.IsStateChanged || entityAdapter.IsStatusChanged)
      {
        this.UpdateStateStatus(entityAdapter.LogicalName, createdEntity, entityAdapter.State, entityAdapter.Status);
      }

      return createdEntity;
    }

    /// <summary>
    /// Updates the specified CRM entity.
    /// </summary>
    /// <param name="crmEntity">The CRM entity.</param>
    public override void Update(ICrmEntity crmEntity)
    {
      var entityAdapter = crmEntity as CrmEntityAdapter;
      if (entityAdapter == null)
      {
        return;
      }

      var systemProperties = entityAdapter.AttributeCollectionAdapter.StripSystem();

      this.organizationServiceCache.GetOrganizationService().Update(entityAdapter.Adaptee);

      entityAdapter.AttributeCollectionAdapter.AddRange(systemProperties);

      if (entityAdapter.IsStateChanged || entityAdapter.IsStatusChanged)
      {
        this.UpdateStateStatus(entityAdapter.LogicalName, entityAdapter.Id, entityAdapter.State, entityAdapter.Status);
      }
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="id">The GUID.</param>
    public override void Delete(string entityName, Guid id)
    {
      this.organizationServiceCache.GetOrganizationService().Delete(entityName, id);
    }

    /// <summary>
    /// Gets the entity.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="value">The value.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <param name="columns">The columns.</param>
    /// <returns>The <see cref="ICrmEntity"/>.</returns>
    public override ICrmEntity GetEntity(string logicalName, string fieldName, string value, bool onlyActive, string[] columns)
    {
      var filterExpression = new FilterExpression { FilterOperator = LogicalOperator.And };
      filterExpression.AddCondition(new ConditionExpression(fieldName, ConditionOperator.Equal, new object[] { value }));
      if (onlyActive)
      {
        filterExpression.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, new object[] { "Active" }));
      }

      var retrieveMultipleReq = new RetrieveMultipleRequest
      {
        Query = new QueryExpression
           {
             ColumnSet = columns == null ? new ColumnSet(true) : new ColumnSet(columns),
             EntityName = logicalName,
             PageInfo = new PagingInfo { Count = 1, PageNumber = 1 },
             Criteria = filterExpression,
           },
      };

      var retrieveMultipleRes = (RetrieveMultipleResponse)this.organizationServiceCache.GetOrganizationService().Execute(retrieveMultipleReq);

      var entity = retrieveMultipleRes.EntityCollection.Entities.FirstOrDefault();

      return entity != null ? new CrmEntityAdapter(this, entity) : null;
    }

    /// <summary>
    /// Updates the state status.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="id">The id.</param>
    /// <param name="state">The state.</param>
    /// <param name="status">The status.</param>
    private void UpdateStateStatus(string logicalName, Guid id, ICrmKeyValueAttribute state, ICrmKeyValueAttribute status)
    {
      try
      {
        this.organizationServiceCache.GetOrganizationService().Execute(new SetStateRequest
        {
          EntityMoniker = new EntityReference(logicalName, id),
          State = new OptionSetValue(state.Key),
          Status = new OptionSetValue(status.Key)
        });
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't update entity state and status", e, this);

        if (!e.Message.Contains("is not a valid status code for state code"))
        {
          return;
        }

        ConditionalLog.Info("Trying to set state only", this);

        try
        {
          this.organizationServiceCache.GetOrganizationService().Execute(new SetStateRequest
          {
            EntityMoniker = new EntityReference(logicalName, id),
            State = new OptionSetValue(state.Key),
            Status = new OptionSetValue(-1)
          });
        }
        catch
        {
          ConditionalLog.Error("Couldn't update entity state", e, this);
        }
      }
    }
  }
}