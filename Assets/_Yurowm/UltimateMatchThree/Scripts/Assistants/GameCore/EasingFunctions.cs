// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.EasingFunctions
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using UnityEngine;

namespace Yurowm.GameCore
{
  public class EasingFunctions
  {
    public static float linear(float t) => t;

    public static float easeInQuad(float t) => t * t;

    public static float easeOutQuad(float t) => t * (2f - t);

    public static float easeInOutQuad(float t) => (double) t >= 0.5 ? (float) ((4.0 - 2.0 * (double) t) * (double) t - 1.0) : 2f * t * t;

    public static float easeInCubic(float t) => t * t * t;

    public static float easeOutCubic(float t) => (float) ((double) --t * (double) t * (double) t + 1.0);

    public static float easeInOutCubic(float t) => (double) t >= 0.5 ? (float) (((double) t - 1.0) * (2.0 * (double) t - 2.0) * (2.0 * (double) t - 2.0) + 1.0) : 4f * t * t * t;

    public static float easeInQuart(float t) => t * t * t * t;

    public static float easeOutQuart(float t) => (float) (1.0 - (double) --t * (double) t * (double) t * (double) t);

    public static float easeInOutQuart(float t) => (double) t >= 0.5 ? (float) (1.0 - 8.0 * (double) --t * (double) t * (double) t * (double) t) : 8f * t * t * t * t;

    public static float easeInQuint(float t) => t * t * t * t * t;

    public static float easeOutQuint(float t) => (float) (1.0 + (double) --t * (double) t * (double) t * (double) t * (double) t);

    public static float easeInOutQuint(float t) => (double) t >= 0.5 ? (float) (1.0 + 16.0 * (double) --t * (double) t * (double) t * (double) t * (double) t) : 16f * t * t * t * t * t;

    public static float easeInElastic(float t)
    {
      if ((double) t == 0.0 || (double) t == 1.0)
        return t;
      float num = 0.5f;
      return (float) -((double) Mathf.Pow(2f, -10f * t) * (double) Mathf.Sin((float) (-((double) t + (double) num / 4.0) * 6.28318548202515) / num));
    }

    public static float easeOutElastic(float t)
    {
      if ((double) t == 0.0 || (double) t == 1.0)
        return t;
      float num = 0.5f;
      return (float) ((double) Mathf.Pow(2f, -10f * t) * (double) Mathf.Sin((float) (((double) t - (double) num / 4.0) * 6.28318548202515) / num) + 1.0);
    }

    public static float easeInOutElastic(float t)
    {
      if ((double) t <= 0.0 || (double) t >= 1.0)
        return Mathf.Clamp01(t);
      t = Mathf.Lerp(-1f, 1f, t);
      float num = 0.9f;
      return (double) t < 0.0 ? (float) (0.5 * ((double) Mathf.Pow(2f, 10f * t) * (double) Mathf.Sin((float) (((double) t + (double) num / 4.0) * 6.28318548202515) / num))) : (float) ((double) Mathf.Pow(2f, -10f * t) * (double) Mathf.Sin((float) (((double) t - (double) num / 4.0) * 6.28318548202515) / num) * 0.5 + 1.0);
    }
  }
}
