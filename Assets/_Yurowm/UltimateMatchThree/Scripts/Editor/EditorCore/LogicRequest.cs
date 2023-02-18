// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.LogicRequest
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using Yurowm.EditorCore.Utils;

namespace Yurowm.EditorCore
{
  internal class LogicRequest : NWProtocol
  {
    private LogicRequest.Result result;
    private string method;
    private string parameters;
    private string password;

    public LogicRequest(SocketConnection connection = null)
      : base(connection)
    {
    }

    public override object GetResult() => (object) this.result;

    public void Initialize(string method, string parameters, string password)
    {
      this.method = method;
      this.parameters = parameters;
      this.password = password;
    }

    public override void Logic(SocketConnection connection, NWMessage message = null)
    {
      message = new NWMessage(this.GetType());
      if (!string.IsNullOrEmpty(this.password))
        message.Add((NWEntry) new NWEntryString("master_password", this.password));
      connection.Send(message);
      message = connection.ReceiveMessage();
      if (message.GetResult() != Yurowm.EditorCore.Utils.Result.Success)
      {
        this.result = new LogicRequest.Result();
        this.result.result = string.Format("{0}: {1}", (object) message.GetResult(), (object) message.GetReason());
      }
      else
      {
        message = new NWMessage();
        message.Add((NWEntry) new NWEntryString("method", this.method));
        message.Add((NWEntry) new NWEntryString("parameters", this.parameters));
        connection.Send(message);
        message = connection.ReceiveMessage();
        this.result = new LogicRequest.Result();
        if (message.GetResult() == Yurowm.EditorCore.Utils.Result.Success)
          this.result.result = (string) message["result"];
        else
          this.result.error = (string) message["error"];
      }
    }

    public class Result
    {
      public string result;
      public string error;
    }
  }
}
