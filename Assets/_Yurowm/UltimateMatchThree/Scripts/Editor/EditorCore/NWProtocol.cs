// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.NWProtocol
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using Yurowm.EditorCore.Utils;

namespace Yurowm.EditorCore
{
  internal abstract class NWProtocol
  {
    private SocketConnection connection;

    public NWProtocol(SocketConnection connection = null) => this.connection = connection;

    public string ProtocolName() => this.GetType().Name;

    public void Execute(NWMessage request_message = null)
    {
      try
      {
        if (this.connection == null)
        {
          using (this.connection = new SocketConnection())
            this.Logic(this.connection, request_message);
        }
        else
          this.Logic(this.connection, request_message);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
    }

    public abstract object GetResult();

    public abstract void Logic(SocketConnection connection, NWMessage message = null);
  }
}
