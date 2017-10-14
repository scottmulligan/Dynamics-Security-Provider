// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepository.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.crm3.metadataservice;
  using CRMSecurityProvider.crm3.webservice;
  using CRMSecurityProvider.Repository.V3;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.PagingInfo;
  using CRMSecurityProvider.Sources.Repository;
  using CRMSecurityProvider.Sources.Repository.V3.Attribute.Metadata;
  using CRMSecurityProvider.Sources.Repository.V3.Entity;
  using CRMSecurityProvider.Sources.Repository.V3.Extensions;
  using CRMSecurityProvider.Utils;
  using Sitecore.StringExtensions;

  /// <summary>
  /// The entity repository.
  /// </summary>
  internal class EntityRepository : EntityRepositoryBase
  {
    /// <summary>
    /// The CRM metadata service
    /// </summary>
    private readonly IMetadataServiceV3 crmMetadataService;

    /// <summary>
    /// The CRM service
    /// </summary>
    private readonly ICrmServiceV3 crmService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepository"/> class.
    /// </summary>
    /// <param name="metadataService">The metadata service.</param>
    /// <param name="crmService">The CRM service.</param>
    /// <param name="cacheService">The cache service.</param>
    public EntityRepository(IMetadataServiceV3 metadataService, ICrmServiceV3 crmService, ICacheService cacheService)
      : base(cacheService)
    {
      this.crmMetadataService = metadataService;
      this.crmService = crmService;
    }

    /// <summary>
    /// Gets all entities metadata.
    /// </summary>
    public override CrmEntityMetadata[] GetAllEntitiesMetadata()
    {
      var metadata = this.crmMetadataService.RetrieveMetadata(MetadataFlags.EntitiesOnly);
      return metadata.Entities.Select(m => new CrmEntityMetadataAdapter(m)).ToArray();
    }

    /// <summary>
    /// Gets the entity metadata.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    public override CrmEntityMetadata GetEntityMetadata(string logicalName)
    {
      var metadata = this.crmMetadataService.RetrieveEntityMetadata(logicalName, EntityFlags.IncludeAttributes | EntityFlags.IncludeRelationships);
      return new CrmEntityMetadataAdapter(metadata);
    }

    /// <summary>
    /// Gets the attribute metadata.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    public override ICrmAttributeMetadata GetAttributeMetadata(string entityName, string attributeName)
    {
      var crmAttributeMetadataFactory = new CrmAttributeMetadataFactory();
      return crmAttributeMetadataFactory.Create(this.crmMetadataService.RetrieveAttributeMetadata(entityName, attributeName));
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
          innerFilters.Conditions = new[]
          {
            new ConditionExpression()
            {
              AttributeName = filteredField.Key,
              Operator = ConditionOperator.Like,
              Values = new object[]
              {
                string.Format("%{0}%", filteredField.Value)
              }
            }
          };
        }
        innerFilters.FilterOperator = LogicalOperator.Or;
      }
      if (onlyActive)
      {
        var filterExpression = new FilterExpression();
        if (innerFilters.Conditions != null && innerFilters.Conditions.Length > 0)
        {
          filterExpression.Filters = new[] { innerFilters };
          filterExpression.FilterOperator = LogicalOperator.And;
          filterExpression.Conditions = new[]
          {
            new ConditionExpression { AttributeName = "statecode", Operator = ConditionOperator.Equal, Values = new object[] { "Active" } }
          };
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
    /// Crm entities array
    /// </returns>
    public override ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions, bool onlyActive)
    {
      return GetEntities(logicalName, pagingInfo, orderExpressions, null, onlyActive);
    }

    public override ICrmEntity[] GetEntities(string logicalName, CrmPagingInfo pagingInfo, CrmOrderExpression[] orderExpressions,
      Dictionary<string, string> filteredFields, bool onlyActive)
    {
      var request = new RetrieveMultipleRequest
      {
        Query =
           new QueryExpression
           {
             ColumnSet = new AllColumns(),
             EntityName = logicalName,
             PageInfo = pagingInfo,
             Orders = orderExpressions.Cast<OrderExpression>().ToArray(),
             Criteria = this.GetFilterExpression(filteredFields, onlyActive)
           },
        ReturnDynamicEntities = true
      };

      var response = (RetrieveMultipleResponse)this.crmService.Execute(request);
      return response.BusinessEntityCollection.BusinessEntities.Where(e => e is DynamicEntity).Cast<DynamicEntity>().Select(e => new CrmEntityAdapter(this, e)).ToArray();
    }

    /// <summary>
    /// Gets the entity.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="value">The value.</param>
    /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
    /// <param name="columns">The columns.</param>
    public override ICrmEntity GetEntity(string logicalName, string fieldName, string value, bool onlyActive, string[] columns)
    {
      var filters = new List<ConditionExpression> { new ConditionExpression { AttributeName = fieldName, Operator = ConditionOperator.Equal, Values = new object[] { value } } };
      if (onlyActive)
      {
        filters.Add(new ConditionExpression { AttributeName = "statecode", Operator = ConditionOperator.Equal, Values = new object[] { "Active" } });
      }

      var filterExpression = new FilterExpression { FilterOperator = LogicalOperator.And, Conditions = filters.ToArray() };

      var request = new RetrieveMultipleRequest
      {
        Query =
           new QueryExpression
           {
             ColumnSet = columns == null ? (ColumnSetBase)new AllColumns() : new ColumnSet { Attributes = columns },
             EntityName = logicalName,
             PageInfo = new PagingInfo { Count = 1, PageNumber = 1 },
             Criteria = filterExpression
           },
        ReturnDynamicEntities = true
      };

      var response = (RetrieveMultipleResponse)this.crmService.Execute(request);

      var businessEntityCollection = response.BusinessEntityCollection;
      if (businessEntityCollection == null || businessEntityCollection.BusinessEntities == null || businessEntityCollection.BusinessEntities.Length <= 0)
      {
        return null;
      }

      var dynamicEntity = businessEntityCollection.BusinessEntities[0] as DynamicEntity;

      return dynamicEntity == null ? null : new CrmEntityAdapter(this, dynamicEntity);
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

      const string CountQuery = "<fetch mapping='logical' aggregate='true'><entity name='{0}'><attribute name='{1}' aggregate='count' alias='count' />{2}</entity></fetch>";
      var xml = this.crmService.Fetch(CountQuery.FormatWith(logicalName, primaryKey, this.GetFilterExpression(filteredFields, onlyActive).ToFetchXml()));

      var resultXml = XElement.Parse(xml);
      var resultNode = resultXml.Element("result");

      int returnValue = 0;

      if (resultNode == null)
      {
        return returnValue;
      }

      var node = resultNode.Element("count");
      if (node == null)
      {
        return returnValue;
      }

      int.TryParse(node.Value, out returnValue);

      return returnValue;
    }

    /// <summary>
    /// Creates new entity.
    /// </summary>
    /// <param name="logicalName">Entity logical name.</param>
    public override ICrmEntity NewEntity(string logicalName)
    {
      return new CrmEntityAdapter(this, new DynamicEntity { Name = logicalName });
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

      var createdEntity = this.crmService.Create(entityAdapter.Adaptee);
      entityAdapter.Id = createdEntity;

      if (entityAdapter.IsStateChanged || entityAdapter.IsStatusChanged)
      {
        this.UpdateStateStatus(entityAdapter.LogicalName, entityAdapter.Id, entityAdapter.State, entityAdapter.Status);
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

      this.crmService.Update(entityAdapter.Adaptee);

      if (entityAdapter.IsStateChanged || entityAdapter.IsStatusChanged)
      {
        this.UpdateStateStatus(entityAdapter.LogicalName, entityAdapter.Id, entityAdapter.State, entityAdapter.Status);
      }
    }

    /// <summary>
    /// Updates the state status.
    /// </summary>
    /// <param name="logicalName">Name of the logical.</param>
    /// <param name="id">The id.</param>
    /// <param name="state">The state.</param>
    /// <param name="status">The status.</param>
    public void UpdateStateStatus(string logicalName, Guid id, ICrmKeyValueAttribute state, ICrmKeyValueAttribute status)
    {
      try
      {
        this.crmService.Execute(new SetStateDynamicEntityRequest
        {
          Entity = new Moniker { Id = id, Name = logicalName },
          Status = status.Key,
          State = state.Value,
        });
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't update entity state and status", e, this);

        if (!e.Message.Contains("is not a valid status code for state code"))
        {
          return;
        }

        ConditionalLog.Info("Trying to set only state", this);

        try
        {
          this.crmService.Execute(new SetStateDynamicEntityRequest
          {
            Entity = new Moniker { Id = id, Name = logicalName },
            Status = status.Key,
            State = state.Value,
          });
        }
        catch
        {
          ConditionalLog.Error("Couldn't update entity state", e, this);
        }
      }
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entityName">Name of the entity.</param>
    /// <param name="id">The GUID.</param>
    public override void Delete(string entityName, Guid id)
    {
      this.crmService.Delete(entityName, id);
    }
  }
}