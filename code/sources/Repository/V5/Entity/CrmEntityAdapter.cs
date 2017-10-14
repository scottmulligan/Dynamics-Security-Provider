// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeCollectionAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Entity
{
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Entity;
  using CRMSecurityProvider.Sources.Repository.V5.Attribute;
  using Entity = Microsoft.Xrm.Sdk.Entity;

  /// <summary>
  /// The CRM entity adapter.
  /// </summary>
  public class CrmEntityAdapter : CrmEntityAdapterBase<Entity>
  {
    /// <summary>
    /// The CRM attribute collection adapter
    /// </summary>
    private readonly CrmAttributeCollectionAdapter crmAttributeCollectionAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmEntityAdapter" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="entity">The entity.</param>
    public CrmEntityAdapter(EntityRepository repository, Entity entity)
      : base(repository, entity)
    {
      this.crmAttributeCollectionAdapter = new CrmAttributeCollectionAdapter(this, this.Adaptee.Attributes);
      this.AttributeCollectionAdapterInitialized();

      this.Id = entity.Id;
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
        return this.Adaptee.LogicalName;
      }

      set
      {
        this.Adaptee.LogicalName = value;
      }
    }

    /// <summary>
    /// Gets the attribute collection adapter.
    /// </summary>
    /// <value>
    /// The attribute collection adapter.
    /// </value>
    internal CrmAttributeCollectionAdapter AttributeCollectionAdapter
    {
      get
      {
        return this.crmAttributeCollectionAdapter;
      }
    }
  }
}