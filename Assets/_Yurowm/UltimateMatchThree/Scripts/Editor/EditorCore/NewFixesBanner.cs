// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.NewFixesBanner
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  internal class NewFixesBanner : Banner
  {
    private Vector2 scroll;
    private string annotation = "";
    private const string annotationFormat = "<b>ISSUE #{0}</b>\n   {1}";

    internal override Vector2 GetSize() => new Vector2(400f, 600f);

    internal override void Initialize()
    {
      List<Fixer.Issue> source = Fixer.state.LoadIssues();
      source.Sort((Comparison<Fixer.Issue>) ((x, y) => x.level.CompareTo(y.level)));
      this.annotation = string.Join("\n\n", source.Select<Fixer.Issue, string>((Func<Fixer.Issue, string>) (x => string.Format("<b>ISSUE #{0}</b>\n   {1}", (object) x.level, (object) x.annotation))).ToArray<string>());
    }

    internal override void OnBannerGUI()
    {
      GUILayout.Space(10f);
      GUILayout.Label("FIXER", Styles.largeTitle, GUILayout.ExpandWidth(true));
      GUILayout.Space(10f);
      GUILayout.Label("NEW ISSUES ARE FOUND", this.style_Body, GUILayout.ExpandWidth(true));
      this.scroll = EditorGUILayout.BeginScrollView(this.scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      EditorGUILayout.TextArea(this.annotation, this.textAreaStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      EditorGUILayout.EndScrollView();
      EditorGUILayout.HelpBox("If you will close this popup, you will be able to fix these issues manually in BerryPanel.Fixer.", MessageType.Info);
      if (this.Button("FIX THEM AUTOMATICALLY"))
      {
        Fixer.Apply();
        this.Close();
      }
      if (this.Button("CLOSE"))
        this.Close();
      GUILayout.Space(10f);
    }
  }
}
