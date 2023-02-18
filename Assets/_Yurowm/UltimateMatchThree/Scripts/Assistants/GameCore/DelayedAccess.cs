// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.DelayedAccess
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using UnityEngine;

namespace Yurowm.GameCore
{
  public class DelayedAccess
  {
    private DateTime nextTime;
    private TimeSpan timeSpan;

    public DelayedAccess(float delay, bool fromThisMoment = false)
    {
      this.timeSpan = new TimeSpan(0, 0, 0, 0, Mathf.RoundToInt(delay * 1000f));
      if (!fromThisMoment)
        return;
      this.ResetTimer();
    }

    public void Break() => this.nextTime = new DateTime();

    public void ResetTimer() => this.nextTime = DateTime.Now + this.timeSpan;

    public bool GetAccess()
    {
      if (DateTime.Now < this.nextTime)
        return false;
      this.ResetTimer();
      return true;
    }
  }
}
