// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityEqualityComparer.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the EntityEqualityComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Repository.V5
{
  using System.Collections.Generic;

  /// <summary>
  /// The entity equality comparer.
  /// </summary>
  public class EntityEqualityComparer : IEqualityComparer<Microsoft.Xrm.Sdk.Entity>
  {
    /// <summary>
    /// Checks if one entity equals to another.
    /// </summary>
    /// <param name="leftEntity">The entity to compare.</param>
    /// <param name="rightEntity">The entity to compare with.</param>
    /// <returns>True if entities are equal</returns>
    public bool Equals(Microsoft.Xrm.Sdk.Entity leftEntity, Microsoft.Xrm.Sdk.Entity rightEntity)
    {
      if (leftEntity == null || rightEntity == null)
      {
        return false;
      }

      return leftEntity.Id.Equals(rightEntity.Id);
    }

    /// <summary>
    /// Returns a hash code for the entity.
    /// </summary>
    /// <param name="entity">The entity to get hash code of.</param>
    /// <returns>
    /// A hash code for this entity, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public int GetHashCode(Microsoft.Xrm.Sdk.Entity entity)
    {
      return entity == null ? 0 : entity.Id.GetHashCode();
    }
  }
}