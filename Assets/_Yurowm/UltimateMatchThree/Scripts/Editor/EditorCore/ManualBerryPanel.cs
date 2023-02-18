// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ManualBerryPanel
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEngine;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("Manual", "ManualTabIcon", 0)]
  public class ManualBerryPanel : MetaEditor
  {
    private BerryBrowser browser;

    public override bool Initialize()
    {
      GUI.FocusControl("");
      this.browser = new BerryBrowser();
      BerryBrowser browser1 = this.browser;
      browser1.onOpenPageRequest = browser1.onOpenPageRequest + (Action<string>) (page => this.browser.OpenPage(page));
      BerryBrowser browser2 = this.browser;
      browser2.onRepaintRequest = browser2.onRepaintRequest + new Action(MetaEditor<MonoBehaviour>.Repaint);
      this.browser.Initialize();
      return true;
    }

    public override void OnGUI()
    {
      GUILayout.Label("The manual can be opened in a separate window (Window / Manual)", Styles.centeredLabel);
      this.browser.OnGUI();
    }
  }
}
