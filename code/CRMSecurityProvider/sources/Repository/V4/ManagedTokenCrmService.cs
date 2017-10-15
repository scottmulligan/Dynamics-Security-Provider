namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Globalization;

  using Sitecore.Diagnostics;

  using CRMSecurityProvider.Configuration;
  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// Managed token CRM service wrapper class.
  /// </summary>
  public class ManagedTokenCrmService : ManagedTokenService, ICrmServiceV4
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedTokenCrmService"/> class.
    /// </summary>
    /// <param name="crmService">The CRM service.</param>
    /// <param name="settings">The configuration settings.</param>
    public ManagedTokenCrmService(CrmService crmService, ConfigurationSettings settings)
      : base(settings)
    {
      Assert.ArgumentNotNull(crmService, "crmService");

      this.CrmService = crmService;
    }

    /// <summary>
    /// Gets or sets CRM service.
    /// </summary>
    protected CrmService CrmService { get; private set; }

    /// <summary>
    /// Gets or sets expiration date.
    /// </summary>
    protected DateTime ExpirationDate { get; private set; }

    public Response Execute(Request request)
    {
      this.Validate();
      return this.CrmService.Execute(request);
    }

    public string Fetch(string fetchXml)
    {
      this.Validate();
      return this.CrmService.Fetch(fetchXml);
    }

    public Guid Create(BusinessEntity entity)
    {
      this.Validate();
      return this.CrmService.Create(entity);
    }

    public void Delete(string entityName, Guid id)
    {
      this.Validate();
      this.CrmService.Delete(entityName, id);
    }

    public BusinessEntity Retrieve(string entityName, Guid id, ColumnSetBase columnSet)
    {
      this.Validate();
      return this.CrmService.Retrieve(entityName, id, columnSet);
    }

    public BusinessEntityCollection RetrieveMultiple(QueryBase query)
    {
      this.Validate();
      return this.CrmService.RetrieveMultiple(query);
    }

    public void Update(BusinessEntity entity)
    {
      this.Validate();
      this.CrmService.Update(entity);
    }

    private void Validate()
    {
      if (DateTime.UtcNow.AddMinutes(5) >= this.ExpirationDate)
      {
        var ticket = this.GetTicket();

        this.CrmService.CrmAuthenticationTokenValue.CrmTicket = ticket.CrmTicket;
        this.ExpirationDate = DateTime.Parse(ticket.ExpirationDate, null, DateTimeStyles.AdjustToUniversal);
      }
    }
  }
}
