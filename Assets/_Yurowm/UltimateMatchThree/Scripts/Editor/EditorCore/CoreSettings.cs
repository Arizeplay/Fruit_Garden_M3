// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.CoreSettings
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("Core Settings", -1000)]
  public class CoreSettings : MetaEditor
  {
    private static PrefVariable coreUpdate = new PrefVariable("{0}_coreUpdate".FormatText((object) 27990));
    private static bool _coreUpdate = true;

    internal static bool IsCoreUpdateEnable
    {
      get
      {
        try
        {
          CoreSettings._coreUpdate = CoreSettings.coreUpdate.IsEmpty() || CoreSettings.coreUpdate.Bool;
        }
        catch (Exception ex)
        {
        }
        return CoreSettings._coreUpdate;
      }
    }

    public override bool Initialize()
    {
      if (CoreSettings.coreUpdate.IsEmpty())
        CoreSettings.coreUpdate.Bool = true;
      return true;
    }

    public override void OnGUI()
    {
      GUILayout.Label("The Core is three DLL libraries in the project. It contains such useful features as:\r\n* BerryPanel - the main dashboard of this asset\r\n* Fixer - the tool that allows to apply all last bug fixes and small improvements\r\n* BerryBrowser - the tool that gives you an access to the projects manual\r\n* Content Manager - special component, which makes work with in-game content clear and easy\r\n* Support Form - easy way to contact me (the asset developer) and to get support\r\n* Utils - a set of different kind base types as int2 (two-dimential integer), Side (enum of directions: Left, Top, Right, etc), easing functions, URandom (special kind of random number generator), different type extensions, etc.\r\nThis is a part of background of your project. The Core is updated automatically making features described above is fresh, even if you are using very old version of the product. In the same time, such tool as the Fixer allows you to fix all known bugs and to implement all new small imporvements. As for big improvements, such as new in-game features like new goals, new chips and blockers, they also can be implemented, but using another tool - DLC, which is a part of the Core too.\r\nThe Core makes the asset is updated to the last version every day, so you don't need to worry about it. It's as if I work with you side by side on this project. This is exactly why it was made. But you can have a reason to don't wish that. In this case you can disable automatic updates.", EditorStyles.wordWrappedLabel);
      CoreSettings.coreUpdate.Bool = !EditorGUILayout.ToggleLeft("I don't want to update the Core", !CoreSettings.coreUpdate.Bool);
      if (CoreSettings.coreUpdate.Bool)
        return;
      EditorGUILayout.HelpBox("The Core updates is disabled. Fixer and DLC features isn't work anymore.", MessageType.Info);
    }
  }
}
