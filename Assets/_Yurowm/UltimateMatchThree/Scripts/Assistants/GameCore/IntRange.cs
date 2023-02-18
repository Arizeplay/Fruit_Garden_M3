// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.IntRange
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.GameCore
{
  [Serializable]
  public class IntRange : IEnumerable<int>, IEnumerable
  {
    public int min;
    public int max;
    private static Regex parser = new Regex("\\((?<min>\\d+)\\,(?<max>\\d+)\\)");
    private static string format = "({0},{1})";

    public int interval => Mathf.Abs(this.max - this.min);

    public int Max => Mathf.Max(this.min, this.max);

    public int Min => Mathf.Min(this.min, this.max);

    public int count => Mathf.Abs(this.max - this.min) + 1;

    public IntRange(int min, int max)
    {
      this.min = min;
      this.max = max;
    }

    public bool IsInRange(int value) => value >= this.Min && value <= this.Max;

    public int Lerp(float t) => Mathf.RoundToInt(Mathf.Lerp((float) this.min, (float) this.max, t));

    internal IntRange GetClone() => (IntRange) this.MemberwiseClone();

    IEnumerator<int> IEnumerable<int>.GetEnumerator()
    {
      for (int value = this.Min; value <= this.Max; ++value)
        yield return value;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) ((IEnumerable<int>) this).GetEnumerator();

    public static IntRange Parse(string raw)
    {
      Match match = IntRange.parser.Match(raw);
      if (match.Success)
        return new IntRange(int.Parse(match.Groups["min"].Value), int.Parse(match.Groups["max"].Value));
      throw new FormatException("Can't to parse \"" + raw + "\" to IntRange format. It must have next format: (int,int)");
    }

    public override string ToString() => string.Format(IntRange.format, (object) this.min, (object) this.max);

    public static implicit operator IntRange(int number) => new IntRange(number, number);
  }
}
