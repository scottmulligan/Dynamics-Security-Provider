// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmStateAttributeAdapter.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmStateAttributeAdapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4.Attribute
{
  using System.Linq;
  using CRMSecurityProvider.crm4.webservice;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;

  /// <summary>
  /// The crm state attribute adapter.
  /// </summary>
  internal class CrmStateAttributeAdapter : CrmAttributeAdapter<StateProperty>, ICrmKeyValueAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmStateAttributeAdapter"/> class.
    /// </summary>
    /// <param name="crmAttributeCollection">The CRM attribute collection.</param>
    /// <param name="internalAttribute">The internal attribute.</param>
    public CrmStateAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, StateProperty internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public int Key
    {
      get
      {
        if (string.IsNullOrEmpty(this.Value))
        {
          return -1;
        }

        var crmStateAttributeMetadata = this.AttributeCollection.EntityAdapter.Repository.GetAttributeMetadata(this.AttributeCollection.EntityAdapter.LogicalName, this.Name) as ICrmStateAttributeMetadata;
        if (crmStateAttributeMetadata == null)
        {
          return -1;
        }

        var correspondingOption = crmStateAttributeMetadata.Options.FirstOrDefault(o => o.Value == this.Value);
        return correspondingOption.Key;
      }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value
    {
      get
      {
        return this.Adaptee.Value;
      }
    }

    /// <summary>
    /// Gets the stringified value.
    /// </summary>
    /// <returns>The stringified value.</returns>
    public override string GetStringifiedValue()
    {
      return this.Value;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    public override void SetValue(string value, params string[] data)
    {
      int intState;
      string strState = value;
      if (int.TryParse(value, out intState))
      {
        var crmStateAttributeMetadata = this.AttributeCollection.EntityAdapter.Repository.GetAttributeMetadata(this.AttributeCollection.EntityAdapter.LogicalName, this.Name) as ICrmStateAttributeMetadata;
        if (crmStateAttributeMetadata == null)
        {
          return;
        }

        var correspondingOption = crmStateAttributeMetadata.Options.FirstOrDefault(o => o.Key == intState);
        strState = correspondingOption.Value;
      }

      this.Adaptee.Value = strState;
    }
  }
}