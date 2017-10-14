namespace CRMSecurityProvider
{
  using System.Collections.Generic;

  using Sitecore.Reflection;

  class PicklistContactAttribute : ContactAttribute
  {
    public PicklistContactAttribute(SupportedTypes type, Dictionary<string, int> options)
      :base(type)
    {
      Options = options;
    }

    public Dictionary<string, int> Options
    {
      get; set;
    }

    public override long GetDataLength()
    {
      long optionsSize = 0;
      foreach (KeyValuePair<string, int> option in Options)
      {
        optionsSize += TypeUtil.SizeOfString(option.Key) + TypeUtil.SizeOfInt32();
      }
      return base.GetDataLength() + optionsSize;
    }
  }
}