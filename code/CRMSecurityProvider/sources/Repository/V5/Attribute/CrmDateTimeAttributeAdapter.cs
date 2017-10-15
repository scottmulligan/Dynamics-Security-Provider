namespace CRMSecurityProvider.Sources.Repository.V5.Attribute
{
  using System;
  using System.Globalization;
  using CRMSecurityProvider.Sources.Attribute;
  using CRMSecurityProvider.Sources.Utils.Extensions;
  using Microsoft.Xrm.Sdk;

  internal class CrmDateTimeAttributeAdapter : CrmValueTypeAttributeAdapter<DateTime>, ICrmDateTimeAttribute
  {
    public CrmDateTimeAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, DateTime internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    protected override bool TryParseValue(string value, out DateTime result)
    {
      return value.TryParseDateTime(out result);
    }

    public override string GetStringifiedValue()
    {
      return this.Adaptee.ToString(CultureInfo.InvariantCulture);
    }
  }

  internal class CrmMoneyAttributeAdapter : CrmAttributeAdapter<Money>, ICrmDecimalAttribute
  {
    public CrmMoneyAttributeAdapter(CrmAttributeCollectionAdapter crmAttributeCollection, Money internalAttribute)
      : base(crmAttributeCollection, internalAttribute)
    {
    }

    public override string GetStringifiedValue()
    {
      return this.Value.ToString(CultureInfo.InvariantCulture);
    }

    public override void SetValue(string value, params string[] data)
    {
      decimal parsedResult;
      if (decimal.TryParse(value, out parsedResult))
      {
        this.Adaptee.Value = parsedResult;
      }
    }

    public decimal Value
    {
      get
      {
        return this.Adaptee.Value;
      }
    }
  }
}