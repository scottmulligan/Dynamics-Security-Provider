// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmAttributeType.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the CrmAttributeType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CRMSecurityProvider.Sources.Attribute
{
  /// <summary>
  /// The crm attribute type.
  /// </summary>
  public enum CrmAttributeType
  {


    #region old sequence
    //    Boolean,
    //    Customer,
    //    DateTime,
    //    Decimal,
    //    Double,
    //    Float,
    //    Integer,
    //    Lookup,
    //    Memo,
    //    Money,
    //    Owner,
    //    PartyList,
    //    Picklist,
    //    State,
    //    Status,
    //    String,
    //    UniqueIdentifier,
    //    CalendarRules,
    //    Virtual,
    //    BigInt,
    //    ManagedProperty,
    //    EntityName,
    //    Internal,
    //    PrimaryKey,
    //    Empty,
    //    Unknown,
    #endregion

    #region sequence from sc.net
    Boolean,
    Customer,
    DateTime,
    Decimal,

    Float,
    Integer,
    Internal,
    Lookup,
    Memo,
    Money,
    Owner,
    PartyList,
    Picklist,
    PrimaryKey,
    State,
    Status,
    String,
    UniqueIdentifier,

    Virtual,
    CalendarRules,

    Double,
    BigInt,
    ManagedProperty,
    EntityName,
    Empty,
    Unknown
    #endregion
  }
}