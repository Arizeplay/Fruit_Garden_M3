// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.int2
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.GameCore
{
  [Serializable]
  public class int2
  {
    public static readonly int2 zero = new int2(0, 0);
    public static readonly int2 right = new int2(1, 0);
    public static readonly int2 up = new int2(0, 1);
    public static readonly int2 left = new int2(-1, 0);
    public static readonly int2 down = new int2(0, -1);
    public static readonly int2 one = new int2(1, 1);
    public int x;
    public int y;
    private static Regex parser = new Regex("\\((?<x>-?\\d+)\\,\\s*(?<y>-?\\d+)\\)");

    public int2(int _x, int _y)
    {
      this.x = _x;
      this.y = _y;
    }

    public int2()
    {
      this.x = 0;
      this.y = 0;
    }

    public int2(int2 coord)
    {
      this.x = coord.x;
      this.y = coord.y;
    }

    public static bool operator ==(int2 a, int2 b) => (object) a == null ? (object) b == null : a.Equals((object) b);

    public static bool operator !=(int2 a, int2 b) => (object) a == null ? b != null : !a.Equals((object) b);

    public static int2 operator *(int2 a, int b) => new int2(a.x * b, a.y * b);

    public static int2 operator *(int b, int2 a) => a * b;

    public static int2 operator +(int2 a, int2 b) => new int2(a.x + b.x, a.y + b.y);

    public static int2 operator -(int2 a, int2 b) => new int2(a.x - b.x, a.y - b.y);

    public static int2 operator +(int2 a, Side side) => a + side.ToInt2();

    public static int2 operator -(int2 a, Side side) => a + side.ToInt2();

    public bool IsItHit(int min_x, int min_y, int max_x, int max_y) => this.x >= min_x && this.x <= max_x && this.y >= min_y && this.y <= max_y;

    public int FourSideDistanceTo(int2 destination) => Mathf.Abs(this.x - destination.x) + Mathf.Abs(this.y - destination.y);

    public int EightSideDistanceTo(int2 destination) => Mathf.Max(Mathf.Abs(this.x - destination.x), Mathf.Abs(this.y - destination.y));

    public override bool Equals(object obj)
    {
      if (obj == null || (object) (obj as int2) == null)
        return false;
      int2 int2 = (int2) obj;
      return this.x == int2.x && this.y == int2.y;
    }

    public override int GetHashCode() => string.Format("{0},{1}", (object) this.x, (object) this.y).GetHashCode();

    public override string ToString() => string.Format("({0}, {1})", (object) this.x, (object) this.y);

    public static int2 Parse(string raw)
    {
      Match match = int2.parser.Match(raw);
      if (match.Success)
        return new int2(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
      throw new FormatException("Can't to parse \"" + raw + "\" to int2 format. It must have next format: (int,int)");
    }

    public int2 XtoY() => new int2(this.y, this.x);

    public int2 GetClone() => (int2) this.MemberwiseClone();

    public static explicit operator Vector2(int2 coord) => new Vector2((float) coord.x, (float) coord.y);

    public Vector3 ToVector(Asix2 plane, bool inverse = false)
    {
      switch (plane)
      {
        case Asix2.XY:
          return !inverse ? new Vector3((float) this.x, (float) this.y, 0.0f) : new Vector3((float) this.y, (float) this.x, 0.0f);
        case Asix2.XZ:
          return !inverse ? new Vector3((float) this.x, 0.0f, (float) this.y) : new Vector3((float) this.y, 0.0f, (float) this.x);
        case Asix2.YZ:
          return !inverse ? new Vector3(0.0f, (float) this.y, (float) this.x) : new Vector3(0.0f, (float) this.x, (float) this.y);
        default:
          return Vector3.zero;
      }
    }
  }
}
