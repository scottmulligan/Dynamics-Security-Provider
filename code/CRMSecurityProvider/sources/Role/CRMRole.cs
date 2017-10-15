namespace CRMSecurityProvider
{
    using System;

    using Sitecore.Caching;
    using Sitecore.Reflection;

    using CRMSecurityProvider.Caching;
    using System.Collections.Generic;
    /// <summary>
    /// Represents CRM marketing list as a role.
    /// </summary>
    public class CRMRole : ICrmCacheable
    {
    /// <summary>
    /// Data length changed.
    /// </summary>
    public event DataLengthChangedDelegate DataLengthChanged;
    /// <summary>
    /// Called when the data length has changed.
    /// </summary>
    /// <param name="obj">The object.</param>
    protected virtual void OnDataLengthChanged(ICacheable obj)
    {
      DataLengthChangedDelegate handler = this.DataLengthChanged;
      if (handler != null)
      {
        handler(obj);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CRMRole"/> class.
    /// </summary>
    /// <param name="name">Name of the role.</param>
    /// <param name="id">ID of the role.</param>
    public CRMRole(string name, Guid id)
    {
      this.Name = name;
      this.ID = id;

      this.Cacheable = true;
      this.Properties = new Dictionary<string, object>();
    }

    /// <summary>
    /// Gets or sets name of the role.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets ID of the role.
    /// </summary>
    public Guid ID { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is cacheable.
    /// </summary>
    /// <value><c>true</c> if cacheable; otherwise, <c>false</c>.</value>
    public bool Cacheable { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:Sitecore.Caching.ICacheable"/> is immutable.
    /// </summary>
    /// <value><c>true</c> if immutable; otherwise, <c>false</c>.</value>
    public bool Immutable
    {
      get
      {
        return false;
      }
    }
        /// <summary>
        /// Gets a value indicating whether this role represents a dynamic marketing list.
        /// </summary>
        /// <value><c>true</c> if dynamic; <c>false</c> if static.</value>
        public bool IsDynamicList
        {
            get
            {
                var typeValue = GetPropertyValue("type");
                if (typeValue != null)
                {
                    if (typeValue is bool)
                    {
                        return (bool)typeValue;
                    }
                }
                return false;
            }
        }
    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns>Size of the data to use when caching.</returns>
    public long GetDataLength()
    {
      return TypeUtil.SizeOfString(this.Name) + TypeUtil.SizeOfGuid();
    }
    protected IDictionary<string, object> Properties { get; set; }
    public void SetPropertyValue(string name, object value)
    {
        if (this.Properties.Keys.Contains(name))
        {
            this.Properties[name] = value;
        }
        else
        {
            this.Properties.Add(name, value);
        }
    }
    public object GetPropertyValue(string name)
    {
        if (this.Properties.ContainsKey(name))
        {
            return this.Properties[name];
        }
        return null;
    }
    /// <summary>
    /// Provides consistency with standard entities.
    /// </summary>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public object this[string attributeName]
    {
        get
        {
            return this.GetPropertyValue(attributeName);
        }
        set
        {
            this.SetPropertyValue(attributeName, value);
        }
    }
    public const string AttributeListName = "listname";
    public const string AttributeListId = "listid";
    public const string AttributeType = "type";
  }
}
