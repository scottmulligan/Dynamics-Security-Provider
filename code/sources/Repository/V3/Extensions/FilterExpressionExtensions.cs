// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterExpressionExtensions.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilterExpressionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V3.Extensions
{
  using System.Collections.Generic;
  using System.Text;
  using CRMSecurityProvider.crm3.webservice;

  /// <summary>
  /// The filter expression extensions.
  /// </summary>
  public static class FilterExpressionExtensions
  {
    /// <summary>
    /// The operators
    /// </summary>
    private static readonly Dictionary<ConditionOperator, string> Operators = new Dictionary<ConditionOperator, string> { { ConditionOperator.Equal, "eq" } };

    #region Methods

    /// <summary>
    /// Convert to the fetch XML.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The fetch XML query.</returns>
    public static string ToFetchXml(this FilterExpression filter)
    {
      var builder = new StringBuilder();

      if (!filter.IsEmpty())
      {
        builder.AppendFormat("<filter type='{0}'>", filter.FilterOperator.ToString().ToLower());

        if (filter.Conditions != null)
        {
          foreach (var condition in filter.Conditions)
          {
            if (condition != null && !condition.IsEmpty())
            {
              builder.AppendFormat(
                "<condition attribute='{0}' operator='{1}' value='{2}'/>",
                condition.AttributeName,
                Operators.ContainsKey(condition.Operator) ? Operators[condition.Operator] : condition.Operator.ToString().ToLower(),
                condition.Values[0]);
            }
          }
        }

        if (filter.Filters != null)
        {
          builder.Append(filter.Filters.ToFetchXml());
        }

        builder.Append("</filter>");
      }

      return builder.ToString();
    }

    /// <summary>
    /// Determines whether the specified filter is empty.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>
    /// <c>true</c> if the specified filter is empty; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsEmpty(this FilterExpression filter)
    {
      return (filter.Conditions == null || filter.Conditions.Length == 0) &&
             (filter.Filters == null || filter.Filters.Length == 0);
    }

    /// <summary>
    /// Convert to the fetch XML.
    /// </summary>
    /// <param name="filters">The filters.</param>
    /// <returns>The fetch XML query.</returns>
    private static string ToFetchXml(this IEnumerable<FilterExpression> filters)
    {
      var builder = new StringBuilder();
      foreach (var filter in filters)
      {
        if (filter != null)
        {
          builder.Append(filter.ToFetchXml());
        }
      }

      return builder.ToString();
    }

    #endregion
  }
}