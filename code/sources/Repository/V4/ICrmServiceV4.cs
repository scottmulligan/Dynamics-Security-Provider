namespace CRMSecurityProvider.Repository.V4
{
  using System;

  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// CRM service interface (API v4).
  /// </summary>
  public interface ICrmServiceV4
  {
    Response Execute(Request request);

    string Fetch(string fetchXml);

    Guid Create(BusinessEntity entity);

    void Delete(string entityName, Guid id);

    BusinessEntity Retrieve(string entityName, Guid id, ColumnSetBase columnSet);

    BusinessEntityCollection RetrieveMultiple(QueryBase query);

    void Update(BusinessEntity entity);
  }
}
