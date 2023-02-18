// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.PrefVariable
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using UnityEditor;

namespace Yurowm.EditorCore
{
  public class PrefVariable
  {
    private string key = "";

    public PrefVariable(string _key) => this.key = _key;

    public int Int
    {
      get => EditorPrefs.GetInt(this.key);
      set => EditorPrefs.SetInt(this.key, value);
    }

    public float Float
    {
      get => EditorPrefs.GetFloat(this.key);
      set => EditorPrefs.SetFloat(this.key, value);
    }

    public string String
    {
      get => EditorPrefs.GetString(this.key);
      set => EditorPrefs.SetString(this.key, value);
    }

    public bool Bool
    {
      get => EditorPrefs.GetBool(this.key);
      set => EditorPrefs.SetBool(this.key, value);
    }

    public bool IsEmpty() => !EditorPrefs.HasKey(this.key);

    public void Delete() => EditorPrefs.DeleteKey(this.key);
  }
}
