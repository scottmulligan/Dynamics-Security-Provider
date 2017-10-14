namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;

  internal class CrmStringAttributeAdapter : CrmValueTypeAttributeAdapter<string>, ICrmStringAttribute
  {
    public CrmStringAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, string internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    public override string GetStringifiedValue()
    {
      return this.Value.ToString(CultureInfo.InvariantCulture);
    }

    protected override bool TryParseValue(string value, out string result)
    {
      result = value;
      return true;
    }
  }
}