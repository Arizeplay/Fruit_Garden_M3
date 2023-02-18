// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryBool
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryBool : NWEntry
  {
    public NWEntryBool(string name, bool b)
      : base(name, (object) b)
    {
    }

    public override string type => "b";

    public override string ToRaw() => !(bool) this.value ? "0" : "1";

    public override void Parse(string raw) => this.value = (object) (raw == "1");
  }
}
