// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.Pair`1
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

namespace Yurowm.GameCore
{
  public class Pair<T>
  {
    public T a;
    public T b;

    public Pair(T pa, T pb)
    {
      this.a = pa;
      this.b = pb;
    }

    public static bool operator ==(Pair<T> a, Pair<T> b) => object.Equals((object) a, (object) b);

    public static bool operator !=(Pair<T> a, Pair<T> b) => !object.Equals((object) a, (object) b);

    public override bool Equals(object obj)
    {
      Pair<T> pair = (Pair<T>) obj;
      if (this.a.Equals((object) pair.a) && this.b.Equals((object) pair.b))
        return true;
      return this.a.Equals((object) pair.b) && this.b.Equals((object) pair.a);
    }

    public override int GetHashCode() => this.a.GetHashCode() + this.b.GetHashCode();
  }
}
