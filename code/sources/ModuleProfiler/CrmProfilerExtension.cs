namespace CRMSecurityProvider
{
  using System;
  using System.IO;
  using System.Web.Services.Protocols;

  using CRMSecurityProvider.Configuration;

  public class CrmProfilerExtension : SoapExtension
  {
    private Stream oldStream;
    private Stream newStream;

    public static int CallCount { get; private set; }

    public static decimal TotalRequestLength { get; private set; }

    public static decimal TotalResponseLength { get; private set; }

    public static void Reset()
    {
      CallCount = 0;
      TotalRequestLength = 0;
      TotalResponseLength = 0;
    }

    public override Stream ChainStream(Stream stream)
    {
      if (Settings.CrmAccessProfiling)
      {
        this.oldStream = stream;
        this.newStream = new MemoryStream();

        return this.newStream;
      }

      return stream;
    }

    public override object GetInitializer(Type serviceType)
    {
      return null;
    }

    public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
    {
      return null;
    }

    public override void Initialize(object initializer)
    {
    }

    public override void ProcessMessage(SoapMessage message)
    {
      if (Settings.CrmAccessProfiling)
      {
        switch (message.Stage)
        {
          case SoapMessageStage.BeforeSerialize:
            CallCount++;
            break;
          case SoapMessageStage.AfterSerialize:
            TotalRequestLength += ((decimal)this.newStream.Length / 1024);
            this.newStream.Position = 0;
            this.Copy(this.newStream, this.oldStream);
            break;
          case SoapMessageStage.BeforeDeserialize:
            this.Copy(this.oldStream, this.newStream);
            newStream.Position = 0;
            TotalResponseLength += ((decimal)this.newStream.Length / 1024);
            break;
          case SoapMessageStage.AfterDeserialize:
            break;
        }
      }
    }

    private void Copy(Stream from, Stream to)
    {
      var reader = new StreamReader(from);
      var writer = new StreamWriter(to);

      writer.Write(reader.ReadToEnd());
      writer.Flush();
    }
  }
}
