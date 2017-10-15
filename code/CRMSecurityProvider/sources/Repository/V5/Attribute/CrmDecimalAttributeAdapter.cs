namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;

  internal class CrmDecimalAttributeAdapter : CrmValueTypeAttributeAdapter<decimal>, ICrmDecimalAttribute
  {
    public CrmDecimalAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, decimal internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    protected override bool TryParseValue(string value, out decimal result)
    {
      return decimal.TryParse(value, out result);
    }

    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }
  }
}