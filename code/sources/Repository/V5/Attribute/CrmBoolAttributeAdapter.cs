namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;
  using Sitecore;

  internal class CrmBoolAttributeAdapter : CrmValueTypeAttributeAdapter<bool>, ICrmBoolAttribute
  {
    public CrmBoolAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, bool internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    protected override bool TryParseValue(string value, out bool result)
    {
      result = MainUtil.GetBool(value, false);
      return true;
    }

    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }
  }
}