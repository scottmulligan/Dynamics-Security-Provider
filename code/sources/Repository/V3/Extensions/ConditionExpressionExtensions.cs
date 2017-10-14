namespace CRMSecurityProvider.Sources.Repository.V3.Extensions
{
  using CRMSecurityProvider.crm3.webservice;

  public static class ConditionExpressionExtensions
  {
    public static bool IsEmpty(this ConditionExpression condition)
    {
      return string.IsNullOrEmpty(condition.AttributeName) || condition.Values == null || condition.Values.Length == 0;
    }
  }
}