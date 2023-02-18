// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.int3
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.GameCore
{
  [Serializable]
  public class int3
  {
    public static readonly int3 zero = new int3(0, 0, 0);
    public static readonly int3 right = new int3(1, 0, 0);
    public static readonly int3 up = new int3(0, 1, 0);
    public static readonly int3 left = new int3(-1, 0, 0);
    public static readonly int3 down = new int3(0, -1, 0);
    public static readonly int3 forward = new int3(0, 0, 1);
    public static readonly int3 back = new int3(0, 0, -1);
    public static readonly int3 one = new int3(1, 1, 1);
    public int x;
    public int y;
    public int z;
    private static Regex parser = new Regex("\\((?<x>-?\\d+)\\,\\s*(?<y>-?\\d+)\\,\\s*(?<z>-?\\d+)\\)");

    public int3(int _x, int _y, int _z)
    {
      this.x = _x;
      this.y = _y;
      this.z = _z;
    }

    public int3()
      : this(0, 0, 0)
    {
    }

    public int3(int3 coord)
    {
      this.x = coord.x;
      this.y = coord.y;
      this.z = coord.z;
    }

    public static bool operator ==(int3 a, int3 b) => (object) a == null ? (object) b == null : a.Equals((object) b);

    public static bool operator !=(int3 a, int3 b) => (object) a == null ? b != null : !a.Equals((object) b);

    public static int3 operator *(int3 a, int b) => new int3(a.x * b, a.y * b, a.z * b);

    public static int3 operator *(int b, int3 a) => a * b;

    public static int3 operator +(int3 a, int3 b) => new int3(a.x + b.x, a.y + b.y, a.z + b.z);

    public static int3 operator -(int3 a, int3 b) => new int3(a.x - b.x, a.y - b.y, a.z - b.z);

    public bool IsItHit(int min_x, int min_y, int min_z, int max_x, int max_y, int max_z) => this.x >= min_x && this.x <= max_x && this.y >= min_y && this.y <= max_y && this.z >= min_z && this.z <= max_z;

    public override bool Equals(object obj)
    {
      if (obj == null || (object) (obj as int3) == null)
        return false;
      int3 int3 = (int3) obj;
      return this.x == int3.x && this.y == int3.y && this.z == int3.z;
    }

    public override int GetHashCode() => string.Format("{0},{1},{2}", (object) this.x, (object) this.y, (object) this.z).GetHashCode();

    public override string ToString() => string.Format("({0}, {1}, {2})", (object) this.x, (object) this.y, (object) this.z);

    public static int3 Parse(string raw)
    {
      Match match = int3.parser.IsMatch(raw) ? int3.parser.Match(raw) : throw new FormatException("Can't to parse \"" + raw + "\" to int3 format. It must have next format: (int,int,int)");
      return new int3(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value), int.Parse(match.Groups["z"].Value));
    }

    public int3 GetClone() => (int3) this.MemberwiseClone();

    public static explicit operator Vector3(int3 coord) => new Vector3((float) coord.x, (float) coord.y, (float) coord.z);
  }
}
