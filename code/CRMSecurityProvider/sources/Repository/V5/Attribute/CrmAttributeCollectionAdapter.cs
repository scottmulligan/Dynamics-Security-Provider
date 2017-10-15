// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeCollectionAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeCollectionAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Repository.V5.Entity;
  using Microsoft.Xrm.Sdk;

  /// <summary>
  /// The CRM attribute collection adapter.
  /// </summary>
  public class CrmAttributeCollectionAdapter : AdapterBase<AttributeCollection>, ICrmAttributeCollection
  {
    /// <summary>
    /// The system names
    /// </summary>
    private readonly string[] systemNames = { "statecode", "statuscode" };

    /// <summary>
    /// The CRM attribute adapter factory
    /// </summary>
    private readonly CrmNamingAttributeAdapterFactory crmAttributeAdapterFactory;

    /// <summary>
    /// The entity adapter
    /// </summary>
    private readonly CrmEntityAdapter entityAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmAttributeCollectionAdapter" /> class.
    /// </summary>
    /// <param name="entityAdapter">The entity adapter.</param>
    /// <param name="attributeCollection">The attribute collection.</param>
    public CrmAttributeCollectionAdapter(CrmEntityAdapter entityAdapter, AttributeCollection attributeCollection)
      : base(attributeCollection)
    {
      this.entityAdapter = entityAdapter;
      this.crmAttributeAdapterFactory = new CrmNamingAttributeAdapterFactory(this);
    }

    /// <summary>
    /// Gets the entity adapter.
    /// </summary>
    /// <value>
    /// The entity adapter.
    /// </value>
    public CrmEntityAdapter EntityAdapter
    {
      get
      {
        return this.entityAdapter;
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The <see cref="object"/>.
    /// </returns>
    public ICrmAttribute this[string key]
    {
      get
      {
        if (this.Adaptee == null)
        {
          return null;
        }

        return this.Adaptee.ContainsKey(key) ? this.crmAttributeAdapterFactory.Create(key, this.Adaptee[key]) : null;
      }
    }

    /// <summary>
    /// Adds the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public ICrmAttribute Create(string name, CrmAttributeType type, string value, params string[] data)
    {
      object attribute;

      switch (type)
      {
        case CrmAttributeType.BigInt:
          attribute = (long)0;
          break;
        case CrmAttributeType.Integer:
          attribute = 0;
          break;

        case CrmAttributeType.Boolean:
          attribute = false;
          break;

        case CrmAttributeType.Decimal:
          attribute = (decimal)0;
          break;

        case CrmAttributeType.Money:
          attribute = new Money();
          break;
        case CrmAttributeType.Double:
          attribute = (double)0;
          break;
        case CrmAttributeType.UniqueIdentifier:
          attribute = Guid.Empty;
          break;

        case CrmAttributeType.DateTime:
          attribute = DateTime.MinValue;
          break;

        case CrmAttributeType.Lookup:
        case CrmAttributeType.Customer:
        case CrmAttributeType.Owner:
          attribute = new EntityReference { Name = name };
          break;

        case CrmAttributeType.State:
        case CrmAttributeType.Status:
        case CrmAttributeType.Picklist:
          attribute = new OptionSetValue();
          break;

        default:
          attribute = value;
          break;
      }

      this.Add(new KeyValuePair<string, object>(name, attribute));

      var crmAttributeAdapter = this.crmAttributeAdapterFactory.Create(name, attribute);

      // TODO Crm consider to split create/setvalue methods functionality
      crmAttributeAdapter.SetValue(value, data);

      return crmAttributeAdapter;
    }

    /// <summary>
    /// Strips the system properties.
    /// </summary>
    /// <returns>The system properties</returns>
    public KeyValuePair<string, object>[] StripSystem()
    {
      var systemProperties = this.Adaptee.Where(p => this.systemNames.Contains(p.Key)).ToArray();
      foreach (var systemProperty in systemProperties)
      {
        this.Adaptee.Remove(systemProperty);
      }

      return systemProperties;
    }


    /// <summary>
    /// Removes the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    public void Remove(string name)
    {
      var properties = this.Adaptee.Where(p => p.Key.Equals(name)).ToArray();
      foreach (var p in properties)
      {
        this.Adaptee.Remove(p);
      }
    }



    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, ICrmAttribute>> GetEnumerator()
    {
      // TODO Crm consider to optimize adapter creation
      return this.Adaptee.Select(a => new KeyValuePair<string, ICrmAttribute>(a.Key, this.crmAttributeAdapterFactory.Create(a.Key, a.Value))).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="attributes">The attributes.</param>
    public void AddRange(KeyValuePair<string, object>[] attributes)
    {
      foreach (var attribute in attributes)
      {
        this.Add(attribute);
      }
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">The value.</param>
    public void SetValue(string attributeName, object value)
    {
      this.Adaptee[attributeName] = value;
    }

    /// <summary>
    /// Adds the specified attribute.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    public void Add(KeyValuePair<string, object> attribute)
    {
      if (this.Adaptee.ContainsKey(attribute.Key))
      {
        this.Adaptee[attribute.Key] = attribute.Value;
      }
      else
      {
        this.Adaptee.Add(attribute.Key, attribute.Value);
      }
    }
  }
}