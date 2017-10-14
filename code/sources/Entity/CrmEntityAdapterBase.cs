// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmEntityAdapterBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmEntityAdapterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Entity
{
  using System;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Repository;

  /// <summary>
  /// The crm entity adapter base.
  /// </summary>
  /// <typeparam name="T">Type of adaptee entity.</typeparam>
  public abstract class CrmEntityAdapterBase<T> : AdapterBase<T>, ICrmEntity
  {
    /// <summary>
    /// The repository
    /// </summary>
    private readonly EntityRepositoryBase repository;

    /// <summary>
    /// The initial state
    /// </summary>
    private ICrmKeyValueAttribute initialState;

    /// <summary>
    /// The initial status
    /// </summary>
    private ICrmKeyValueAttribute initialStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmEntityAdapterBase{T}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="adaptee">The adaptee.</param>
    protected CrmEntityAdapterBase(EntityRepositoryBase repository, T adaptee)
      : base(adaptee)
    {
      this.repository = repository;
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    public abstract ICrmAttributeCollection Attributes { get; }

    /// <summary>
    /// Gets or sets the name of the logical.
    /// </summary>
    /// <value>
    /// The name of the logical.
    /// </value>
    public abstract string LogicalName { get; set; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public ICrmKeyValueAttribute State
    {
      get
      {
        ICrmKeyValueAttribute stateAttribute;
        if (this.Attributes == null || (stateAttribute = this.Attributes["statecode"] as ICrmKeyValueAttribute) == null)
        {
          return null;
        }

        return stateAttribute;
      }
    }

    /// <summary>
    /// Gets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public ICrmKeyValueAttribute Status
    {
      get
      {
        ICrmKeyValueAttribute statusAttribute;
        if (this.Attributes == null || (statusAttribute = this.Attributes["statuscode"] as ICrmKeyValueAttribute) == null)
        {
          return null;
        }

        return statusAttribute;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is state changed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is state changed; otherwise, <c>false</c>.
    /// </value>
    public bool IsStateChanged
    {
      get
      {
        return this.initialState != this.State;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is status changed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is status changed; otherwise, <c>false</c>.
    /// </value>
    public bool IsStatusChanged
    {
      get
      {
        return this.initialStatus != this.Status;
      }
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <value>
    /// The repository.
    /// </value>
    public EntityRepositoryBase Repository
    {
      get
      {
        return this.repository;
      }
    }

    /// <summary>
    /// Attributes the collection adapter initialized.
    /// </summary>
    protected void AttributeCollectionAdapterInitialized()
    {
      this.initialState = this.State;
      this.initialStatus = this.Status;
    }
  }
}