// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdapterFactoryBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   The adapter factory base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources
{
  /// <summary>
  /// The adapter factory base.
  /// </summary>
  /// <typeparam name="TAdaptee">The type of adaptee.</typeparam>
  /// <typeparam name="TAdapter">The type of adapter</typeparam>
  internal abstract class AdapterFactoryBase<TAdaptee, TAdapter>
  {
    /// <summary>
    /// Creates the specified adaptee.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    /// <returns>The adapter.</returns>
    public abstract TAdapter Create(TAdaptee adaptee);
  }
}