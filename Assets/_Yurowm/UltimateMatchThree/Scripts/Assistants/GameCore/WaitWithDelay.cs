// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.WaitWithDelay
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using UnityEngine;

namespace Yurowm.GameCore
{
  public class WaitWithDelay : CustomYieldInstruction
  {
    private float? lastTrue;
    private float delay;
    private Func<bool> predicate;

    public WaitWithDelay(Func<bool> predicate, float delay)
    {
      this.lastTrue = new float?();
      this.delay = delay;
      this.predicate = predicate;
    }

    public override bool keepWaiting
    {
      get
      {
        if (this.predicate())
        {
          if (!this.lastTrue.HasValue)
            this.lastTrue = new float?(Time.time);
          if ((double) this.lastTrue.Value + (double) this.delay < (double) Time.time)
            return false;
        }
        else
          this.lastTrue = new float?();
        return true;
      }
    }
  }
}
