namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Xml;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Caching;
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Utils;

  /// <summary>
  /// User repository class (API v4).
  /// </summary>
  public class UserRepositoryV4 : UserRepositoryBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepositoryV4"/> class.
    /// </summary>
    /// <param name="crmService">The CRM service.</param>
    /// <param name="contactToUserConverter">The contact to user converter.</param>
    /// <param name="cacheService">The cache service.</param>
    public UserRepositoryV4(ICrmServiceV4 crmService, IContactToUserConverterV4 contactToUserConverter, ICacheService cacheService)
      : base(cacheService)
    {
      Assert.ArgumentNotNull(crmService, "crmService");
      Assert.ArgumentNotNull(contactToUserConverter, "contactToUserConverter");

      this.CrmService = crmService;
      this.ContactToUserConverter = contactToUserConverter;
    }

    /// <summary>
    /// Gets or sets the CRM service.
    /// </summary>
    protected ICrmServiceV4 CrmService { get; private set; }

    /// <summary>
    /// Gets or sets the contact to user converter.
    /// </summary>
    protected IContactToUserConverterV4 ContactToUserConverter { get; private set; }

    public override CRMUser CreateUser(string userName, string email, Guid providerUserKey)
    {
      Assert.ArgumentNotNullOrEmpty(userName, "userName");
      Assert.ArgumentNotNullOrEmpty(email, "email");

      const string CreateUserKey = "createUser";
      ConditionalLog.Info(String.Format("CreateUser({0}, {1}, {2}). Started.", userName, email, providerUserKey), this, TimerAction.Start, CreateUserKey);

      var properties = new List<Property>();
      if (Configuration.Settings.UniqueKeyProperty != "fullname")
      {
        properties.Add(new StringProperty
        {
          Name = Configuration.Settings.UniqueKeyProperty,
          Value = userName
        });
      }
      else
      {
        var nameParts = new List<string>(userName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        properties.Add(new StringProperty
        {
          Name = "firstname",
          Value = nameParts[0]
        });

        if (nameParts.Count > 1)
        {
          properties.Add(new StringProperty
          {
            Name = "lastname",
            Value = String.Join(" ", nameParts.GetRange(1, nameParts.Count - 1).ToArray())
          });
        }
      }

      if ((email != null) && (Configuration.Settings.UniqueKeyProperty != "emailaddress1"))
      {
        properties.Add(new StringProperty
        {
          Name = "emailaddress1",
          Value = email
        });
      }

      if (providerUserKey != Guid.Empty)
      {
        properties.Add(new KeyProperty
        {
          Name = "contactid",
          Value = new Key
          {
            Value = providerUserKey
          }
        });
      }

      var contact = new DynamicEntity();
      contact.Name = EntityName.contact.ToString();
      contact.Properties = properties.ToArray();

      CRMUser user = null;
      try
      {
        providerUserKey = this.CrmService.Create(contact);
        ConditionalLog.Info(String.Format("CreateUser({0}, {1}, {2}). User has been created in CRM.", userName, email, providerUserKey), this, TimerAction.Tick, CreateUserKey);

        user = new CRMUser(
          userName,
          providerUserKey,
          email,
          null,
          String.Empty,
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
        ConditionalLog.Error(String.Format("Couldn't create user {0} in CRM.", userName), e, this);
      }

      ConditionalLog.Info(String.Format("CreateUser({0}, {1}, {2}). Finished.", userName, email, providerUserKey), this, TimerAction.Stop, CreateUserKey);
      return user;
    }

    public override bool DeactivateUser(string userName)
    {
      Assert.ArgumentNotNull(userName, "userName");

      const string DeactivateUserKey = "deactivateUser";
      ConditionalLog.Info(String.Format("DeactivateUser({0}). Started.", userName), this, TimerAction.Start, DeactivateUserKey);

      var result = false;

      var user = this.GetUser(userName);
      if (user != null)
      {
        var request = new SetStateContactRequest();
        request.EntityId = user.ID;
        request.ContactState = ContactState.Inactive;
        request.ContactStatus = 2;

        try
        {
          this.CrmService.Execute(request);
          ConditionalLog.Info(String.Format("DeactivateUser({0}). User has been deactivated in CRM.", userName), this, TimerAction.Tick, DeactivateUserKey);

          this.CacheService.MembersCache.Clear();
          this.CacheService.MemberOfCache.Remove(userName);
          this.CacheService.UserCache.Remove(userName);

          result = true;
        }
        catch (Exception e)
        {
          ConditionalLog.Error(String.Format("Couldn't deactivate user {0} in CRM.", userName), e, this);
        }
      }

      ConditionalLog.Info(String.Format("DeactivateUser({0}). Finished.", userName), this, TimerAction.Stop, DeactivateUserKey);
      return result;
    }

    protected override List<CRMUser> FindUsersInCrmByEmail(string userEmail, int pageIndex, int pageSize, out int totalRecords)
    {
      var pagingInfo = new PagingInfo();
      pagingInfo.Count = pageSize;
      pagingInfo.PageNumber = pageIndex + 1;

      totalRecords = this.GetUsersNumber("emailaddress1", userEmail);

      var contacts = this.GetContacts(pagingInfo, "emailaddress1", ConditionOperator.Like, userEmail);
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override List<CRMUser> FindUsersInCrmByName(string userName, int pageIndex, int pageSize, out int totalRecords)
    {
      var pagingInfo = new PagingInfo();
      pagingInfo.Count = pageSize;
      pagingInfo.PageNumber = pageIndex + 1;

      totalRecords = this.GetUsersNumber(Configuration.Settings.UniqueKeyProperty, userName);

      var contacts = this.GetContacts(pagingInfo, Configuration.Settings.UniqueKeyProperty, ConditionOperator.Like, userName);
      return contacts.Select(this.ContactToUserConverter.Convert).ToList();
    }

    protected override List<CRMUser> GetAllUsersFromCrm(Microsoft.Xrm.Sdk.Query.PagingInfo pagingInfo, out int totalRecords)
    {
      var pagingInfo2 = new PagingInfo
      {
          Count = pagingInfo.Count,
          PageNumber = pagingInfo.PageNumber,
          PagingCookie = pagingInfo.PagingCookie
      };

      totalRecords = this.GetUsersNumber(Configuration.Settings.UniqueKeyProperty, string.Empty);

      var contacts = this.GetContacts(pagingInfo2, "statecode", ConditionOperator.Equal, "Active");
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
      var xmlQuery = new StringBuilder("<fetch mapping='logical' aggregate='true'>");
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
        var result = this.CrmService.Fetch(xmlQuery.ToString());

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(result);

        return Int32.Parse(xmlDoc.SelectSingleNode("resultset/result").InnerText);
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get number of contacts from CRM.", e, this);
      }

      return 0;
    }

    /// <summary>
    /// Gets the contacs.
    /// </summary>
    /// <param name="pagingInfo">The paging info.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="conditionOperator">Condition operator.</param>
    /// <param name="values">The values of the property.</param>
    /// <returns>The contacts.</returns>
    protected IEnumerable<DynamicEntity> GetContacts(PagingInfo pagingInfo, string propertyName, ConditionOperator conditionOperator, params object[] values)
    {
      var propertyConditionExpression = new ConditionExpression();
      propertyConditionExpression.AttributeName = propertyName;
      propertyConditionExpression.Operator = conditionOperator;
      propertyConditionExpression.Values = values;

      var stateCodeConditionExpression = new ConditionExpression();
      stateCodeConditionExpression.AttributeName = "statecode";
      stateCodeConditionExpression.Operator = ConditionOperator.Equal;
      stateCodeConditionExpression.Values = new object[] { "Active" };

      var uniquePropertyNotNullConditionExpression = new ConditionExpression();
      uniquePropertyNotNullConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      uniquePropertyNotNullConditionExpression.Operator = ConditionOperator.NotNull;

      var uniquePropertyNotEmptyConditionExpression = new ConditionExpression();
      uniquePropertyNotEmptyConditionExpression.AttributeName = Configuration.Settings.UniqueKeyProperty;
      uniquePropertyNotEmptyConditionExpression.Operator = ConditionOperator.NotEqual;
      uniquePropertyNotEmptyConditionExpression.Values = new object[] { String.Empty };

      var filterExpression = new FilterExpression();
      filterExpression.Conditions = new ConditionExpression[]
      {
        propertyConditionExpression,
        stateCodeConditionExpression,
        uniquePropertyNotNullConditionExpression,
        uniquePropertyNotEmptyConditionExpression
      };
      filterExpression.FilterOperator = LogicalOperator.And;

      var queryExpression = new QueryExpression();
      queryExpression.ColumnSet = new ColumnSet()
      {
        Attributes = this.Attributes
      };
      queryExpression.Criteria = filterExpression;
      queryExpression.EntityName = EntityName.contact.ToString();
      queryExpression.PageInfo = pagingInfo;

      var request = new RetrieveMultipleRequest();
      request.Query = queryExpression;
      request.ReturnDynamicEntities = true;

      try
      {
        var response = (RetrieveMultipleResponse)this.CrmService.Execute(request);
        if ((response != null) && (response.BusinessEntityCollection != null))
        {
          return response.BusinessEntityCollection.BusinessEntities.Cast<DynamicEntity>();
        }
      }
      catch (Exception e)
      {
        ConditionalLog.Error("Couldn't get contact(s) from CRM.", e, this);
      }

      return new DynamicEntity[0];
    }
  }
}
