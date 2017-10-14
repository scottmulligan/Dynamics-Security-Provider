namespace CRMSecurityProvider.Repository.V3
{
  using System;

  using CRMSecurityProvider.crm3.webservice;

  /// <summary>
  /// CRM service interface (API v3).
  /// </summary>
  public interface ICrmServiceV3
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
