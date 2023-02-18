// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.TreeFolder
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;

namespace Yurowm.GameCore
{
  [Serializable]
  public class TreeFolder
  {
    public string path = "";
    public string name = "";

    public string fullPath
    {
      get => this.path.Length > 0 ? this.path + "/" + this.name : this.name;
      set
      {
        int length = value.LastIndexOf('/');
        if (length >= 0)
        {
          this.path = value.Substring(0, length);
          this.name = value.Substring(length + 1, value.Length - length - 1);
        }
        else
        {
          this.path = "";
          this.name = value;
        }
      }
    }

    public override int GetHashCode() => this.fullPath.GetHashCode();
  }
}
