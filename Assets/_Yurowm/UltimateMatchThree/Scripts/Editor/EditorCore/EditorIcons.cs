// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.EditorIcons
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public static class EditorIcons
  {
    private static Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();
    private static Dictionary<string, Texture2D> bi_icons = new Dictionary<string, Texture2D>();

    public static Texture2D GetIcon(string name)
    {
      if (!EditorIcons.icons.ContainsKey(name))
        EditorIcons.icons.Add(name, (Texture2D) null);
      if ((Object) EditorIcons.icons[name] == (Object) null)
        EditorIcons.icons[name] = EditorIcons.FindIcon(name);
      return EditorIcons.icons[name];
    }

    public static Texture2D GetBuildInIcon(string name)
    {
      if (!EditorIcons.bi_icons.ContainsKey(name))
        EditorIcons.bi_icons.Add(name, (Texture2D) null);
      if ((Object) EditorIcons.bi_icons[name] == (Object) null)
        EditorIcons.bi_icons[name] = EditorIcons.FindIcon(name, true);
      return EditorIcons.bi_icons[name];
    }

    private static Texture2D FindIcon(string name, bool buildin = false)
    {
      if (!buildin)
        return EditorGUIUtility.Load(string.Format("LevelEditor/{0}.png", (object) name)) as Texture2D;
      var texture2D = Resources.Load<Texture2D>(name);
      if (texture2D == null)
        return (Texture2D) null;

      return texture2D;
    }
  }
}
