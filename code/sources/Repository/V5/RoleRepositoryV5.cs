namespace CRMSecurityProvider.Repository.V5
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Sources.Repository.V5;
  using CRMSecurityProvider.Utils;
  using Microsoft.Crm.Sdk.Messages;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Messages;
  using Microsoft.Xrm.Sdk.Query;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Role repository class (API v5).
  /// </summary>
  public class RoleRepositoryV5 : RoleRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRepositoryV5"/> class.
    /// </summary>
    /// <param name="organizationServiceCache"></param>
    /// <param name="marketingListToRoleConverter">The marketing list to role converter.</param>
    /// <param name="contactToUserConverter">The contact to user converter.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="cacheService">The cache service.</param>
    public RoleRepositoryV5(OrganizationServiceCacheV5 organizationServiceCache, IMarketingListToRoleConverterV5 marketingListToRoleConverter, IContactToUserConverterV5 contactToUserConverter, UserRepositoryBase userRepository, ICacheService cacheService)
      : base(userRepository, cacheService)
    {
      Assert.ArgumentNotNull(organizationServiceCache, "organizationServiceCache");
      Assert.ArgumentNotNull(marketingListToRoleConverter, "marketingListToRoleConverter");

      this.organizationServiceCache = organizationServiceCache;
      this.MarketingListToRoleConverter = marketingListToRoleConverter;
      this.ContactToUserConverter = contactToUserConverter;
    }

    private readonly OrganizationServiceCacheV5 organizationServiceCache;

    /// <summary>
    /// Gets the marketing list to role converter.
    /// </summary>
    protected IMarketingListToRoleConverterV5 MarketingListToRoleConverter { get; private set; }

    /// <summary>
    /// Gets the contact to user converter.
    /// </summary>
    protected IContactToUserConverterV5 ContactToUserConverter { get; private set; }

    public override bool CreateRole(string roleName)
    {
      Assert.ArgumentNotNullOrEmpty(roleName, "roleName");

      const string CreateRoleKey = "createRole";
      ConditionalLog.Info(String.Format("CreateRole({0}). Started.", roleName), this, TimerAction.Start, CreateRoleKey);

      var marketingList = new Entity();
      marketingList.LogicalName = "list";
      marketingList["listname"] = roleName;
      marketingList["membertype"] = 2;
      marketingList["createdfromcode"] = new OptionSetValue { Value = 2 };

      var result = false;
      try
      {
        var id = this.organizationServiceCache.GetOrganizationService().Create(marketingList);
        ConditionalLog.Info(String.Format("CreateRole({0}). Role has been created in CRM.", roleName), this, TimerAction.Tick, CreateRoleKey);

        this.CacheService.RoleCache.Add(new CRMRole(roleName, id));

        result = true;
      }
      catch (Exception e)
      {
        ConditionalLog.Error(String.Format("Couldn't create role {0} in CRM.", roleName), e, this);
      }

      ConditionalLog.Info(String.Format("CreateRole({0}). Finished.", roleName), this, TimerAction.Stop, CreateRoleKey);
      return result;
    }

    public override bool DeactivateRole(string roleName)
    {
      Assert.ArgumentNotNull(roleName, "roleName");

      const string DeactivateRoleKey = "deactivateRole";
      ConditionalLog.Info(String.Format("DeactivateRole({0}). Started.", roleName), this, TimerAction.Start, DeactivateRoleKey);

      var result = false;

      var role = this.GetRole(roleName);
      if (role != null)
      {
        var request = new SetStateRequest();
        request.EntityMoniker = new EntityReference("list", role.ID);
        request.State = new OptionSetValue { Value = 1 };
        request.Status = new OptionSetValue { Value = 1 };

        try
        {
          this.organizationServiceCache.GetOrganizationService().Execute(request);
          ConditionalLog.Info(String.Format("DeactivateRole({0}). Role has been deactivated in CRM.", roleName), this, TimerAction.Tick, DeactivateRoleKey);

          this.CacheService.MemberOfCache.Clear();
          this.CacheService.MembersCache.Remove(roleName);
          this.CacheService.RoleCache.Remove(roleName);

          result = true;
        }
        catch (Exception e)
        {
          ConditionalLog.Error(String.Format("Couldn't deactivate role {0} in CRM.", roleName), e, this);
        }
      }

      ConditionalLog.Info(String.Format("DeactivateRole({0}). Finished.", roleName), this, TimerAction.Stop, DeactivateRoleKey);
      return result;
    }

    public override bool IsUserInRole(string userName, string roleName)
    {
      Assert.ArgumentNotNull(userName, "userName");
      Assert.ArgumentNotNull(roleName, "roleName");

      const string IsUserInRoleKey = "isUserInRole";
      ConditionalLog.Info(String.Format("IsUserInRole({0}, {1}). Started.", userName, roleName), this, TimerAction.Start, IsUserInRoleKey);

      var contactUniquePropertyConditionExpression = new ConditionExpression();
      contactUniquePropertyConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      contactUniquePropertyConditionExpression.Operator = ConditionOperator.Equal;
      contactUniquePropertyConditionExpression.Values.Add(userName);

      var contactStateCodeConditionExpression = new ConditionExpression();
      contactStateCodeConditionExpression.AttributeName = "statecode";
      contactStateCodeConditionExpression.Operator = ConditionOperator.Equal;
      contactStateCodeConditionExpression.Values.Add("Active");

      var contactFilterExpression = new FilterExpression();
      contactFilterExpression.Conditions.AddRange(
          contactUniquePropertyConditionExpression,
          contactStateCodeConditionExpression);
      contactFilterExpression.FilterOperator = LogicalOperator.And;

      var listNameConditionExpression = new ConditionExpression();
      listNameConditionExpression.AttributeName = "listname";
      listNameConditionExpression.Operator = ConditionOperator.Equal;
      listNameConditionExpression.Values.Add(roleName);

      var listFilterExpression = new FilterExpression();
      listFilterExpression.Conditions.Add(listNameConditionExpression);
      listFilterExpression.FilterOperator = LogicalOperator.And;

      var listMemberListLinkEntity = new LinkEntity();
      listMemberListLinkEntity.JoinOperator = JoinOperator.Inner;
      listMemberListLinkEntity.LinkCriteria = listFilterExpression;
      listMemberListLinkEntity.LinkFromAttributeName = "listid";
      listMemberListLinkEntity.LinkFromEntityName = "listmember";
      listMemberListLinkEntity.LinkToAttributeName = "listid";
      listMemberListLinkEntity.LinkToEntityName = "list";

      var contactListMemberLinkEntity = new LinkEntity();
      contactListMemberLinkEntity.JoinOperator = JoinOperator.Inner;
      contactListMemberLinkEntity.LinkEntities.Add(listMemberListLinkEntity);
      contactListMemberLinkEntity.LinkFromAttributeName = "contactid";
      contactListMemberLinkEntity.LinkFromEntityName = "contact";
      contactListMemberLinkEntity.LinkToAttributeName = "entityid";
      contactListMemberLinkEntity.LinkToEntityName = "listmember";

      var queryExpression = new QueryExpression();
      queryExpression.ColumnSet = new ColumnSet(Configuration.Settings.UniqueKeyProperty);
      queryExpression.Criteria = contactFilterExpression;
      queryExpression.EntityName = "contact";
      queryExpression.LinkEntities.Add(contactListMemberLinkEntity);
      queryExpression.NoLock = true;
      queryExpression.Distinct = false;

      var request = new RetrieveMultipleRequest();
      request.Query = queryExpression;

      try
      {
        var response = (RetrieveMultipleResponse)this.organizationServiceCache.GetOrganizationService().Execute(request);
        if ((response != null) && (response.EntityCollection != null) && (response.EntityCollection.Entities.Count != 0))
        {
          return true;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error(String.Format("Couldn't get {0} contact and {1} marketing list from CRM.", userName, roleName), e, this);
      }
      finally
      {
        ConditionalLog.Info(String.Format("IsUserInRole({0}, {1}). Finished.", userName, roleName), this, TimerAction.Stop, IsUserInRoleKey);
      }

      return false;
    }

    protected override bool AddUsersToRoles(List<CRMUser> users, List<CRMRole> roles)
    {
      const string AddUsersToRolesKey = "addUsersToRoles";
      ConditionalLog.Info("AddUsersToRoles(...). Started.", this, TimerAction.Start, AddUsersToRolesKey);

      var result = true;
      foreach (var role in roles)
      {
        foreach (var user in users)
        {
          var request = new AddMemberListRequest();
          request.EntityId = user.ID;
          request.ListId = role.ID;

          try
          {
            this.organizationServiceCache.GetOrganizationService().Execute(request);
            ConditionalLog.Info(String.Format("AddUsersToRoles(...). User {0} has been added to the {1} role.", user.Name, role.Name), this, TimerAction.Tick, AddUsersToRolesKey);

            this.CacheService.MembersCache.Remove(role.Name);
            this.CacheService.MemberOfCache.Remove(user.Name);
          }
          catch (Exception e)
          {
            ConditionalLog.Error(String.Format("Couldn't add contact {0} to marketing list {1} in CRM.", user.Name, role.Name), e, this);
            result = false;
          }
        }
      }

      ConditionalLog.Info("AddUsersToRoles(...). Finished.", this, TimerAction.Stop, AddUsersToRolesKey);
      return result;
    }

    protected override bool RemoveUsersFromRoles(List<CRMUser> users, List<CRMRole> roles)
    {
      const string RemoveUsersFromRolesKey = "removeUsersFromRoles";
      ConditionalLog.Info("RemoveUsersFromRoles(...). Started.", this, TimerAction.Start, RemoveUsersFromRolesKey);

      var result = true;
      foreach (var role in roles)
      {
        foreach (var user in users)
        {
          var request = new RemoveMemberListRequest();
          request.EntityId = user.ID;
          request.ListId = role.ID;

          try
          {
            this.organizationServiceCache.GetOrganizationService().Execute(request);
            ConditionalLog.Info(String.Format("RemoveUsersFromRoles(...). User {0} has been removed from {1} role.", user.Name, role.Name), this, TimerAction.Tick, RemoveUsersFromRolesKey);

            this.CacheService.MembersCache.Remove(role.Name);
            this.CacheService.MemberOfCache.Remove(user.Name);
          }
          catch (Exception e)
          {
            ConditionalLog.Error(String.Format("Couldn't remove contact {0} from marketing list {1} in CRM.", user.Name, role.Name), e, this);
            result = false;
          }
        }
      }

      ConditionalLog.Info("RemoveUsersFromRoles(...). Finished.", this, TimerAction.Stop, RemoveUsersFromRolesKey);
      return result;
    }

    protected override List<CRMRole> GetAllRolesFromCrm()
    {
      var marketingLists = this.GetMarketingLists("statecode", ConditionOperator.Equal, "Active");
      return marketingLists.Select(this.MarketingListToRoleConverter.Convert).ToList();
    }

    protected override CRMRole GetRoleFromCrm(string roleName)
    {
      var marketingLists = this.GetMarketingLists("listname", ConditionOperator.Equal, roleName);
      return marketingLists.Select(this.MarketingListToRoleConverter.Convert).FirstOrDefault();
    }

    protected override List<CRMRole> GetRolesFromCrm(string[] roleNames)
    {
      var marketingLists = this.GetMarketingLists("listname", ConditionOperator.In, roleNames);
      return marketingLists.Select(this.MarketingListToRoleConverter.Convert).ToList();
    }

    public override string[] GetRolesForUser(string userName)
    {
      Assert.ArgumentNotNull(userName, "userName");

      const string GetRolesForUserKey = "getRolesForUser";
      ConditionalLog.Info(String.Format("GetRolesForUser({0}). Started.", userName), this, TimerAction.Start, GetRolesForUserKey);

      var roles = this.CacheService.MemberOfCache.Get(userName);
      if (roles != null)
      {
        ConditionalLog.Info(String.Format("GetRolesForUser({0}). Finished (roles have been retrieved from cache).", userName), this, TimerAction.Stop, GetRolesForUserKey);
        return roles.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      }

      var listStateCodeConditionExpression = new ConditionExpression();
      listStateCodeConditionExpression.AttributeName = "statecode";
      listStateCodeConditionExpression.Operator = ConditionOperator.Equal;
      listStateCodeConditionExpression.Values.Add("Active");

      var listFilterExpression = new FilterExpression();
      listFilterExpression.Conditions.Add(listStateCodeConditionExpression);
      listFilterExpression.FilterOperator = LogicalOperator.And;

      var contactNameConditionExpression = new ConditionExpression();
      contactNameConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      contactNameConditionExpression.Operator = ConditionOperator.Equal;
      contactNameConditionExpression.Values.Add(userName);

      var contactFilterExpression = new FilterExpression();
      contactFilterExpression.Conditions.Add(contactNameConditionExpression);
      contactFilterExpression.FilterOperator = LogicalOperator.And;

      var listMemberContactLinkEntity = new LinkEntity();
      listMemberContactLinkEntity.JoinOperator = JoinOperator.Inner;
      listMemberContactLinkEntity.LinkCriteria = contactFilterExpression;
      listMemberContactLinkEntity.LinkFromAttributeName = "entityid";
      listMemberContactLinkEntity.LinkFromEntityName = "listmember";
      listMemberContactLinkEntity.LinkToAttributeName = "contactid";
      listMemberContactLinkEntity.LinkToEntityName = "contact";

      var listListMemberLinkEntity = new LinkEntity();
      listListMemberLinkEntity.JoinOperator = JoinOperator.Inner;
      listListMemberLinkEntity.LinkEntities.Add(listMemberContactLinkEntity);
      listListMemberLinkEntity.LinkFromAttributeName = "listid";
      listListMemberLinkEntity.LinkFromEntityName = "list";
      listListMemberLinkEntity.LinkToAttributeName = "listid";
      listListMemberLinkEntity.LinkToEntityName = "listmember";

      var pagingInfo = new PagingInfo
      {
        PageNumber = 1,
        Count = Configuration.Settings.FetchThrottlingPageSize
      };

      var queryExpression = new QueryExpression();
      queryExpression.PageInfo = pagingInfo;
      queryExpression.ColumnSet = new ColumnSet("listname");
      queryExpression.Criteria = listFilterExpression;
      queryExpression.EntityName = "list";
      queryExpression.LinkEntities.Add(listListMemberLinkEntity);
      queryExpression.NoLock = true;
      queryExpression.Distinct = false;

      var request = new RetrieveMultipleRequest();
      request.Query = queryExpression;

      var result = new HashSet<string>();
      try
      {
        while (true)
        {
          var service = this.organizationServiceCache.GetOrganizationService();
          if (service == null)
          {
            ConditionalLog.Error("[CRM_GetRolesForUser]Service is null", this);
            return result.ToArray();
          }
          var response = service.Execute(request) as RetrieveMultipleResponse;
          if (response == null || response.EntityCollection == null)
          {
            break;
          }

          ConditionalLog.Info(String.Format("GetRolesForUser({0}). Retrieved {1} roles from CRM.", userName, response.EntityCollection.Entities.Count), this, TimerAction.Tick, GetRolesForUserKey);

          foreach (var entity in response.EntityCollection.Entities)
          {
            result.Add((string)entity["listname"]);
          }

          if (!response.EntityCollection.MoreRecords)
          {
            break;
          }

          pagingInfo.PageNumber++;
          pagingInfo.PagingCookie = response.EntityCollection.PagingCookie;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error(String.Format("Couldn't get roles of {0} contact from CRM.", userName), e, this);
      }

      var returnValue = result.ToArray();
      this.CacheService.MemberOfCache.Add(userName, String.Join("|", returnValue));

      ConditionalLog.Info(String.Format("GetRolesForUser({0}). Finished.", userName), this, TimerAction.Stop, GetRolesForUserKey);

      return returnValue;
    }

    public override string[] GetUsersInRole(string roleName)
    {
      Assert.ArgumentNotNull(roleName, "roleName");

      const string GetUsersInRoleKey = "getUsersInRole";
      ConditionalLog.Info(string.Format("GetUsersInRole({0}). Started.", roleName), this, TimerAction.Start, GetUsersInRoleKey);

      var users = this.CacheService.MembersCache.Get(roleName);
      if (users != null)
      {
        ConditionalLog.Info(string.Format("GetUsersInRole({0}). Finished (users have been retrieved from cache).", roleName), this, TimerAction.Stop, GetUsersInRoleKey);
        return users.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      }

      var contactStateCodeConditionExpression = new ConditionExpression();
      contactStateCodeConditionExpression.AttributeName = "statecode";
      contactStateCodeConditionExpression.Operator = ConditionOperator.Equal;
      contactStateCodeConditionExpression.Values.Add("Active");

      var contactUniquePropertyNotNullConditionExpression = new ConditionExpression();
      contactUniquePropertyNotNullConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      contactUniquePropertyNotNullConditionExpression.Operator = ConditionOperator.NotNull;

      var contactUniquePropertyNotEmptyConditionExpression = new ConditionExpression();
      contactUniquePropertyNotEmptyConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      contactUniquePropertyNotEmptyConditionExpression.Operator = ConditionOperator.NotEqual;
      contactUniquePropertyNotEmptyConditionExpression.Values.Add(string.Empty);

      var contactFilterExpression = new FilterExpression();
      contactFilterExpression.Conditions.AddRange(
          contactStateCodeConditionExpression,
          contactUniquePropertyNotNullConditionExpression,
          contactUniquePropertyNotEmptyConditionExpression);
      contactFilterExpression.FilterOperator = LogicalOperator.And;

      var listNameConditionExpression = new ConditionExpression();
      listNameConditionExpression.AttributeName = "listname";
      listNameConditionExpression.Operator = ConditionOperator.Equal;
      listNameConditionExpression.Values.Add(roleName);

      var listFilterExpression = new FilterExpression();
      listFilterExpression.Conditions.Add(listNameConditionExpression);
      listFilterExpression.FilterOperator = LogicalOperator.And;

      var listMemberListLinkEntity = new LinkEntity();
      listMemberListLinkEntity.JoinOperator = JoinOperator.Inner;
      listMemberListLinkEntity.LinkCriteria = listFilterExpression;
      listMemberListLinkEntity.LinkFromAttributeName = "listid";
      listMemberListLinkEntity.LinkFromEntityName = "listmember";
      listMemberListLinkEntity.LinkToAttributeName = "listid";
      listMemberListLinkEntity.LinkToEntityName = "list";

      var contactListMemberLinkEntity = new LinkEntity();
      contactListMemberLinkEntity.JoinOperator = JoinOperator.Inner;
      contactListMemberLinkEntity.LinkEntities.Add(listMemberListLinkEntity);
      contactListMemberLinkEntity.LinkFromAttributeName = "contactid";
      contactListMemberLinkEntity.LinkFromEntityName = "contact";
      contactListMemberLinkEntity.LinkToAttributeName = "entityid";
      contactListMemberLinkEntity.LinkToEntityName = "listmember";

      var pagingInfo = new PagingInfo();
      pagingInfo.Count = Configuration.Settings.FetchThrottlingPageSize;
      pagingInfo.PageNumber = 1;

      var queryExpression = new QueryExpression();
      queryExpression.ColumnSet = new ColumnSet(Configuration.Settings.UniqueKeyProperty);
      queryExpression.Criteria = contactFilterExpression;
      queryExpression.EntityName = "contact";
      queryExpression.LinkEntities.Add(contactListMemberLinkEntity);
      queryExpression.PageInfo = pagingInfo;
      queryExpression.NoLock = true;
      queryExpression.Distinct = false;

      var request = new RetrieveMultipleRequest();
      request.Query = queryExpression;

      var result = new HashSet<string>();
      try
      {
        while (true)
        {
          var response = (RetrieveMultipleResponse)this.organizationServiceCache.GetOrganizationService().Execute(request);
          if ((response != null) && (response.EntityCollection != null))
          {
            ConditionalLog.Info(string.Format("GetUsersInRole({0}). Retrieved {1} users from CRM.", roleName, response.EntityCollection.Entities.Count), this, TimerAction.Tick, GetUsersInRoleKey);

            foreach (var entity in response.EntityCollection.Entities)
            {
              result.Add((string)entity[Configuration.Settings.UniqueKeyProperty]);
            }

            if (response.EntityCollection.MoreRecords)
            {
              pagingInfo.PageNumber++;
              pagingInfo.PagingCookie = response.EntityCollection.PagingCookie;
            }
            else
            {
              break;
            }
          }
        }

        this.CacheService.MembersCache.Add(roleName, string.Join("|", result.ToArray()));
      }
      catch (Exception e)
      {
        ConditionalLog.Error(string.Format("Couldn't get contacts of {0} marketing list from CRM.", roleName), e, this);
      }

      ConditionalLog.Info(string.Format("GetUsersInRole({0}). Finished.", roleName), this, TimerAction.Stop, GetUsersInRoleKey);
      return result.ToArray();
    }

    /// <summary>
    /// Gets the marketing lists.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="conditionOperator">Condition operator.</param>
    /// <param name="values">The values of the property.</param>
    /// <returns>The marketing lists.</returns>
    protected IEnumerable<Entity> GetMarketingLists(string propertyName, ConditionOperator conditionOperator, params object[] values)
    {
      var propertyConditionExpression = new ConditionExpression();
      propertyConditionExpression.AttributeName = propertyName;
      propertyConditionExpression.Operator = conditionOperator;
      propertyConditionExpression.Values.AddRange(values);

      var memberTypeConditionExpression = new ConditionExpression();
      memberTypeConditionExpression.AttributeName = "membertype";
      memberTypeConditionExpression.Operator = ConditionOperator.Equal;
      memberTypeConditionExpression.Values.Add(2);

      var stateCodeConditionExpression = new ConditionExpression();
      stateCodeConditionExpression.AttributeName = "statecode";
      stateCodeConditionExpression.Operator = ConditionOperator.Equal;
      stateCodeConditionExpression.Values.Add("Active");

      var filterExpression = new FilterExpression();
      filterExpression.Conditions.AddRange
        (
          propertyConditionExpression,
          memberTypeConditionExpression,
          stateCodeConditionExpression
        );
      filterExpression.FilterOperator = LogicalOperator.And;

      var pagingInfo = new PagingInfo
      {
        PageNumber = 1,
        Count = Configuration.Settings.FetchThrottlingPageSize
      };

      var queryExpression = new QueryExpression();
      queryExpression.PageInfo = pagingInfo;
      queryExpression.ColumnSet = new ColumnSet(this.Attributes);
      queryExpression.Criteria = filterExpression;
      queryExpression.EntityName = "list";
      queryExpression.NoLock = true;
      queryExpression.Distinct = false;

      var returnValue = new HashSet<Entity>(new EntityEqualityComparer());
      try
      {
        while (true)
        {
          var entities = this.organizationServiceCache.GetOrganizationService().RetrieveMultiple(queryExpression);
          if (entities == null)
          {
            break;
          }

          foreach (var entity in entities.Entities)
          {
            returnValue.Add(entity);
          }

          if (!entities.MoreRecords)
          {
            break;
          }

          pagingInfo.PageNumber++;
          pagingInfo.PagingCookie = entities.PagingCookie;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get the marketing list(s) from CRM.", e, this);
      }

      return returnValue.ToArray();
    }
  }
}