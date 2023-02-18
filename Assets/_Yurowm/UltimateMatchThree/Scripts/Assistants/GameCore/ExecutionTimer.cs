// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.ExecutionTimer
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yurowm.GameCore
{
  public class ExecutionTimer : IDisposable
  {
    private DateTime start;
    private string title;
    private List<ExecutionTimer.Checkpoint> checkpoints = new List<ExecutionTimer.Checkpoint>();

    public ExecutionTimer(string title = "Execution")
    {
      this.start = DateTime.Now;
      this.title = title;
    }

    public void Flash(string name = null) => this.checkpoints.Add(new ExecutionTimer.Checkpoint()
    {
      time = (DateTime.Now - this.start).TotalMilliseconds,
      name = name
    });

    public void Dispose()
    {
      string str = (string) null;
      string message;
      if (this.checkpoints.Count > 0)
      {
        message = str + this.title + ":\n" + "Total: " + (object) (DateTime.Now - this.start).TotalMilliseconds + " ms.";
        double num = 0.0;
        for (int index = 0; index < this.checkpoints.Count; ++index)
        {
          message = message + "\n" + (this.checkpoints[index].name == null ? (object) ("Checkpoint" + (object) index) : (object) this.checkpoints[index].name) + ": " + (object) (this.checkpoints[index].time - num) + " ms.";
          num = this.checkpoints[index].time;
        }
      }
      else
        message = str + this.title + ": " + (object) (DateTime.Now - this.start).TotalMilliseconds + " ms.";
      Debug.Log((object) message);
    }

    private struct Checkpoint
    {
      public double time;
      public string name;
    }
  }
}
