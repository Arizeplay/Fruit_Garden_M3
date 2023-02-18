// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryPanelGroupAttribute
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;

namespace Yurowm.EditorCore
{
  public class BerryPanelGroupAttribute : Attribute
  {
    private string group;

    public BerryPanelGroupAttribute(string group) => this.group = group;

    public string Group => this.group;
  }
}
