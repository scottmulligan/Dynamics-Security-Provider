// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRepositoryV5.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   User repository class (API v5).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Repository.V5
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.Sources.Repository.V5;
  using CRMSecurityProvider.Utils;

  using Microsoft.Crm.Sdk.Messages;
  using Microsoft.Xrm.Sdk;
  using Microsoft.Xrm.Sdk.Messages;
  using Microsoft.Xrm.Sdk.Query;

  using Sitecore.Diagnostics;

  /// <summary>
  /// User repository class (API v5).
  /// </summary>
  public class UserRepositoryV5 : UserRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepositoryV5"/> class.
    /// </summary>
    /// <param name="organizationServiceCache"></param>
    /// <param name="contactToUserConverter">The contact to user converter.</param>
    /// <param name="cacheService">The cache service.</param>
    public UserRepositoryV5(OrganizationServiceCacheV5 organizationServiceCache, IContactToUserConverterV5 contactToUserConverter, ICacheService cacheService)
      : base(cacheService)
    {
      Assert.ArgumentNotNull(organizationServiceCache, "organizationServiceCache");
      Assert.ArgumentNotNull(contactToUserConverter, "contactToUserConverter");

      this.organizationServiceCache = organizationServiceCache;
      this.ContactToUserConverter = contactToUserConverter;
    }

    private readonly OrganizationServiceCacheV5 organizationServiceCache;

    /// <summary>
    /// Gets or sets the contact to user converter.
    /// </summary>
    protected IContactToUserConverterV5 ContactToUserConverter { get; private set; }

    public override CRMUser CreateUser(string userName, string email, Guid providerUserKey)
    {
      Assert.ArgumentNotNullOrEmpty(userName, "userName");
      Assert.ArgumentNotNullOrEmpty(email, "email");

      const string CreateUserKey = "createUser";
      ConditionalLog.Info(string.Format("CreateUser({0}, {1}, {2}). Started.", userName, email, providerUserKey), this, TimerAction.Start, CreateUserKey);

      var contact = new Entity
      {
        LogicalName = "contact"
      };
      if (Configuration.Settings.UniqueKeyProperty != "fullname")
      {
        contact[Configuration.Settings.UniqueKeyProperty] = userName;
      }
      else
      {
        var nameParts = new List<string>(userName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

        contact["firstname"] = nameParts[0];

        if (nameParts.Count > 1)
        {
          contact["lastname"] = string.Join(" ", nameParts.GetRange(1, nameParts.Count - 1).ToArray());
        }
      }

      if ((email != null) && (Configuration.Settings.UniqueKeyProperty != "emailaddress1"))
      {
        contact["emailaddress1"] = email;
      }

      if (providerUserKey != Guid.Empty)
      {
        contact["contactid"] = providerUserKey;
      }

      CRMUser user = null;
      try
      {
        providerUserKey = this.organizationServiceCache.GetOrganizationService().Create(contact);
        ConditionalLog.Info(string.Format("CreateUser({0}, {1}, {2}). User has been created in CRM.", userName, email, providerUserKey), this, TimerAction.Tick, CreateUserKey);

        user = new CRMUser(
          userName,
          providerUserKey,
          email,
          null,
          string.Empty,
          true,
          false,
          DateTime.Now,
          DateTime.MinValue,
          DateTime.MinValue,
          DateTime.MinValue,
          DateTime.MinValue);

        this.CacheService.UserCache.Add(user);
      }
      catch (Exception e)
      {
        ConditionalLog.Error(string.Format("Couldn't create user {0} in CRM.", userName), e, this);
      }

      ConditionalLog.Info(string.Format("CreateUser({0}, {1}, {2}). Finished.", userName, email, providerUserKey), this, TimerAction.Stop, CreateUserKey);
      return user;
    }

    public override bool DeactivateUser(string userName)
    {
      Assert.ArgumentNotNull(userName, "userName");

      const string DeactivateUserKey = "deactivateUser";
      ConditionalLog.Info(string.Format("DeactivateUser({0}). Started.", userName), this, TimerAction.Start, DeactivateUserKey);

      var result = false;

      var user = this.GetUser(userName);
      if (user != null)
      {
        var request = new SetStateRequest
        {
          EntityMoniker = new EntityReference("contact", user.ID),
          State = new OptionSetValue { Value = 1 },
          Status = new OptionSetValue { Value = 2 }
        };

        try
        {
          this.organizationServiceCache.GetOrganizationService().Execute(request);
          ConditionalLog.Info(string.Format("DeactivateUser({0}). User has been deactivated in CRM.", userName), this, TimerAction.Tick, DeactivateUserKey);

          this.CacheService.MembersCache.Clear();
          this.CacheService.MemberOfCache.Remove(userName);
          this.CacheService.UserCache.Remove(userName);

          result = true;
        }
        catch (Exception e)
        {
          ConditionalLog.Error(string.Format("Couldn't deactivate user {0} in CRM.", userName), e, this);
        }
      }

      ConditionalLog.Info(string.Format("DeactivateUser({0}). Finished.", userName), this, TimerAction.Stop, DeactivateUserKey);
      return result;
    }

    protected override List<CRMUser> FindUsersInCrmByEmail(string userEmail, int pageIndex, int pageSize, out int totalRecords)
    {
      var pagingInfo = new PagingInfo
      {
        Count = pageSize,
        PageNumber = pageIndex + 1,
        ReturnTotalRecordCount = true
      };

      var contacts = this.GetContacts(pagingInfo, out totalRecords, "emailaddress1", ConditionOperator.Like, userEmail);
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override List<CRMUser> FindUsersInCrmByName(string userName, int pageIndex, int pageSize, out int totalRecords)
    {
      var pagingInfo = new PagingInfo
      {
        Count = pageSize,
        PageNumber = pageIndex + 1,
        ReturnTotalRecordCount = true
      };

      var contacts = this.GetContacts(pagingInfo, out totalRecords, Configuration.Settings.UniqueKeyProperty, ConditionOperator.Like, userName);
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override List<CRMUser> GetAllUsersFromCrm(PagingInfo pagingInfo, out int totalRecords)
    {
      var contacts = this.GetContacts(pagingInfo, out totalRecords, "statecode", ConditionOperator.Equal, "Active");
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override CRMUser GetUserFromCrm(string userName)
    {
      var contacts = this.GetContacts(null, Configuration.Settings.UniqueKeyProperty, ConditionOperator.Equal, userName);
      return contacts.Select(this.ContactToUserConverter.Convert).FirstOrDefault();
    }

    protected override CRMUser GetUserFromCrm(Guid userId)
    {
      var contacts = this.GetContacts(null, "contactid", ConditionOperator.Equal, userId.ToString());
      return contacts.Select(this.ContactToUserConverter.Convert).FirstOrDefault();
    }

    protected override List<CRMUser> GetUsersFromCrm(string[] userNames)
    {
      var contacts = this.GetContacts(null, Configuration.Settings.UniqueKeyProperty, ConditionOperator.In, userNames);
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override int GetUsersNumber(string propertyName, string propertyValue)
    {
      var xmlQuery = new StringBuilder("<fetch mapping='logical' aggregate='true' nolock='true'>");
      xmlQuery.Append("<entity name='contact'>");
      xmlQuery.Append("<attribute name='contactid' aggregate='count' alias='count' />");
      xmlQuery.Append("<filter type='and'>");
      xmlQuery.Append("<condition attribute='statecode' operator='eq' value='Active'/>");
      xmlQuery.Append(string.Format("<condition attribute='{0}' operator='not-null'/>", Configuration.Settings.UniqueKeyProperty));
      xmlQuery.Append(string.Format("<condition attribute='{0}' operator='ne' value=''/>", Configuration.Settings.UniqueKeyProperty));
      if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
      {
        xmlQuery.Append(string.Format("<condition attribute='{0}' operator='like' value='{1}'/>", propertyName, propertyValue));
      }

      xmlQuery.Append("</filter>");
      xmlQuery.Append("</entity>");
      xmlQuery.Append("</fetch>");

      try
      {
        var result = this.organizationServiceCache.GetOrganizationService().RetrieveMultiple(new FetchExpression(xmlQuery.ToString()));

        foreach (var entity in result.Entities)
        {
          return (int)((AliasedValue)entity["count"]).Value;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get number of contacts from CRM.", e, this);
      }

      return 0;
    }

    /// <summary>
    /// Gets the contacts.
    /// </summary>
    /// <param name="pagingInfo">The paging info.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="conditionOperator">The condition operator.</param>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    protected IEnumerable<Entity> GetContacts(PagingInfo pagingInfo, string propertyName, ConditionOperator conditionOperator, params object[] values)
    {
      int totalRecords;
      return this.GetContacts(pagingInfo, out totalRecords, propertyName, conditionOperator, values);
    }

    /// <summary>
    /// Gets the contacs.
    /// </summary>
    /// <param name="pagingInfo">The paging info.</param>
    /// <param name="totalRecords">The total records.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="conditionOperator">Condition operator.</param>
    /// <param name="values">The values of the property.</param>
    /// <returns>
    /// The contacts.
    /// </returns>
    protected IEnumerable<Entity> GetContacts(PagingInfo pagingInfo, out int totalRecords, string propertyName, ConditionOperator conditionOperator, params object[] values)
    {
      totalRecords = 0;

      var propertyConditionExpression = new ConditionExpression
      {
        AttributeName = propertyName,
        Operator = conditionOperator
      };
      propertyConditionExpression.Values.AddRange(values);

      var stateCodeConditionExpression = new ConditionExpression
      {
        AttributeName = "statecode",
        Operator = ConditionOperator.Equal
      };
      stateCodeConditionExpression.Values.Add("Active");

      var uniquePropertyNotNullConditionExpression = new ConditionExpression
      {
        AttributeName = Configuration.Settings.UniqueKeyProperty,
        Operator = ConditionOperator.NotNull
      };

      var uniquePropertyNotEmptyConditionExpression = new ConditionExpression
      {
        AttributeName = Configuration.Settings.UniqueKeyProperty,
        Operator = ConditionOperator.NotEqual
      };
      uniquePropertyNotEmptyConditionExpression.Values.Add(string.Empty);

      var filterExpression = new FilterExpression();
      filterExpression.Conditions.AddRange(
        propertyConditionExpression,
        stateCodeConditionExpression,
        uniquePropertyNotNullConditionExpression,
        uniquePropertyNotEmptyConditionExpression);

      filterExpression.FilterOperator = LogicalOperator.And;

      var queryExpression = new QueryExpression
      {
        ColumnSet = new ColumnSet(this.Attributes),
        Criteria = filterExpression,
        EntityName = "contact",
        PageInfo = pagingInfo,
        NoLock = true
      };

      var request = new RetrieveMultipleRequest
      {
        Query = queryExpression
      };

      try
      {
        var service = this.organizationServiceCache.GetOrganizationService();
        if (service == null)
        {
          ConditionalLog.Error("[CRM_GetContacts]Service is null", this);
          return new Entity[0];
        }
        var response = service.Execute(request) as RetrieveMultipleResponse;
        if ((response != null) && (response.EntityCollection != null))
        {
          totalRecords = response.EntityCollection.TotalRecordCount;
          if (response.EntityCollection.TotalRecordCountLimitExceeded)
          {
            ConditionalLog.Error(
              string.Format(
                "The number of '{0}' entities is exceeding TotalRecordCountLimit. Only first {1} can be shown.",
                queryExpression.EntityName,
                totalRecords),
              this);
          }
          if (pagingInfo != null)
          {
            pagingInfo.PagingCookie = response.EntityCollection.PagingCookie;
          }
          return response.EntityCollection.Entities;
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get contact(s) from CRM.", e, this);
      }

      return new Entity[0];
    }
  }
}
