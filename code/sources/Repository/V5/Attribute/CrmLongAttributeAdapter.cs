namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;

  internal class CrmLongAttributeAdapter : CrmValueTypeAttributeAdapter<long>, ICrmLongAttribute
  {
    public CrmLongAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, long internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    protected override bool TryParseValue(string value, out long result)
    {
      return long.TryParse(value, out result);
    }

    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }
  }
}