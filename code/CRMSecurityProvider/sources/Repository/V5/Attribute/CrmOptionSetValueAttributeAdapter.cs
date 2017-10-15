namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using System.Linq;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Attribute.Metadata;
  using Microsoft.Xrm.Sdk;

  public class CrmOptionSetValueAttributeAdapter : CrmAttributeAdapter<OptionSetValue>, ICrmKeyValueAttribute
  {
    public CrmOptionSetValueAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, OptionSetValue internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    public int Key
    {
      get
      {
        return this.Adaptee.Value;
      }
    }

    public string Value
    {
      get
      {
        var crmStateAttributeMetadata = this.AttributeCollection.EntityAdapter.Repository.GetAttributeMetadata(this.AttributeCollection.EntityAdapter.LogicalName, this.Name) as ICrmStateAttributeMetadata;
        if (crmStateAttributeMetadata == null)
        {
          return this.Key.ToString(CultureInfo.InvariantCulture);
        }

        var correspondingOption = crmStateAttributeMetadata.Options.FirstOrDefault(o => o.Key == this.Key);
        return correspondingOption.Value;
      }
    }

    public override string GetStringifiedValue()
    {
      return this.Value;
    }

    public override void SetValue(string value, params string[] data)
    {
      int valueToBeSet;
      if (!int.TryParse(value, out valueToBeSet))
      {
        return;
      }

      this.Adaptee.Value = valueToBeSet;
    }
  }
}