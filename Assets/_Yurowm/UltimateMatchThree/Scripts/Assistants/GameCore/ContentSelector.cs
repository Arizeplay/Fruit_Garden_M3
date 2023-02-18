// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.ContentSelector
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using UnityEngine;

namespace Yurowm.GameCore
{
  public class ContentSelector : PropertyAttribute
  {
    public System.Type targetType;

    public ContentSelector()
    {
    }

    public ContentSelector(System.Type type) => this.targetType = type;
  }
}
