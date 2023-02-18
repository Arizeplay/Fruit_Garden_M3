// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryDateTime
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryDateTime : NWEntry
  {
    public NWEntryDateTime(string name, DateTime dt)
      : base(name, (object) dt)
    {
    }

    public override string type => "dt";

    public override string ToRaw() => ((DateTime) this.value).Ticks.ToString();

    public override void Parse(string raw) => this.value = (object) new DateTime(long.Parse(raw));
  }
}
