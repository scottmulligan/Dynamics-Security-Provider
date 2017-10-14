namespace CRMSecurityProvider.Repository.V4
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Net;
  using System.Text;
  using System.Xml;

  public class LiveIdTicketManager
  {
    // Fields
    private static string _authorizationEndpoint;
    private static LiveIdEnvironment _environment;
    internal const string DeviceRegistrationTemplate = "<DeviceAddRequest>\r\n            <ClientInfo name=\"{0:clientId}\" version=\"1.0\"/>\r\n            <Authentication>\r\n                <Membername>{1:prefix}{2:deviceName}</Membername>\r\n                <Password>{3:password}</Password>\r\n            </Authentication>\r\n        </DeviceAddRequest>";
    private const string deviceTokenResponseFileName = "DeviceTokenResponse.xml";
    private static string deviceTokenResponseFilePath = string.Empty;
    internal const string DeviceTokenTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n        <s:Envelope \r\n          xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" \r\n          xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" \r\n          xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" \r\n          xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n          xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" \r\n          xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n           <s:Header>\r\n            <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n            <wsa:To s:mustUnderstand=\"1\">http://Passport.NET/tb</wsa:To>    \r\n            <wsse:Security>\r\n              <wsse:UsernameToken wsu:Id=\"devicesoftware\">\r\n                <wsse:Username>{0:prefix}{1:deviceName}</wsse:Username>\r\n                <wsse:Password>{2:password}</wsse:Password>\r\n              </wsse:UsernameToken>\r\n            </wsse:Security>\r\n          </s:Header>\r\n          <s:Body>\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                 <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                 <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                       <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                 </wsp:AppliesTo>\r\n              </wst:RequestSecurityToken>\r\n          </s:Body>\r\n        </s:Envelope>\r\n        ";
    private static string[] federationMetadataUrl = new string[] { "https://nexus.passport.com/federationmetadata/2006-12/FederationMetaData.xml", "https://nexus.passport-int.com/federationmetadata/2006-12/FederationMetaData.xml" };
    private const string RequestSecurityTokenExpiryResponseXPath = "S:Envelope/S:Body/wst:RequestSecurityTokenResponse";
    private const string securityTokenResponseFileName = "UserTokenResponse.xml";
    private static string securityTokenResponseFilePath = string.Empty;
    private const string TokenExpiryXPath = "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:Lifetime/wsu:Expires";
    private const string TokenResponseXPath = "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken";
    internal const string UserTokenTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n    <s:Envelope \r\n      xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" \r\n      xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" \r\n      xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" \r\n      xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n      xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" \r\n      xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n       <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">http://Passport.NET/tb</wsa:To>    \r\n       <ps:AuthInfo Id=\"PPAuthInfo\" xmlns:ps=\"http://schemas.microsoft.com/LiveID/SoapServices/v1\">\r\n             <ps:HostingApp>{0:clientId}</ps:HostingApp>\r\n          </ps:AuthInfo>\r\n          <wsse:Security>\r\n             <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{1:userName}</wsse:Username>\r\n                <wsse:Password>{2:password}</wsse:Password>\r\n             </wsse:UsernameToken>\r\n             <wsse:BinarySecurityToken ValueType=\"urn:liveid:device\">\r\n               {3:deviceTokenValue}\r\n             </wsse:BinarySecurityToken>\r\n          </wsse:Security>\r\n      </s:Header>\r\n      <s:Body>\r\n        <wst:RequestSecurityToken Id=\"RST0\">\r\n             <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n             <wsp:AppliesTo>\r\n                <wsa:EndpointReference>\r\n                   <wsa:Address>{4:partnerUrl}</wsa:Address>\r\n                </wsa:EndpointReference>\r\n             </wsp:AppliesTo>\r\n             <wsp:PolicyReference URI=\"{5:policy}\"/>\r\n          </wst:RequestSecurityToken>\r\n      </s:Body>\r\n    </s:Envelope>\r\n    ";
    private static string[] windowsLiveDeviceUrl = new string[] { "https://login.live.com/ppsecure/DeviceAddCredential.srf", "https://login.live-int.com/ppsecure/DeviceAddCredential.srf" };

    // Methods
    private LiveIdTicketManager()
    {
    }

    private static XmlDocument CallLiveIdServices(string serviceUrl, string method, string soapMessageEnvelope)
    {
      string str2;
      string requestUriString = serviceUrl;
      WebRequest request = WebRequest.Create(requestUriString);
      request.Method = method;
      request.Timeout = 0x2bf20;
      if (method == "POST")
      {
        request.ContentType = "application/soap+xml; charset=UTF-8";
      }
      if (!string.IsNullOrEmpty(soapMessageEnvelope))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(soapMessageEnvelope);
        using (Stream stream = request.GetRequestStream())
        {
          stream.Write(bytes, 0, bytes.Length);
          stream.Close();
        }
      }
      using (WebResponse response = request.GetResponse())
      {
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          str2 = reader.ReadToEnd();
        }
        response.Close();
      }
      XmlDocument document = new XmlDocument();
      document.LoadXml(str2);
      return document;
    }

    private static XmlNamespaceManager CreateNamespaceManager(XmlNameTable nameTable)
    {
      XmlNamespaceManager manager = new XmlNamespaceManager(nameTable);
      manager.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2005/02/trust");
      manager.AddNamespace("S", "http://www.w3.org/2003/05/soap-envelope");
      manager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
      manager.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
      return manager;
    }

    private static string GetServiceEndpoint(LiveIdEnvironment environment)
    {
      XmlDocument document = CallLiveIdServices(federationMetadataUrl[(int)environment], "GET", null);
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
      namespaceManager.AddNamespace("fed", "http://schemas.xmlsoap.org/ws/2006/03/federation");
      namespaceManager.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
      return SelectNode(document, namespaceManager, "//fed:FederationMetadata/fed:Federation/fed:TargetServiceEndpoint/wsa:Address").InnerText.Trim();
    }

    private static bool IsResponseValid(XmlDocument doc, XmlNamespaceManager namespaceManager, string source)
    {
      string xPathToNode = "S:Envelope/S:Body/S:Fault";
      if (null != SelectNode(doc, namespaceManager, xPathToNode))
      {
        string message = string.Empty;
        string innerText = string.Empty;
        string str4 = "S:Envelope/S:Body/S:Fault/S:Reason/S:Text";
        string str5 = "S:Envelope/S:Body/S:Fault/S:Detail/psf:error/psf:internalerror";
        message = SelectNode(doc, namespaceManager, str4).InnerText;
        message = (source != "") ? (source + ": " + message) : message;
        if (null != SelectNode(doc, namespaceManager, str5))
        {
          innerText = SelectNode(doc, namespaceManager, str5 + "//psf:text").InnerText;
          throw new Exception(message, new Exception(innerText));
        }
        throw new Exception(message);
      }
      return true;
    }

    private static string Issue(string userName, string password, string partner, string policy, Guid clientId, string deviceToken)
    {
      string soapMessageEnvelope = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n    <s:Envelope \r\n      xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" \r\n      xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" \r\n      xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" \r\n      xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n      xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" \r\n      xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n       <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">http://Passport.NET/tb</wsa:To>    \r\n       <ps:AuthInfo Id=\"PPAuthInfo\" xmlns:ps=\"http://schemas.microsoft.com/LiveID/SoapServices/v1\">\r\n             <ps:HostingApp>{0:clientId}</ps:HostingApp>\r\n          </ps:AuthInfo>\r\n          <wsse:Security>\r\n             <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{1:userName}</wsse:Username>\r\n                <wsse:Password>{2:password}</wsse:Password>\r\n             </wsse:UsernameToken>\r\n             <wsse:BinarySecurityToken ValueType=\"urn:liveid:device\">\r\n               {3:deviceTokenValue}\r\n             </wsse:BinarySecurityToken>\r\n          </wsse:Security>\r\n      </s:Header>\r\n      <s:Body>\r\n        <wst:RequestSecurityToken Id=\"RST0\">\r\n             <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n             <wsp:AppliesTo>\r\n                <wsa:EndpointReference>\r\n                   <wsa:Address>{4:partnerUrl}</wsa:Address>\r\n                </wsa:EndpointReference>\r\n             </wsp:AppliesTo>\r\n             <wsp:PolicyReference URI=\"{5:policy}\"/>\r\n          </wst:RequestSecurityToken>\r\n      </s:Body>\r\n    </s:Envelope>\r\n    ", new object[] { clientId.ToString(), userName, password, deviceToken, partner, policy });
      XmlDocument doc = CallLiveIdServices(_authorizationEndpoint, "POST", soapMessageEnvelope);
      XmlNamespaceManager namespaceManager = CreateNamespaceManager(doc.NameTable);
      if (IsResponseValid(doc, namespaceManager, "IssueTicket"))
      {
        XmlNode node = SelectNode(doc, namespaceManager, "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken");
        if (node != null)
        {
          return node.InnerText;
        }
      }
      return string.Empty;
    }

    private static string IssueDeviceToken(string deviceName, string devicePassword)
    {
      string soapMessageEnvelope = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n        <s:Envelope \r\n          xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" \r\n          xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" \r\n          xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" \r\n          xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n          xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" \r\n          xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n           <s:Header>\r\n            <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n            <wsa:To s:mustUnderstand=\"1\">http://Passport.NET/tb</wsa:To>    \r\n            <wsse:Security>\r\n              <wsse:UsernameToken wsu:Id=\"devicesoftware\">\r\n                <wsse:Username>{0:prefix}{1:deviceName}</wsse:Username>\r\n                <wsse:Password>{2:password}</wsse:Password>\r\n              </wsse:UsernameToken>\r\n            </wsse:Security>\r\n          </s:Header>\r\n          <s:Body>\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                 <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                 <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                       <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                 </wsp:AppliesTo>\r\n              </wst:RequestSecurityToken>\r\n          </s:Body>\r\n        </s:Envelope>\r\n        ", new object[] { "11", deviceName, devicePassword });
      XmlDocument doc = CallLiveIdServices(_authorizationEndpoint, "POST", soapMessageEnvelope);
      XmlNamespaceManager namespaceManager = CreateNamespaceManager(doc.NameTable);
      if (IsResponseValid(doc, namespaceManager, "IssueDeviceToken"))
      {
        return SelectNode(doc, namespaceManager, "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken").InnerXml;
      }
      return string.Empty;
    }

    private static XmlNodeList RegisterMachine(string deviceName, string devicePassword, Guid clientId)
    {
      try
      {
        string soapMessageEnvelope = string.Format(CultureInfo.InvariantCulture, "<DeviceAddRequest>\r\n            <ClientInfo name=\"{0:clientId}\" version=\"1.0\"/>\r\n            <Authentication>\r\n                <Membername>{1:prefix}{2:deviceName}</Membername>\r\n                <Password>{3:password}</Password>\r\n            </Authentication>\r\n        </DeviceAddRequest>", new object[] { clientId.ToString(), "11", deviceName, devicePassword });
        XmlDocument document = CallLiveIdServices(windowsLiveDeviceUrl[(int)_environment], "POST", soapMessageEnvelope);
        if (document != null)
        {
          XmlNodeList list = document.SelectNodes("DeviceAddResponse");
          if ((list != null) && (list.Count > 0))
          {
            bool flag2 = false;
            if (list[0].Attributes[0].Value == flag2.ToString(CultureInfo.InvariantCulture))
            {
            }
          }
          return list;
        }
      }
      catch (WebException exception)
      {
        if (!exception.Message.Contains("Bad Request"))
        {
          throw;
        }
      }
      return null;
    }

    public static string RetrieveTicket(string devicename, string devicepassword, string partner, string username, string password, string policy, LiveIdEnvironment environment)
    {
      return RetrieveTicket(devicename, devicepassword, partner, username, password, policy, environment, "");
    }

    public static string RetrieveTicket(string devicename, string devicepassword, string partner, string username, string password, string policy, LiveIdEnvironment environment, string ticketPath)
    {
      if (ticketPath != "")
      {
        deviceTokenResponseFilePath = ticketPath + @"\DeviceTokenResponse.xml";
        securityTokenResponseFilePath = ticketPath + @"\UserTokenResponse.xml";
      }
      else
      {
        deviceTokenResponseFilePath = "DeviceTokenResponse.xml";
        securityTokenResponseFilePath = "UserTokenResponse.xml";
      }
      _environment = environment;
      SetAuthorizationEndpoint(_environment);
      Guid clientId = Guid.NewGuid();
      string token = string.Empty;
      string str2 = string.Empty;
      if (TokenExist(securityTokenResponseFilePath, out str2))
      {
        return str2;
      }
      if (!TokenExist(deviceTokenResponseFilePath, out token))
      {
        XmlNodeList list = RegisterMachine(devicename, devicepassword, clientId);
        token = IssueDeviceToken(devicename, devicepassword);
      }
      return Issue(username, password, partner, policy, clientId, token);
    }

    private static XmlNode SelectNode(XmlDocument document, XmlNamespaceManager namespaceManager, string xPathToNode)
    {
      XmlNodeList list = document.SelectNodes(xPathToNode, namespaceManager);
      if (((list != null) && (list.Count > 0)) && (list[0] != null))
      {
        return list[0];
      }
      return null;
    }

    private static void SetAuthorizationEndpoint(LiveIdEnvironment environment)
    {
      if (string.IsNullOrEmpty(_authorizationEndpoint))
      {
        _authorizationEndpoint = GetServiceEndpoint(environment);
      }
    }

    private static bool TokenExist(string filePath, out string token)
    {
      bool flag = false;
      token = string.Empty;
      if (!File.Exists(filePath))
      {
        return flag;
      }
      XmlDocument document = new XmlDocument();
      document.Load(filePath);
      XmlNamespaceManager namespaceManager = CreateNamespaceManager(document.NameTable);
      string innerXml = SelectNode(document, namespaceManager, "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:Lifetime/wsu:Expires").InnerXml;
      string[] strArray = innerXml.Split(new char[] { 'T' })[0].Split(new char[] { '-' });
      string[] strArray2 = innerXml.Split(new char[] { 'T' })[1].Split(new char[] { 'Z' })[0].Split(new char[] { ':' });
      DateTime time = new DateTime(int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2]), int.Parse(strArray2[0]), int.Parse(strArray2[1]), int.Parse(strArray2[2]));
      if (DateTime.UtcNow.CompareTo(time) >= 0)
      {
        return flag;
      }
      XmlNode node = SelectNode(document, namespaceManager, "S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken");
      if (node == null)
      {
        return flag;
      }
      if (filePath == deviceTokenResponseFilePath)
      {
        token = node.InnerXml;
      }
      else
      {
        token = node.InnerText;
      }
      return true;
    }

    // Nested Types
    public enum LiveIdEnvironment
    {
      PROD,
      INT
    }
  }
}