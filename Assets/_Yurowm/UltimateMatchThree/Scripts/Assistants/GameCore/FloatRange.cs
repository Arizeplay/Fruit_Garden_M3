// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.FloatRange
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.GameCore
{
  [Serializable]
  public class FloatRange
  {
    public float min;
    public float max;
    private static Regex parser = new Regex("\\((?<min>\\d*\\.?\\d+)\\,(?<max>\\d*\\.?\\d+)\\)");
    private static string format = "({0},{1})";

    public float interval => Mathf.Abs(this.max - this.min);

    public FloatRange(float min, float max)
    {
      this.min = min;
      this.max = max;
    }

    public bool IsInRange(float value) => (double) value >= (double) Mathf.Min(this.min, this.max) && (double) value <= (double) Mathf.Max(this.min, this.max);

    public float Lerp(float t) => Mathf.Lerp(this.min, this.max, t);

    internal FloatRange GetClone() => (FloatRange) this.MemberwiseClone();

    public static FloatRange Parse(string raw)
    {
      Match match = FloatRange.parser.Match(raw);
      if (match.Success)
        return new FloatRange(float.Parse(match.Groups["min"].Value), float.Parse(match.Groups["max"].Value));
      throw new FormatException("Can't to parse \"" + raw + "\" to FloatRange format. It must have next format: (float,float)");
    }

    public override string ToString() => string.Format(FloatRange.format, (object) this.min, (object) this.max);

    public static implicit operator FloatRange(float value) => new FloatRange(value, value);
  }
}
