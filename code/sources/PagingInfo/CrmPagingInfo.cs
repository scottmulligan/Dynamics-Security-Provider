// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmPagingInfo.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmPagingInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.PagingInfo
{
  using PagingInfoV3 = CRMSecurityProvider.crm3.webservice.PagingInfo;
  using PagingInfoV4 = CRMSecurityProvider.crm4.webservice.PagingInfo;
  using PagingInfoV5 = Microsoft.Xrm.Sdk.Query.PagingInfo;

  // ReSharper disable ClassNeverInstantiated.Global

  /// <summary>
  /// The crm paging info.
  /// </summary>
  public sealed class CrmPagingInfo
  {
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    /// <value>
    /// The page number.
    /// </value>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [return total record count].
    /// </summary>
    /// <value>
    /// <c>true</c> if [return total record count]; otherwise, <c>false</c>.
    /// </value>
    public bool ReturnTotalRecordCount { get; set; }

    // ReSharper restore UnusedAutoPropertyAccessor.Global
    // ReSharper restore MemberCanBePrivate.Global

    /// <summary>
    /// Casts <see cref="CrmPagingInfo"/> to PagingInfo of CRM V3.
    /// </summary>
    /// <param name="crmPagingInfo">The CRM paging info.</param>
    /// <returns>PagingInfo of CRM V3.</returns>
    public static implicit operator PagingInfoV3(CrmPagingInfo crmPagingInfo)
    {
      return new PagingInfoV3 { Count = crmPagingInfo.Count, PageNumber = crmPagingInfo.PageNumber };
    }

    /// <summary>
    /// Casts <see cref="CrmPagingInfo"/> to PagingInfo of CRM V4.
    /// </summary>
    /// <param name="crmPagingInfo">The CRM paging info.</param>
    /// <returns>PagingInfo of CRM V4.</returns>
    public static implicit operator PagingInfoV4(CrmPagingInfo crmPagingInfo)
    {
      return new PagingInfoV4 { Count = crmPagingInfo.Count, PageNumber = crmPagingInfo.PageNumber };
    }

    /// <summary>
    /// Casts <see cref="CrmPagingInfo"/> to PagingInfo of CRM V5.
    /// </summary>
    /// <param name="crmPagingInfo">The CRM paging info.</param>
    /// <returns>PagingInfo of CRM V5.</returns>
    public static implicit operator PagingInfoV5(CrmPagingInfo crmPagingInfo)
    {
      return new PagingInfoV5 { Count = crmPagingInfo.Count, PageNumber = crmPagingInfo.PageNumber, ReturnTotalRecordCount = crmPagingInfo.ReturnTotalRecordCount };
    }
  }

  // ReSharper restore ClassNeverInstantiated.Global
}
