// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.EULA
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  internal class EULA : Banner
  {
    public static string agreed_key = string.Format("{0}_EUAL_agreed", (object) 27990);
    private PrefVariable agreed = new PrefVariable(EULA.agreed_key);
    private string eula;
    private Vector2 scroll;

    internal override Vector2 GetSize() => new Vector2(500f, 700f);

    internal override void Initialize()
    {
      using (StreamReader streamReader = new StreamReader(typeof (EULA).Assembly.GetManifestResourceStream("Yurowm.EditorCore.Resources.EULA.txt")))
        this.eula = streamReader.ReadToEnd();
      this.eula = this.eula.Replace("*PRODUCT_NAME*", "Ultimate Match-Three");
      if (new Regex("YV\\d{9}-\\d{6}").IsMatch(ProjectInfo.accessKey.ToString()))
        return;
      this.agreed.Bool = true;
    }

    internal override void OnBannerGUI()
    {
      if (this.agreed.Bool)
        this.Close();
      GUILayout.Space(10f);
      GUILayout.Label("END-USER LICENSE AGREEMENT", Styles.largeTitle, GUILayout.ExpandWidth(true));
      this.scroll = EditorGUILayout.BeginScrollView(this.scroll, GUILayout.Width(500f), GUILayout.ExpandHeight(true));
      EditorGUILayout.TextArea(this.eula, this.textAreaStyle, GUILayout.Width(470f), GUILayout.ExpandHeight(true));
      EditorGUILayout.EndScrollView();
      if (this.Button("Agree"))
      {
        this.agreed.Bool = true;
        this.Close();
      }
      if (this.Button("Close Unity"))
      {
        this.Close();
        EditorApplication.Exit(0);
      }
      GUILayout.Space(20f);
    }
  }
}
