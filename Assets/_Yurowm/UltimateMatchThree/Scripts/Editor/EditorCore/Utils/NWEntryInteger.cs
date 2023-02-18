// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryInteger
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryInteger : NWEntry
  {
    public NWEntryInteger(string name, int i)
      : base(name, (object) i)
    {
    }

    public override string type => "i";

    public override string ToRaw() => this.value.ToString();

    public override void Parse(string raw) => this.value = (object) int.Parse(raw);
  }
}
