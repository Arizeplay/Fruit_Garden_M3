// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.area
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yurowm.GameCore
{
  [Serializable]
  public class area
  {
    public int2 position;
    public int2 size;

    public int left => this.position.x;

    public int down => this.position.y;

    public int right => this.position.x + this.size.x - 1;

    public int up => this.position.y + this.size.y - 1;

    public int width => this.size.x;

    public int height => this.size.y;

    public area(int2 position, int2 size)
    {
      this.position = position.GetClone();
      this.size = size.GetClone();
    }

    public area(int2 position)
      : this(position, int2.one)
    {
    }

    public area()
      : this(int2.zero)
    {
    }

    public static bool operator ==(area a, area b) => (object) a == null ? (object) b == null : a.Equals((object) b);

    public static bool operator !=(area a, area b) => (object) a == null ? b != null : !a.Equals((object) b);

    public bool IsItInclude(int2 point) => this.left <= point.x && this.right >= point.x && this.down <= point.y && this.up >= point.y;

    public bool IsItInclude(area subarea) => this.left <= subarea.left && this.right >= subarea.right && this.down <= subarea.down && this.up >= subarea.up;

    public bool IsItIntersect(area subarea) => Mathf.Max(this.left, subarea.left) <= Mathf.Min(this.right, subarea.right) && Mathf.Max(this.down, subarea.down) <= Mathf.Min(this.up, subarea.up);

    public override bool Equals(object obj)
    {
      if (obj == null || (object) (obj as area) == null)
        return false;
      area area = (area) obj;
      return this.position == area.position && this.size == area.size;
    }

    public override int GetHashCode() => this.position.GetHashCode() + this.size.GetHashCode();

    public override string ToString() => this == (area) null ? "(Null)" : "position:" + this.position.ToString() + " size:" + this.size.ToString();

    public area GetClone() => (area) this.MemberwiseClone();

    public IEnumerable<int2> GetPoints()
    {
      List<int2> points = new List<int2>();
      for (int left = this.left; left <= this.right; ++left)
      {
        for (int down = this.down; down <= this.up; ++down)
          points.Add(new int2(left, down));
      }
      return (IEnumerable<int2>) points;
    }
  }
}
