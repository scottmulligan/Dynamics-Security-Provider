// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdapterBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdapterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources
{
  /// <summary>
  /// The adapter base.
  /// </summary>
  /// <typeparam name="T">Type of adaptee.</typeparam>
  public class AdapterBase<T>
  {
    /// <summary>
    /// The adaptee
    /// </summary>
    private readonly T adaptee;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdapterBase{T}"/> class.
    /// </summary>
    /// <param name="adaptee">The adaptee.</param>
    protected AdapterBase(T adaptee)
    {
      this.adaptee = adaptee;
    }

    /// <summary>
    /// Gets the adaptee.
    /// </summary>
    /// <value>
    /// The adaptee.
    /// </value>
    internal T Adaptee
    {
      get
      {
        return this.adaptee;
      }
    }
  }
}