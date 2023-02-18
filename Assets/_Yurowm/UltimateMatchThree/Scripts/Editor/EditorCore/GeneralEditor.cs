// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.GeneralEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelTab("Product Info", 1000)]
  public class GeneralEditor : MetaEditor
  {
    public override bool Initialize()
    {
      if ((bool) (Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main && !EditorApplication.isPlaying)
      {
        SerializedObject serializedObject = new SerializedObject((Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main);
        serializedObject.FindProperty("key").stringValue = ProjectInfo.accessKey;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
      }
      return true;
    }

    public override void OnGUI()
    {
      EditorGUILayout.LabelField("Product", "{0} v{1}:{2}".FormatText((object) "Ultimate Match-Three", (object) "1.00", (object) 2));
      if (ProjectInfo.accessKey.IsNullOrEmpty())
      {
        EditorGUILayout.HelpBox("Please provide information about you. It will help to make the product better.", MessageType.Info);
        if (!GUILayout.Button("Provide", GUILayout.Width(100f)))
          return;
        Banner.NewWindow(typeof (Welcome));
      }
      else
      {
        string str = ProjectInfo.buyer;
        if (!ProjectInfo.company.IsNullOrEmpty())
          str = str + " from " + ProjectInfo.company;
        if (str.IsNullOrEmpty())
          EditorGUILayout.LabelField("User", "Unknown");
        else
          EditorGUILayout.LabelField("User", str);
        EditorGUILayout.LabelField("Invoice", ProjectInfo.accessKey);
      }
    }
  }
}
