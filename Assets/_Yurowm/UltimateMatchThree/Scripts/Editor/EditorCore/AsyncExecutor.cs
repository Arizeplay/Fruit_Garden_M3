// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.AsyncExecutor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Threading;

namespace Yurowm.EditorCore
{
  internal class AsyncExecutor
  {
    private Action action;
    private Action onSuccess;
    private Action<Exception> onFailed;
    private DateTime? expiryTime;
    private bool complete;

    public AsyncExecutor(Action action, Action onSuccess = null, Action<Exception> onFailed = null, float delay = 0.0f)
    {
      this.action = action;
      this.onSuccess = onSuccess;
      this.onFailed = onFailed;
      if ((double) delay > 0.0)
        this.expiryTime = new DateTime?(DateTime.Now.AddSeconds((double) delay));
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.Start));
    }

    private void Start(object state)
    {
      if (this.action != null)
      {
        this.complete = false;
        try
        {
          this.action();
        }
        catch (Exception ex)
        {
          if (this.onFailed != null)
            this.onFailed(ex);
        }
        if (this.onSuccess != null)
          this.onSuccess();
      }
      this.complete = true;
      if (this.action != null)
        return;
      this.onFailed((Exception) new NullReferenceException("action"));
    }

    public bool IsComplete()
    {
      if (!this.complete && this.expiryTime.HasValue && this.expiryTime.Value <= DateTime.Now)
        this.complete = true;
      return this.complete;
    }
  }
}
