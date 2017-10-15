// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListEqualityComparer.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ListEqualityComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V4
{
  using System.Collections.Generic;
  using CRMSecurityProvider.crm4.webservice;

  /// <summary>
  /// The list equality comparer.
  /// </summary>
  public class ListEqualityComparer : IEqualityComparer<list>
  {
    /// <summary>
    /// Checks if one list equals to another.
    /// </summary>
    /// <param name="leftList">The list to compare.</param>
    /// <param name="rightList">The list to compare with.</param>
    /// <returns>True if lists are equal</returns>
    public bool Equals(list leftList, list rightList)
    {
      if (leftList == null || rightList == null || leftList.listid == null || rightList.listid == null)
      {
        return false;
      }

      return leftList.listid.Value.Equals(rightList.listid.Value);
    }

    /// <summary>
    /// Returns a hash code for the list.
    /// </summary>
    /// <param name="list">The list to get hash code of.</param>
    /// <returns>
    /// A hash code for this list, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public int GetHashCode(list list)
    {
      if (list == null || list.listid == null)
      {
        return 0;
      }

      return list.listid.Value.GetHashCode();
    }
  }
}