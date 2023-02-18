// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryPanelTabAttribute
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public class BerryPanelTabAttribute : Attribute
  {
    private float priority;
    private string title;
    private Texture2D icon;

    public BerryPanelTabAttribute(string title, int priority = 0)
    {
      this.title = title;
      this.priority = (float) priority;
    }

    public BerryPanelTabAttribute(string title, string icon, int priority = 0)
    {
      this.title = title;
      this.priority = (float) priority;
      if (string.IsNullOrEmpty(icon))
        return;
      this.icon = EditorIcons.GetBuildInIcon(icon);
      if ((bool) (UnityEngine.Object) this.icon)
        return;
      this.icon = EditorIcons.GetIcon(icon);
    }

    public string Title => this.title;

    public float Priority => this.priority;

    public Texture2D Icon => this.icon;
  }
}
