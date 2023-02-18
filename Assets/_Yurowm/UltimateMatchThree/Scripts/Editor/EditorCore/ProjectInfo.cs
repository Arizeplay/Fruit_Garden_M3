// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ProjectInfo
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEditor;

namespace Yurowm.EditorCore
{
  public static class ProjectInfo
  {
    public const string fullName = "Ultimate Match-Three";
    public const string name = "ultimate";
    public const int id = 27990;

    public static int coreVersion => 50;

    public static int fixerLevel => Fixer.state.level;

    public static bool isDebug => false;

    public static string buyer
    {
      get => EditorPrefs.GetString(string.Format("{0}_buyer", (object) 27990));
      set => EditorPrefs.SetString(string.Format("{0}_buyer", (object) 27990), value);
    }

    public static string email
    {
      get => EditorPrefs.GetString(string.Format("{0}_email", (object) 27990));
      set => EditorPrefs.SetString(string.Format("{0}_email", (object) 27990), value);
    }

    public static string company
    {
      get => EditorPrefs.GetString(string.Format("{0}_company", (object) 27990));
      set => EditorPrefs.SetString(string.Format("{0}_company", (object) 27990), value);
    }

    public static int teamSize
    {
      get => EditorPrefs.GetInt(string.Format("{0}_teamSize", (object) 27990));
      set => EditorPrefs.SetInt(string.Format("{0}_teamSize", (object) 27990), value);
    }

    public static string accessKey
    {
      get => EditorPrefs.GetString(string.Format("{0}_invoice", (object) 27990));
      set => EditorPrefs.SetString(string.Format("{0}_invoice", (object) 27990), value);
    }

    internal static DateTime last_check
    {
      get => EditorPrefs.HasKey(string.Format("{0}_last_check", (object) 27990)) ? new DateTime(long.Parse(EditorPrefs.GetString(string.Format("{0}_last_check", (object) 27990)))) : new DateTime();
      set => EditorPrefs.SetString(string.Format("{0}_last_check", (object) 27990), value.Ticks.ToString());
    }
  }
}
