// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.MetaWindow`1
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEditor;

namespace Yurowm.EditorCore
{
  public class MetaWindow<W> : EditorWindow where W : IMetaEditor
  {
    internal W editor;

    private void OnEnable()
    {
      System.Type type = typeof (W);
      if (type.IsAbstract || type.IsInterface)
        return;
      this.editor = Activator.CreateInstance<W>();
      if (this.editor.Initialize())
        return;
      this.editor = default (W);
    }

    private void OnGUI()
    {
      if (this.editor.Equals((object) default (W)))
        return;
      this.editor.OnGUI();
    }

    public static MW Open<MW>() where MW : MetaWindow<W>
    {
      MW window = EditorWindow.GetWindow<MW>();
      window.Show();
      window.OnEnable();
      return window;
    }
  }
}
