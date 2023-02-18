// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntryResult
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yurowm.EditorCore.Utils
{
  internal class NWEntryResult : NWEntry
  {
    private Result result;
    private Reason reason;

    public NWEntryResult(Result result = Result.Success, Reason reason = Reason.Unknown)
      : base("", (object) "")
    {
      this.result = result;
      this.reason = reason;
    }

    public override string type => "r";

    public override void Parse(string raw)
    {
      int[] array = ((IEnumerable<string>) raw.Split(',')).Select<string, int>((Func<string, int>) (x => int.Parse(x))).ToArray<int>();
      this.result = (Result) array[0];
      this.reason = (Reason) array[1];
    }

    public override string ToRaw()
    {
      int num = (int) this.result;
      string str1 = num.ToString();
      num = (int) this.reason;
      string str2 = num.ToString();
      return str1 + "," + str2;
    }

    public Result GetResult() => this.result;

    public Reason GetReason() => this.reason;
  }
}
