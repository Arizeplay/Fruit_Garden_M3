// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.IMetaEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

namespace Yurowm.EditorCore
{
  public interface IMetaEditor
  {
    void OnGUI();

    bool Initialize();

    void OnFocus();
  }
}
