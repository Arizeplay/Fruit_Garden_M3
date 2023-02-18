// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryString
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryString : NWEntry
  {
    public NWEntryString(string name, string s)
      : base(name, (object) s)
    {
    }

    public override string type => "s";

    public override string ToRaw() => (string) this.value;

    public override void Parse(string raw) => this.value = (object) raw;
  }
}
