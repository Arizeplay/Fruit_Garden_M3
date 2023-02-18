// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryPanel
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public class BerryPanel : EditorWindow
  {
    private static BerryPanel instance;
    private const string helpLibraryLink = "http://jellymobile.net/jellymobile.net/yurowm/helpLibrary.xml";
    private Dictionary<string, Dictionary<string, string>> helpLibrary = new Dictionary<string, Dictionary<string, string>>();
    private Texture BPLogo;
    private static bool style_IsInitialized;
    private static GUIStyle style_Exeption;
    private static GUIStyle tabButtonStyle;
    public BerryPanelTabAttribute editorAttribute;
    private UnityEngine.Color selectionColor;
    private UnityEngine.Color bgColor;
    private Dictionary<string, List<System.Type>> editors = new Dictionary<string, List<System.Type>>();
    private UnityEngine.Color defalutColor;
    public Vector2 editorScroll;
    public Vector2 tabsScroll;
    private IMetaEditor current_editor;
    private Action editorRender;
    private PrefVariable save_CurrentEditor = new PrefVariable(string.Format("{0}_BerryPanel_CurrentEditor", (object) 27990));
    public static Rect currectEditorRect;

    private void InitializeStyles()
    {
      BerryPanel.style_Exeption = new GUIStyle(GUI.skin.label);
      BerryPanel.style_Exeption.normal.textColor = new UnityEngine.Color(0.5f, 0.0f, 0.0f, 1f);
      BerryPanel.style_Exeption.alignment = TextAnchor.UpperLeft;
      BerryPanel.style_Exeption.wordWrap = true;
      BerryPanel.tabButtonStyle = new GUIStyle(EditorStyles.miniButton);
      BerryPanel.tabButtonStyle.normal.textColor = UnityEngine.Color.white;
      BerryPanel.tabButtonStyle.active.textColor = new UnityEngine.Color(1f, 0.8f, 0.8f, 1f);
      BerryPanel.style_IsInitialized = true;
    }

    [MenuItem("Window/Berry Panel")]
    public static BerryPanel CreateBerryPanel()
    {
      BerryPanel berryPanel;
      if ((UnityEngine.Object) BerryPanel.instance == (UnityEngine.Object) null)
      {
        berryPanel = EditorWindow.GetWindow<BerryPanel>();
        berryPanel.Show();
        berryPanel.OnEnable();
      }
      else
      {
        berryPanel = BerryPanel.instance;
        berryPanel.Show();
      }
      return berryPanel;
    }

    private void OnFocus()
    {
      if (this.current_editor == null)
        return;
      this.current_editor.OnFocus();
    }

    public static void RepaintAll()
    {
      if (!(bool) (UnityEngine.Object) BerryPanel.instance)
        return;
      BerryPanel.instance.Repaint();
    }

    private void OnEnable()
    {
      BerryPanel.instance = this;
      Styles.Initialize();
      this.titleContent.text = "Berry Panel";
      this.LoadEditors();
      this.ShowFirstEditor();
      this.selectionColor = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.white, 0.7f);
      this.bgColor = UnityEngine.Color.Lerp(GUI.backgroundColor, UnityEngine.Color.black, 0.3f);
    }

    private void ShowFirstEditor()
    {
      if (!this.save_CurrentEditor.IsEmpty())
      {
        System.Type type1 = typeof (IMetaEditor);
        System.Type type2 = typeof (MetaEditor<>);
        string editorName = this.save_CurrentEditor.String;
        System.Type type3 = this.editors.Values.SelectMany<List<System.Type>, System.Type>((Func<List<System.Type>, IEnumerable<System.Type>>) (x => (IEnumerable<System.Type>) x)).FirstOrDefault<System.Type>((Func<System.Type, bool>) (x => x.FullName == editorName));
        if (type1.IsAssignableFrom(type3) && type3 != type1 && type3 != type2)
        {
          this.Show((IMetaEditor) Activator.CreateInstance(type3));
          return;
        }
      }
      System.Type type = this.editors.Values.SelectMany<List<System.Type>, System.Type>((Func<List<System.Type>, IEnumerable<System.Type>>) (x => (IEnumerable<System.Type>) x)).FirstOrDefault<System.Type>((Func<System.Type, bool>) (x => ((IEnumerable<object>) x.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (y => y is BerryPanelDefaultAttribute)) != null));
      if (type == null)
        return;
      this.Show((IMetaEditor) Activator.CreateInstance(type));
    }

    private void LoadEditors()
    {
      System.Type type1 = typeof (IMetaEditor);
      System.Type type2 = typeof (MetaEditor<>);
      List<System.Type> typeList = new List<System.Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        try
        {
          foreach (System.Type type3 in assembly.GetTypes())
          {
            if (type1.IsAssignableFrom(type3) && type3 != type1 && type3 != type2)
              typeList.Add(type3);
          }
        }
        catch (ReflectionTypeLoadException ex)
        {
        }
      }
      typeList.RemoveAll((Predicate<System.Type>) (x => ((IEnumerable<object>) x.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (y => y is BerryPanelTabAttribute)) == null));
      typeList.Sort((Comparison<System.Type>) ((a, b) => ((BerryPanelTabAttribute) ((IEnumerable<object>) a.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (x => x is BerryPanelTabAttribute))).Priority.CompareTo(((BerryPanelTabAttribute) ((IEnumerable<object>) b.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (x => x is BerryPanelTabAttribute))).Priority)));
      this.editors.Clear();
      foreach (System.Type type4 in typeList)
      {
        BerryPanelGroupAttribute panelGroupAttribute = (BerryPanelGroupAttribute) ((IEnumerable<object>) type4.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (x => x is BerryPanelGroupAttribute));
        string key = panelGroupAttribute != null ? panelGroupAttribute.Group : "";
        if (!this.editors.ContainsKey(key))
          this.editors.Add(key, new List<System.Type>());
        this.editors[key].Add(type4);
      }
    }

    private IEnumerator DownloadHelpLibraryRoutine()
    {
      Debug.Log((object) "Lagacy help library downloader! It is needed to update!");
      WWW data = new WWW("http://jellymobile.net/jellymobile.net/yurowm/helpLibrary.xml");
      while (!data.isDone)
        yield return (object) 0;
      if (string.IsNullOrEmpty(data.error))
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(data.text);
        this.helpLibrary.Clear();
        foreach (XmlNode childNode in xmlDocument.ChildNodes[0].ChildNodes)
        {
          string key1 = "";
          string key2 = "";
          string str = "";
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
          {
            if (attribute.Name == "title")
              key2 = attribute.Value;
            if (attribute.Name == "link")
              str = attribute.Value;
            if (attribute.Name == "name")
              key1 = attribute.Value;
          }
          if (!(str == "") && !(key2 == "") && !(key1 == ""))
          {
            if (!this.helpLibrary.ContainsKey(key2))
              this.helpLibrary.Add(key2, new Dictionary<string, string>());
            if (!this.helpLibrary[key2].ContainsKey(key1))
              this.helpLibrary[key2].Add(key1, str);
          }
        }
      }
    }

    public IMetaEditor CurrentEditor => this.current_editor;

    private void OnGUI()
    {
      try
      {
        Styles.Update();
        if (!BerryPanel.style_IsInitialized)
          this.InitializeStyles();
        EditorGUILayout.Space();
        if (this.editorRender == null || this.current_editor == null)
        {
          this.editorRender = (Action) null;
          this.current_editor = (IMetaEditor) null;
        }
        this.defalutColor = GUI.backgroundColor;
        using (new GUIHelper.Horizontal(new GUILayoutOption[2]
        {
          GUILayout.ExpandWidth(true),
          GUILayout.ExpandHeight(true)
        }))
        {
          using (new GUIHelper.Vertical(Styles.berryArea, new GUILayoutOption[2]
          {
            GUILayout.Width(150f),
            GUILayout.ExpandHeight(true)
          }))
          {
            if ((UnityEngine.Object) this.BPLogo == (UnityEngine.Object) null)
              this.BPLogo = (Texture) EditorIcons.GetBuildInIcon("BPLogo");
            if ((UnityEngine.Object) this.BPLogo != (UnityEngine.Object) null)
              GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width((float) this.BPLogo.width), GUILayout.Height((float) this.BPLogo.height)), this.BPLogo);
            this.tabsScroll = EditorGUILayout.BeginScrollView(this.tabsScroll);
            this.DrawTabs();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            Rect rect = EditorGUILayout.BeginVertical(Styles.berryArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            this.editorScroll = EditorGUILayout.BeginScrollView(this.editorScroll);
            if (this.current_editor != null && this.editorRender != null)
            {
              if (this.editorAttribute != null)
                this.DrawTitle(this.editorAttribute.Title);
              try
              {
                if (EditorApplication.isCompiling)
                {
                  GUILayout.Label("Compiling...", Styles.centeredMiniLabel, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                }
                else
                {
                  if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
                    BerryPanel.currectEditorRect = rect;
                  this.editorRender();
                }
              }
              catch (Exception ex)
              {
                Debug.LogException(ex);
                GUILayout.Label(ex.ToString(), BerryPanel.style_Exeption);
              }
            }
            else
              GUILayout.Label("Nothing selected", Styles.centeredMiniLabel, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndScrollView();
          }
        }
        //GUILayout.Label(string.Format("{0} v{1}:{2}, Berry Panel v{3}\nYurov Viktor Copyright 2015 - {4}", (object) "Ultimate Match-Three", (object) "1.00", (object) 2, (object) 50, (object) DateTime.Now.Year), Styles.centeredMiniLabel, GUILayout.ExpandWidth(true));
      }
      catch (ArgumentException ex)
      {
        if (ex.Message.StartsWith("Getting control") && ex.Message.StartsWith("GUILayout"))
          return;
        Debug.LogException((Exception) ex);
      }
    }

    private void DrawTabs()
    {
      this.DrawTabs("");
      foreach (KeyValuePair<string, List<System.Type>> editor in this.editors)
      {
        if (!string.IsNullOrEmpty(editor.Key))
          this.DrawTabs(editor.Key);
      }
    }

    private void DrawTabs(string group)
    {
      if (!this.editors.ContainsKey(group))
        return;
      if (!string.IsNullOrEmpty(group))
        this.DrawTabTitle(group);
      foreach (System.Type type in this.editors[group])
      {
        BerryPanelTabAttribute tabAttribute = (BerryPanelTabAttribute) ((IEnumerable<object>) type.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (x => x is BerryPanelTabAttribute));
        if (tabAttribute != null && this.DrawTabButton(tabAttribute))
          this.Show((IMetaEditor) Activator.CreateInstance(type));
      }
    }

    private bool DrawTabButton(BerryPanelTabAttribute tabAttribute)
    {
      bool flag = false;
      if (tabAttribute != null)
      {
        using (new GUIHelper.BackgroundColor(this.editorAttribute == null || !this.editorAttribute.Match((object) tabAttribute) ? UnityEngine.Color.white : this.selectionColor))
        {
          using (new GUIHelper.ContentColor(Styles.centeredMiniLabel.normal.textColor))
            flag = GUILayout.Button(new GUIContent(tabAttribute.Title, (Texture) tabAttribute.Icon), BerryPanel.tabButtonStyle, GUILayout.ExpandWidth(true));
        }
        if (this.editorAttribute != null && this.editorAttribute.Match((object) tabAttribute) && this.editorRender == null)
          flag = true;
      }
      return flag;
    }

    private void DrawTabTitle(string text) => GUILayout.Label(text, Styles.centeredMiniLabel, GUILayout.ExpandWidth(true));

    private void DrawTitle(string text)
    {
      using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
      {
        GUILayout.Label(text, Styles.largeTitle, GUILayout.ExpandWidth(true));
        if (this.helpLibrary.ContainsKey(text))
        {
          foreach (string key in this.helpLibrary[text].Keys)
          {
            GUIContent content = new GUIContent(key);
            if (GUILayout.Button(key, EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(content).x)))
              Application.OpenURL(this.helpLibrary[text][key]);
          }
        }
      }
      GUILayout.Space(10f);
    }

    public static void Scroll(float position)
    {
      if (!((UnityEngine.Object) BerryPanel.instance != (UnityEngine.Object) null))
        return;
      BerryPanel.instance.editorScroll = new Vector2(0.0f, position);
    }

    public void Show(IMetaEditor editor)
    {
      EditorGUI.FocusTextInControl("");
      if (!editor.Initialize())
        return;
      this.current_editor = editor;
      this.save_CurrentEditor.String = editor.GetType().FullName;
      this.editorAttribute = (BerryPanelTabAttribute) ((IEnumerable<object>) editor.GetType().GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (x => x is BerryPanelTabAttribute));
      this.editorRender = new Action(this.current_editor.OnGUI);
    }

    public static IMetaEditor GetCurrentEditor() => (UnityEngine.Object) BerryPanel.instance == (UnityEngine.Object) null ? (IMetaEditor) null : BerryPanel.instance.current_editor;

    public void Show(string editorName)
    {
      System.Type type = this.editors.SelectMany<KeyValuePair<string, List<System.Type>>, System.Type>((Func<KeyValuePair<string, List<System.Type>>, IEnumerable<System.Type>>) (x => (IEnumerable<System.Type>) x.Value)).FirstOrDefault<System.Type>((Func<System.Type, bool>) (x => x.Name == editorName));
      if (type == null)
        return;
      this.Show((IMetaEditor) Activator.CreateInstance(type));
    }
  }
}
