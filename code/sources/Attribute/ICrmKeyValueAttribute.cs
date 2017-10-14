namespace CRMSecurityProvider.Sources.Attribute
{
  public interface ICrmKeyValueAttribute : ICrmAttribute
  {
    int Key { get; }
    string Value { get; }
  }
}