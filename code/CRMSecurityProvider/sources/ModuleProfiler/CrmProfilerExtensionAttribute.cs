namespace CRMSecurityProvider
{
  using System;
  using System.Web.Services.Protocols;

  [AttributeUsage(AttributeTargets.Method)]
  public class CrmProfilerExtensionAttribute : SoapExtensionAttribute
  {
    public override Type ExtensionType
    {
      get
      {
        return typeof(CrmProfilerExtension);
      }
    }

    public override int Priority { get; set; }
  }
}
