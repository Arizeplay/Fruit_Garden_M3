// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.EditorCoroutine
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.Collections;
using UnityEditor;

namespace Yurowm.EditorCore
{
  public class EditorCoroutine
  {
    private readonly IEnumerator routine;
    private bool isPlaying;

    public static EditorCoroutine start(IEnumerator _routine)
    {
      EditorCoroutine editorCoroutine = new EditorCoroutine(_routine);
      editorCoroutine.start();
      return editorCoroutine;
    }

    private EditorCoroutine(IEnumerator _routine) => this.routine = _routine;

    public void start()
    {
      EditorApplication.update += new EditorApplication.CallbackFunction(this.update);
      this.isPlaying = true;
    }

    public void stop()
    {
      EditorApplication.update -= new EditorApplication.CallbackFunction(this.update);
      this.isPlaying = false;
    }

    public bool IsPlaying() => this.isPlaying;

    private void update()
    {
      if (this.routine.MoveNext())
        return;
      this.stop();
    }
  }
}
