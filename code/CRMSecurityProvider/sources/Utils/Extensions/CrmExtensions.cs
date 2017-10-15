// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmExtensions.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Utils.Extensions
{
  using System;
  using System.Globalization;

  /// <summary>
  /// The crm extensions.
  /// </summary>
  public static class CrmExtensions
  {
    /// <summary>
    /// Converts the <see cref="CrmExtensions"/> to a <see cref="String"/>.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>
    /// The <see cref="String"/>.
    /// </returns>
     public static string ToCrmDateTimeString(this DateTime dateTime)
     {
         return string.Format(CultureInfo.InvariantCulture, "{0:s}Z", dateTime.ToUniversalTime());
     }
  }
}