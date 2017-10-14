namespace CRMSecurityProvider.Configuration
{
  using System;
  
  using Sitecore;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The configuration settings class.
  /// </summary>
  public class ConfigurationSettings
  {
    #region Properties

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public ApiVersion ApiVersion { get; private set; }

    /// <summary>
    /// Gets or sets the type of the authentication.
    /// </summary>
    public AuthenticationType AuthenticationType { get; private set; }

    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    public string Environment { get; private set; }

    /// <summary>
    /// Gets or sets the organization.
    /// </summary>
    public string Organization { get; private set; }

    /// <summary>
    /// Gets or sets the partner.
    /// </summary>
    public string Partner { get; private set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether [pre authenticate].
    /// </summary>
    public bool PreAuthenticate { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether [unsafe authenticated connection sharing].
    /// </summary>
    public bool UnsafeAuthenticatedConnectionSharing { get; private set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public string User { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Parses the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The configuration settings.</returns>
    public static ConfigurationSettings Parse(string connectionString)
    {
      Assert.ArgumentNotNullOrEmpty(connectionString, "connectionString");

      if (!connectionString.StartsWith("crm:", StringComparison.InvariantCultureIgnoreCase))
      {
        throw new NotSupportedException("Connection string is not supported: " + connectionString);
      }

      connectionString = connectionString.Substring(4);
      var connectionValues = StringUtil.GetNameValues(connectionString, '=', ';');

      var settings = new ConfigurationSettings();

      settings.Url = connectionValues["url"];
      if (String.IsNullOrEmpty(settings.Url))
      {
        throw new NotSupportedException("Connection string without 'url' is not supported.");
      }
      settings.Url = settings.Url.ToLower();

      settings.User = connectionValues["user id"];
      if (String.IsNullOrEmpty(settings.User))
      {
        throw new NotSupportedException("Connection string without 'user id' is not supported.");
      }

      settings.Password = connectionValues["password"];
      if (String.IsNullOrEmpty(settings.Password))
      {
        throw new NotSupportedException("Connection string without 'password' is not supported.");
      }

      settings.Organization = connectionValues["organization"];
      if (String.IsNullOrEmpty(settings.Organization))
      {
        throw new NotSupportedException("Connection string without 'organization' is not supported.");
      }

      if (settings.Url.Contains("/2006/"))
      {
        settings.ApiVersion = ApiVersion.V3;
      }
      else if (settings.Url.Contains("/2007/"))
      {
        settings.ApiVersion = ApiVersion.V4;
      }
      else if (settings.Url.Contains("/2011/"))
      {
        settings.ApiVersion = ApiVersion.V5;
      }
      else
      {
        throw new NotSupportedException("Connection string is not supported: " + connectionString);
      }

      var stringAuthenticationType = connectionValues["authentication type"];
      if (String.IsNullOrEmpty(stringAuthenticationType))
      {
        throw new NotSupportedException("Connection string without 'authentication type' is not supported.");
      }
      int authenticationType;
      if (!Int32.TryParse(stringAuthenticationType, out authenticationType))
      {
        throw new ArgumentException("Connection string contains wrong 'authentication type'.");
      }
      settings.AuthenticationType = (AuthenticationType)authenticationType;

      settings.Partner = connectionValues["partner"] ?? String.Empty;
      settings.Environment = connectionValues["environment"] ?? String.Empty;
      settings.UnsafeAuthenticatedConnectionSharing = MainUtil.GetBool(connectionValues["unsafeauthenticatedconnectionsharing"], true);
      settings.PreAuthenticate = MainUtil.GetBool(connectionValues["preauthenticate"], true);

      return settings;
    }

    #endregion
  }
}