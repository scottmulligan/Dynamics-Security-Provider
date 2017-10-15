// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Utils.Extensions
{
  using System;
  using System.Globalization;

  /// <summary>
  /// The string extensions.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Tries the parse date time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if parsing succeeded, otherwise - false.</returns>
     public static bool TryParseDateTime(this string value, out DateTime result)
     {
       if (Sitecore.DateUtil.IsIsoDate(value))
       {
         result = Sitecore.DateUtil.IsoDateToDateTime(value);
         return true;
       }

       if ((value.EndsWith("Z") && DateTime.TryParse(value, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out result)) || DateTime.TryParse(value, out result))
       {
         return true;
       }

       return false;
     }
  }
}