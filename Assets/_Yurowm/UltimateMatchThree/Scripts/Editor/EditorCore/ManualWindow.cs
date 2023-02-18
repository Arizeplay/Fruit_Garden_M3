// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ManualWindow
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEditor;
using UnityEngine;

namespace Yurowm.EditorCore
{
  public class ManualWindow : EditorWindow
  {
    public static ManualWindow instance;
    private BerryBrowser browser;

    [MenuItem("Window/Manual")]
    public static ManualWindow CreateManual()
    {
      ManualWindow manual;
      if ((UnityEngine.Object) ManualWindow.instance != (UnityEngine.Object) null && ManualWindow.instance != null)
      {
        manual = ManualWindow.instance;
        manual.Show();
        manual.ShowTab();
      }
      else
      {
        manual = EditorWindow.GetWindow<ManualWindow>();
        manual.Show();
        manual.ShowTab();
        manual.Initialize();
      }
      return manual;
    }

    private void OnEnable() => this.Initialize();

    private void Initialize()
    {
      ManualWindow.instance = this;
      this.titleContent.text = "Manual";
      GUI.FocusControl("");
      this.browser = new BerryBrowser();
      BerryBrowser browser1 = this.browser;
      browser1.onOpenPageRequest = browser1.onOpenPageRequest + (Action<string>) (page => ManualWindow.OpenPage(page));
      BerryBrowser browser2 = this.browser;
      browser2.onRepaintRequest = browser2.onRepaintRequest + new Action(((EditorWindow) this).Repaint);
      this.browser.Initialize();
    }

    private void OnGUI() => this.browser.OnGUI();

    public static void OpenPage(string pagename)
    {
      if ((UnityEngine.Object) ManualWindow.instance == (UnityEngine.Object) null)
      {
        ManualWindow.CreateManual();
        if (!((UnityEngine.Object) ManualWindow.instance != (UnityEngine.Object) null))
          return;
        ManualWindow.instance.browser.startPage = pagename;
      }
      else
        ManualWindow.instance.browser.OpenPage(pagename);
    }
  }
}
