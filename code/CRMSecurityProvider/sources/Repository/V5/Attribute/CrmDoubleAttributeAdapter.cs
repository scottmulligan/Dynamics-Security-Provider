namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;

  /// <summary>
  /// The crm double attribute adapter.
  /// </summary>
  internal class CrmDoubleAttributeAdapter : CrmValueTypeAttributeAdapter<double>, ICrmDoubleAttribute
  {
    public CrmDoubleAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, double internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    protected override bool TryParseValue(string value, out double result)
    {
      return double.TryParse(value, out result);
    }

    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }
  }
}