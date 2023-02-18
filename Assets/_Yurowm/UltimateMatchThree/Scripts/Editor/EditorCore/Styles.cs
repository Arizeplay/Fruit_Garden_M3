// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Styles
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public static class Styles
  {
    private static bool initialized;
    public static GUIStyle richLabel;
    public static GUIStyle whiteRichLabel;
    public static GUIStyle whiteLabel;
    public static GUIStyle whiteBoldLabel;
    public static GUIStyle multilineLabel;
    public static GUIStyle centeredLabel;
    public static GUIStyle miniLabel;
    public static GUIStyle centeredMiniLabel;
    public static GUIStyle centeredMiniLabelWhite;
    public static GUIStyle centeredMiniLabelBlack;
    public static GUIStyle largeTitle;
    public static GUIStyle title;
    public static GUIStyle berryArea;
    public static GUIStyle area;
    public static GUIStyle levelArea;
    public static GUIStyle textAreaLineBreaked;
    public static GUIStyle separator;
    public static GUIStyle monospaceLabel;
    public static string highlightStrongBlue;
    public static string highlightBlue;
    public static string highlightStrongRed;
    public static string highlightRed;
    public static string highlightStrongGreen;
    public static string highlightGreen;

    [InitializeOnLoadMethod]
    internal static void OnLoad() => EditorApplication.update += new EditorApplication.CallbackFunction(Styles.Update);

    public static bool IsInitialized() => Styles.initialized;

    internal static void Initialize()
    {
      try
      {
        Styles.richLabel = new GUIStyle(EditorStyles.label);
        Styles.richLabel.richText = true;
        Styles.whiteRichLabel = new GUIStyle(Styles.richLabel);
        Styles.whiteRichLabel.normal.textColor = UnityEngine.Color.white;
        Styles.whiteLabel = new GUIStyle(EditorStyles.whiteLabel);
        Styles.whiteLabel.normal.textColor = UnityEngine.Color.white;
        Styles.whiteBoldLabel = new GUIStyle(EditorStyles.whiteBoldLabel);
        Styles.whiteBoldLabel.normal.textColor = UnityEngine.Color.white;
        Styles.multilineLabel = new GUIStyle(EditorStyles.label);
        Styles.multilineLabel.clipping = TextClipping.Clip;
        Styles.centeredLabel = new GUIStyle(EditorStyles.label);
        Styles.centeredLabel.alignment = TextAnchor.MiddleCenter;
        Styles.centeredMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        Styles.centeredMiniLabel.normal.textColor = EditorStyles.label.normal.textColor;
        Styles.miniLabel = new GUIStyle(Styles.centeredMiniLabel);
        Styles.miniLabel.alignment = Styles.richLabel.alignment;
        Styles.centeredMiniLabelWhite = new GUIStyle(Styles.centeredMiniLabel);
        Styles.centeredMiniLabelWhite.normal.textColor = UnityEngine.Color.white;
        Styles.centeredMiniLabelBlack = new GUIStyle(Styles.centeredMiniLabel);
        Styles.centeredMiniLabelBlack.normal.textColor = UnityEngine.Color.black;
        Styles.textAreaLineBreaked = new GUIStyle(EditorStyles.textArea);
        Styles.textAreaLineBreaked.clipping = TextClipping.Clip;
        Styles.monospaceLabel = new GUIStyle(Styles.textAreaLineBreaked);
        Styles.monospaceLabel.font = Resources.Load<Font>("Fonts/CourierNew");
        Styles.monospaceLabel.fontSize = 11;
        Styles.largeTitle = new GUIStyle(EditorStyles.label);
        Styles.largeTitle.normal.textColor = EditorStyles.label.normal.textColor;
        Styles.largeTitle.fontStyle = FontStyle.Bold;
        Styles.largeTitle.fontSize = 21;
        Styles.title = new GUIStyle(EditorStyles.label);
        Styles.title.normal.textColor = EditorStyles.label.normal.textColor;
        Styles.title.fontStyle = FontStyle.Bold;
        Styles.title.fontSize = 18;
        Styles.berryArea = new GUIStyle(EditorStyles.textArea);
        Styles.berryArea.normal.background = EditorIcons.GetBuildInIcon("BerryArea");
        Styles.berryArea.border = new RectOffset(4, 4, 5, 3);
        Styles.berryArea.margin = new RectOffset(2, 2, 2, 2);
        Styles.berryArea.padding = new RectOffset(2, 2, 2, 2);
        Styles.area = new GUIStyle(EditorStyles.textArea);
        Styles.area.normal.background = EditorIcons.GetBuildInIcon("Area");
        Styles.area.border = new RectOffset(4, 4, 5, 3);
        Styles.area.margin = new RectOffset(2, 2, 2, 2);
        Styles.area.padding = new RectOffset(4, 4, 4, 4);
        Styles.levelArea = new GUIStyle(Styles.area);
        Styles.levelArea.normal.background = EditorIcons.GetBuildInIcon("LevelArea");
        Styles.levelArea.border = new RectOffset(4, 4, 5, 3);
        Styles.levelArea.margin = new RectOffset(2, 2, 2, 2);
        Styles.levelArea.padding = new RectOffset(4, 4, 4, 4);
        Styles.separator = new GUIStyle(Styles.area);
        Styles.separator.normal.background = EditorIcons.GetBuildInIcon("Separator");
        Styles.separator.border = new RectOffset(2, 2, 2, 2);
        Styles.separator.margin = new RectOffset(0, 0, 0, 0);
        Styles.separator.padding = new RectOffset(0, 0, 0, 0);
        Styles.highlightStrongBlue = "<color=#" + (EditorGUIUtility.isProSkin ? "8888ff" : "4444ff") + "ff>{0}</color>";
        Styles.highlightBlue = "<color=#" + (EditorGUIUtility.isProSkin ? "5555bb" : "222266") + "ff>{0}</color>";
        Styles.highlightStrongRed = "<color=#" + (EditorGUIUtility.isProSkin ? "ff8888" : "ff4444") + "ff>{0}</color>";
        Styles.highlightRed = "<color=#" + (EditorGUIUtility.isProSkin ? "bb5555" : "662222") + "ff>{0}</color>";
        Styles.highlightStrongGreen = "<color=#" + (EditorGUIUtility.isProSkin ? "88ff88" : "44ff44") + "ff>{0}</color>";
        Styles.highlightGreen = "<color=#" + (EditorGUIUtility.isProSkin ? "55bb55" : "226622") + "ff>{0}</color>";
      }
      catch (Exception ex)
      {
        return;
      }
      Styles.initialized = true;
    }

    internal static void Update()
    {
      if (Styles.initialized && !((UnityEngine.Object) Styles.area.normal.background == (UnityEngine.Object) null))
        return;
      Styles.Initialize();
    }
  }
}
