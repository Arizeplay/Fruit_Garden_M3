// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.MetaEditor`1
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using UnityEngine;

namespace Yurowm.EditorCore
{
  public abstract class MetaEditor<T> : IMetaEditor where T : MonoBehaviour
  {
    private T target;

    public T metaTarget
    {
      get
      {
        if ((Object) this.target == (Object) null)
          this.target = this.FindTarget();
        return this.target;
      }
    }

    public abstract T FindTarget();

    public abstract void OnGUI();

    public abstract bool Initialize();

    public static void Repaint() => BerryPanel.RepaintAll();

    public virtual void OnFocus()
    {
    }
  }
}
