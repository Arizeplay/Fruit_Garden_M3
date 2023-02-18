// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.SortingLayerAndOrder
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;

namespace Yurowm.GameCore
{
  [Serializable]
  public class SortingLayerAndOrder
  {
    public int layerID;
    public int order;

    public SortingLayerAndOrder(int layerID, int order)
    {
      this.layerID = layerID;
      this.order = order;
    }
  }
}
