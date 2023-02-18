// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Banner
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  internal abstract class Banner : EditorWindow
  {
    internal UnityEngine.Color bgColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1f);
    private GUIStyle _textAreaStyle;
    private GUIStyle _sBody;
    private GUIStyle _sInput;
    private GUIStyle _sPlaceholder;
    private UnityEngine.Color nonvalidated = new UnityEngine.Color(1f, 0.8f, 0.8f, 1f);

    internal GUIStyle textAreaStyle
    {
      get
      {
        if (this._textAreaStyle == null)
        {
          this._textAreaStyle = new GUIStyle(EditorStyles.textArea);
          this._textAreaStyle.richText = true;
          this._textAreaStyle.fontSize = 14;
          this._textAreaStyle.normal.background = Texture2D.whiteTexture;
          this._textAreaStyle.normal.textColor = UnityEngine.Color.black;
          this._textAreaStyle.hover = this._textAreaStyle.normal;
          this._textAreaStyle.active = this._textAreaStyle.normal;
          this._textAreaStyle.focused = this._textAreaStyle.normal;
        }
        return this._textAreaStyle;
      }
    }

    internal GUIStyle style_Body
    {
      get
      {
        if (this._sBody == null)
        {
          this._sBody = new GUIStyle(EditorStyles.label);
          this._sBody.fontSize = 12;
          this._sBody.alignment = TextAnchor.MiddleCenter;
          this._sBody.fontStyle = FontStyle.Bold;
        }
        return this._sBody;
      }
    }

    internal GUIStyle style_InputField
    {
      get
      {
        if (this._sInput == null)
        {
          this._sInput = new GUIStyle(EditorStyles.numberField);
          this._sInput.fontSize = 17;
        }
        return this._sInput;
      }
    }

    internal GUIStyle style_Placeholder
    {
      get
      {
        if (this._sPlaceholder == null)
        {
          this._sPlaceholder = new GUIStyle(EditorStyles.numberField);
          this._sPlaceholder.normal.background = (Texture2D) null;
          this._sPlaceholder.fontSize = 17;
          var textColor = this._sPlaceholder.normal.textColor;
          textColor.a = 0.2f;
          this._sPlaceholder.normal.textColor = textColor;
          this._sPlaceholder.alignment = TextAnchor.MiddleCenter;
          this._sPlaceholder.fontStyle = FontStyle.Italic;
        }
        return this._sPlaceholder;
      }
    }

    internal static void NewWindow(System.Type bannerType)
    {
      if (!Banner.IsBannerType(bannerType))
        return;
      Banner.CloseAll(bannerType);
      Banner instance = (Banner) ScriptableObject.CreateInstance(bannerType);
      instance.Initialize();
      instance.titleContent = new GUIContent("Unity Locker");
      Rect rect = new Rect();
      rect.size = instance.GetSize();
      ref Rect local = ref rect;
      Resolution currentResolution = Screen.currentResolution;
      double width = (double) currentResolution.width;
      currentResolution = Screen.currentResolution;
      double height = (double) currentResolution.height;
      Vector2 vector2 = new Vector2((float) width, (float) height) / (EditorGUIUtility.pixelsPerPoint * 2f);
      local.center = vector2;
      instance.position = rect;
      instance.hideFlags = HideFlags.DontSave;
      instance.ShowPopup();
      instance.Focus();
    }

    internal static void CloseAll(System.Type bannerType)
    {
      if (!Banner.IsBannerType(bannerType))
        return;
      foreach (EditorWindow editorWindow in Resources.FindObjectsOfTypeAll(bannerType))
        editorWindow.Close();
    }

    private void OnGUI()
    {
      if (!Styles.IsInitialized())
        Styles.Initialize();
      this.OnBannerGUI();
    }

    internal abstract void OnBannerGUI();

    internal abstract Vector2 GetSize();

    internal abstract void Initialize();

    internal static void NewWindowOnLoad(System.Type bannerType)
    {
      if (!Banner.IsBannerType(bannerType))
        return;
      EditorApplication.update = (EditorApplication.CallbackFunction) null;
      Banner.NewWindow(bannerType);
    }

    internal static bool IsBannerType(System.Type type) => typeof (Banner).IsAssignableFrom(type);

    private void Update() => EditorApplication.isPlaying = false;

    internal string InputField(
      string name,
      string value,
      string placeholder = null,
      Func<string, bool> validator = null)
    {
      if (value == null)
        value = "";
      using (validator == null || validator(value) ? (GUIHelper.BackgroundColor) null : new GUIHelper.BackgroundColor(this.nonvalidated))
      {
        Rect position = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(25f)), new GUIContent(name), this.style_Body);
        value = EditorGUI.TextField(position, value, this.style_InputField);
        if (value.IsNullOrEmpty())
        {
          if (!placeholder.IsNullOrEmpty())
            GUI.Label(position, placeholder, this.style_Placeholder);
        }
      }
      return value;
    }

    internal int InputFieldNumber(string name, int value)
    {
      value = EditorGUI.IntField(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(25f)), new GUIContent(name), this.style_Body), value, this.style_InputField);
      return value;
    }

    internal bool EmailValidation(string email) => email != null && new Regex("^([\\w\\.\\-\\+]+)@([\\w\\-]+)((\\.(\\w){2,5})+)$").IsMatch(email);

    internal bool NameValidation(string name) => name != null && name.Length >= 2;

    internal bool InvoiceValidation(string invoice) => invoice != null && invoice.Length >= 5;

    internal int PopupField(string name, int value, params string[] names)
    {
      if (names == null || names.Length == 0)
        return -1;
      value = EditorGUI.Popup(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(25f)), new GUIContent(name), this.style_Body), value, names, this.style_InputField);
      return value;
    }

    internal bool Button(string name)
    {
      bool flag = false;
      GUILayout.Space(10f);
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(100f);
      if (GUILayout.Button(name, GUILayout.ExpandWidth(true), GUILayout.Height(30f)))
        flag = true;
      GUILayout.Space(100f);
      EditorGUILayout.EndHorizontal();
      return flag;
    }
  }
}
