// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.Pair
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;

namespace Yurowm.GameCore
{
  [Serializable]
  public class Pair
  {
    public string a;
    public string b;

    public Pair(string pa, string pb)
    {
      this.a = pa;
      this.b = pb;
    }

    public static bool operator ==(Pair a, Pair b) => object.Equals((object) a, (object) b);

    public static bool operator !=(Pair a, Pair b) => !object.Equals((object) a, (object) b);

    public override bool Equals(object obj)
    {
      Pair pair = (Pair) obj;
      if (this.a.Equals(pair.a) && this.b.Equals(pair.b))
        return true;
      return this.a.Equals(pair.b) && this.b.Equals(pair.a);
    }

    public override int GetHashCode() => this.a.GetHashCode() + this.b.GetHashCode();
  }
}
