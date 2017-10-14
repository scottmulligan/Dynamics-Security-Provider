// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmEntityAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Entity
{
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.Repository.V4.Attribute;
  using CRMSecurityProvider.Sources.Repository.V4.Extensions;

  /// <summary>
  /// The crm entity adapter.
  /// </summary>
  internal class CrmEntityAdapter : CrmEntityAdapterBase<DynamicEntity>
  {
    /// <summary>
    /// The CRM attribute collection adapter
    /// </summary>
    private readonly CrmAttributeCollectionAdapter crmAttributeCollectionAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmEntityAdapter" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="dynamicEntity">The dynamic entity.</param>
    public CrmEntityAdapter(EntityRepositoryBase repository, DynamicEntity dynamicEntity)
      : base(repository, dynamicEntity)
    {
      this.crmAttributeCollectionAdapter = new CrmAttributeCollectionAdapter(this, this.Adaptee);
      this.Id = dynamicEntity.GetId();

      this.AttributeCollectionAdapterInitialized();
    }

    /// <summary>
    /// Gets the attribute collection adapter.
    /// </summary>
    /// <value>
    /// The attribute collection adapter.
    /// </value>
    public CrmAttributeCollectionAdapter AttributeCollectionAdapter
    {
      get
      {
        return this.crmAttributeCollectionAdapter;
      }
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    public override ICrmAttributeCollection Attributes
    {
      get
      {
        return this.crmAttributeCollectionAdapter;
      }
    }

    /// <summary>
    /// Gets or sets the name of the logical.
    /// </summary>
    /// <value>
    /// The name of the logical.
    /// </value>
    public override string LogicalName
    {
      get
      {
        return this.Adaptee.Name;
      }

      set
      {
        this.Adaptee.Name = value;
      }
    }
  }
}