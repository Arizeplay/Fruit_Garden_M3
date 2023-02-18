// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.LogicRequestInstruction
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public class LogicRequestInstruction : CustomYieldInstruction
  {
    private AsyncExecutor executor;
    private LogicRequest request;
    public string result;
    public string error;

    public LogicRequestInstruction(string method, string parameters, string password, float delay = 0.0f)
    {
      if (password != null && (method.Length == 0 || method.Length > 64 || password.Length != 47))
      {
        this.error = "Incorect password!";
      }
      else
      {
        this.request = new LogicRequest();
        this.request.Initialize(method, parameters, password);
        this.executor = new AsyncExecutor(new Action(this.Logic), onFailed: ((Action<Exception>) (e => Debug.LogException(e))), delay: delay);
      }
    }

    private void Logic()
    {
      this.request.Execute();
      LogicRequest.Result result = (LogicRequest.Result) this.request.GetResult();
      this.result = result.result;
      this.error = result.error;
    }

    public override bool keepWaiting => !this.executor.IsComplete();
  }
}
