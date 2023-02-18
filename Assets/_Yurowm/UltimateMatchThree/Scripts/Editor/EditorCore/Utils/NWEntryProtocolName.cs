// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryProtocolName
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryProtocolName : NWEntry
  {
    public NWEntryProtocolName()
      : base("protocol", (object) null)
    {
    }

    public NWEntryProtocolName(Type type)
      : base("protocol", (object) type)
    {
      this.value = (object) type;
    }

    public override string type => "pn";

    public NWProtocol GetProtocol(SocketConnection connection)
    {
      if (this.value == null)
        return (NWProtocol) null;
      return (NWProtocol) Activator.CreateInstance((Type) this.value, (object) connection);
    }

    public override string ToRaw() => ((Type) this.value).Name;

    public override void Parse(string raw)
    {
      Type type = typeof (NWProtocol).Assembly.GetType("Yurowm.EditorCore." + raw);
      if (type == null)
        return;
      this.value = (object) type;
    }
  }
}
