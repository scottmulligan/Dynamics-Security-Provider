namespace CRMSecurityProvider
{
  /// <summary>
  /// The enumeration of the supported CRM data types.
  /// </summary>
  public enum SupportedTypes : byte
  {
    /// <summary>
    /// Represents string property.
    /// </summary>
    String,

    /// <summary>
    /// Represents boolean property.
    /// </summary>
    CrmBoolean,

    /// <summary>
    /// Represents DateTime property.
    /// </summary>
    CrmDateTime,

    /// <summary>
    /// Represents int property.
    /// </summary>
    CrmNumber,

    /// <summary>
    /// Represents float property.
    /// </summary>
    CrmFloat,

    /// <summary>
    /// Represents decimal property.
    /// </summary>
    CrmDecimal,

    /// <summary>
    /// Represents money property.
    /// </summary>
    CrmMoney,

    /// <summary>
    /// Represents picklist property.
    /// </summary>
    Picklist
  }
}
