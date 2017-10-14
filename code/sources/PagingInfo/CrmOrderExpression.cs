// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmOrderExpression.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   The crm order expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.PagingInfo
{
  using System.ComponentModel;
  using OrderExpressionV3 = CRMSecurityProvider.crm3.webservice.OrderExpression;
  using OrderExpressionV4 = CRMSecurityProvider.crm4.webservice.OrderExpression;
  using OrderExpressionV5 = Microsoft.Xrm.Sdk.Query.OrderExpression;

  // ReSharper disable ClassNeverInstantiated.Global

  /// <summary>
  /// The crm order expression.
  /// </summary>
  public class CrmOrderExpression
  {
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// Gets or sets the name of the attribute.
    /// </summary>
    /// <value>
    /// The name of the attribute.
    /// </value>
    public string AttributeName { get; set; }

    /// <summary>
    /// Gets or sets the type of the order.
    /// </summary>
    /// <value>
    /// The type of the order.
    /// </value>
    public ListSortDirection OrderType { get; set; }

    // ReSharper restore UnusedAutoPropertyAccessor.Global
    // ReSharper restore MemberCanBePrivate.Global

    /// <summary>
    /// Casts <see cref="CrmOrderExpression"/> to OrderExpression of CRM V3.
    /// </summary>
    /// <param name="orderExpression">The order expression.</param>
    /// <returns>OrderExpression of CRM V3.</returns>
    public static implicit operator OrderExpressionV3(CrmOrderExpression orderExpression)
    {
      return new OrderExpressionV3
      {
        AttributeName = orderExpression.AttributeName,
        OrderType = orderExpression.OrderType == ListSortDirection.Ascending ? crm3.webservice.OrderType.Ascending : crm3.webservice.OrderType.Descending
      };
    }

    /// <summary>
    /// Casts <see cref="CrmOrderExpression"/> to OrderExpression of CRM V4.
    /// </summary>
    /// <param name="orderExpression">The order expression.</param>
    /// <returns>OrderExpression of CRM V4.</returns>
    public static implicit operator OrderExpressionV4(CrmOrderExpression orderExpression)
    {
      return new OrderExpressionV4
      {
        AttributeName = orderExpression.AttributeName,
        OrderType = orderExpression.OrderType == ListSortDirection.Ascending ? crm4.webservice.OrderType.Ascending : crm4.webservice.OrderType.Descending
      };
    }

    /// <summary>
    /// Casts <see cref="CrmOrderExpression"/> to OrderExpression of CRM V5.
    /// </summary>
    /// <param name="orderExpression">The order expression.</param>
    /// <returns>OrderExpression of CRM V5.</returns>
    public static implicit operator OrderExpressionV5(CrmOrderExpression orderExpression)
    {
      return new OrderExpressionV5
      {
        AttributeName = orderExpression.AttributeName,
        OrderType = orderExpression.OrderType == ListSortDirection.Ascending ? Microsoft.Xrm.Sdk.Query.OrderType.Ascending : Microsoft.Xrm.Sdk.Query.OrderType.Descending
      };
    }
  }

  // ReSharper restore ClassNeverInstantiated.Global
}