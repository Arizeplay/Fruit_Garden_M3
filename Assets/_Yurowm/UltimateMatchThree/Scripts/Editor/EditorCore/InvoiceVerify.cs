// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.InvoiceVerify
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using Yurowm.EditorCore.Utils;

namespace Yurowm.EditorCore
{
  internal class InvoiceVerify : NWProtocol
  {
    private InvoiceVerify.Result result;
    private string accessKey;
    private int project_id;
    private string device;
    private string bundle;
    private string name;
    private string company;
    private int teamSize;
    private string email;

    public InvoiceVerify(SocketConnection connection = null)
      : base(connection)
    {
    }

    public override object GetResult() => (object) this.result;

    public void Initialize(
      string invoice,
      int project_id,
      string device,
      string bundle,
      string name = null,
      string company = null,
      int teamSize = 0,
      string email = null)
    {
      this.accessKey = invoice;
      this.project_id = project_id;
      this.device = device;
      this.bundle = bundle;
      this.name = name;
      this.company = company;
      this.teamSize = teamSize;
      this.email = email;
    }

    public override void Logic(SocketConnection connection, NWMessage message = null)
    {
      message = new NWMessage(this.GetType());
      message.Add((NWEntry) new NWEntryString("accessKey", this.accessKey));
      message.Add((NWEntry) new NWEntryInteger("project_id", this.project_id));
      message.Add((NWEntry) new NWEntryString("device", this.device));
      message.Add((NWEntry) new NWEntryString("bundle", this.bundle));
      if (!string.IsNullOrEmpty(this.email))
      {
        message.Add((NWEntry) new NWEntryString("name", this.name));
        message.Add((NWEntry) new NWEntryString("company", this.company));
        message.Add((NWEntry) new NWEntryString("email", this.email));
        message.Add((NWEntry) new NWEntryInteger("teamSize", this.teamSize));
      }
      connection.Send(message);
      message = connection.ReceiveMessage();
      this.result = new InvoiceVerify.Result();
      if (message.GetResult() == Yurowm.EditorCore.Utils.Result.Success)
      {
        this.result.success = true;
        this.result.message = Yurowm.EditorCore.Utils.Result.Success.ToString();
        this.result.fixerLevel = (int) message["fixerLevel"];
        if (message.ContainsName("core"))
          this.result.lastCoreVersion = (int) message["core"];
        if (!message.ContainsName("accessKey"))
          return;
        this.result.accessKey = (string) message["accessKey"];
      }
      else
      {
        this.result.success = false;
        this.result.failed = message.GetResult() == Yurowm.EditorCore.Utils.Result.Failed;
        this.result.message = string.Format("{0}: {1}", (object) message.GetResult(), (object) message.GetReason());
      }
    }

    public class Result
    {
      public bool success;
      public bool failed;
      public string message;
      public int fixerLevel;
      public int lastCoreVersion;
      public string accessKey;
    }
  }
}
