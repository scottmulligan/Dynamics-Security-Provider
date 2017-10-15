namespace CRMSecurityProvider.Sources.Repository.V5.Extensions
{
  using Microsoft.Xrm.Sdk.Query;

  public static class ConditionExpressionExtensions
  {
    public static bool IsEmpty(this ConditionExpression condition)
    {
      return string.IsNullOrEmpty(condition.AttributeName) || condition.Values == null || condition.Values.Count == 0;
    }
  }
}