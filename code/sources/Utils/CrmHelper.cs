namespace CRMSecurityProvider.Utils
{
  using System;
  using System.Linq;
  using System.Net;
  using System.Security.Cryptography;
  using System.Text;

  using Sitecore.Web.UI.Sheer;

  /// <summary>
  /// The helper class.
  /// </summary>
  public static class CrmHelper
  {
    /// <summary>
    /// Shows the message in UI or in log if the UI output is disabled.
    /// </summary>
    /// <param name="message">The message to show.</param>
    public static void ShowMessage(string message)
    {
      if (SheerResponse.OutputEnabled)
      {
        SheerResponse.Alert(message);
      }
      else
      {
        ConditionalLog.Error(message, message);
      }
    }

    /// <summary>
    /// Creates the network credential.
    /// </summary>
    /// <param name="login">The login.</param>
    /// <param name="password">The password.</param>
    /// <returns>The network credential</returns>
    public static NetworkCredential CreateNetworkCredential(string login, string password)
    {
      if (login.IndexOf(@"\", StringComparison.OrdinalIgnoreCase) > 0)
      {
        var parts = login.Split(new[] { @"\" }, 2, StringSplitOptions.None);
        return new NetworkCredential(parts[1], password, parts[0]);
      }

      return new NetworkCredential(login, password);
    }

    /// <summary>
    /// Gets the name of the connection group.
    /// </summary>
    /// <param name="credential">The network credential.</param>
    /// <returns>The connection group name.</returns>
    public static string GetConnectionGroupName(NetworkCredential credential)
    {
      var toEncode = credential.UserName + credential.Password + credential.Domain;
      var bytesToEncode = Encoding.UTF8.GetBytes(toEncode);

      var sha1Managed = new SHA1Managed();
      var encodedBytes = sha1Managed.ComputeHash(bytesToEncode);
      return Encoding.Default.GetString(encodedBytes);
    }

    public static crm3.webservice.Property ByName(this crm3.webservice.Property[] properties, string propertyName)
    {
      return properties.FirstOrDefault(property => property.Name == propertyName);
    }

    public static crm4.webservice.Property ByName(this crm4.webservice.Property[] properties, string propertyName)
    {
      return properties.FirstOrDefault(property => property.Name == propertyName);
    }
  }
}
