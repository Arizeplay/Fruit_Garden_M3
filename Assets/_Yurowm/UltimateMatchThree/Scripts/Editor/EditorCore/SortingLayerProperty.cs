// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.SortingLayerProperty
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [CustomPropertyDrawer(typeof (SortingLayerAndOrder))]
  public class SortingLayerProperty : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (property.hasMultipleDifferentValues)
        return;
      EditorGUI.BeginProperty(position, label, property);
      int indentLevel = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;
      Rect rect1 = new Rect(position);
      Rect rect2 = EditorGUI.PrefixLabel(rect1, label);
      rect1.xMin = rect2.x;
      rect1.width /= 2f;
      string[] sortingLayerNames = SortingLayerProperty.GetSortingLayerNames();
      List<int> list = ((IEnumerable<int>) SortingLayerProperty.GetSortingLayerUniqueIDs()).ToList<int>();
      int intValue = property.FindPropertyRelative("layerID").intValue;
      int selectedIndex = Mathf.Max(0, list.IndexOf(intValue));
      int index = EditorGUI.Popup(rect1, selectedIndex, sortingLayerNames);
      if (index != selectedIndex)
        property.FindPropertyRelative("layerID").intValue = list[index];
      rect1.x += rect1.width;
      property.FindPropertyRelative("order").intValue = EditorGUI.IntField(rect1, property.FindPropertyRelative("order").intValue);
      EditorGUI.indentLevel = indentLevel;
      EditorGUI.EndProperty();
    }

    public static string[] GetSortingLayerNames() => (string[]) typeof (InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null, new object[0]);

    public static int[] GetSortingLayerUniqueIDs() => (int[]) typeof (InternalEditorUtility).GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null, new object[0]);

    public static void DrawSortingLayerAndOrder(string name, SortingLayerAndOrder sorting)
    {
      Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
      Rect rect = EditorGUI.PrefixLabel(controlRect, new GUIContent(name));
      controlRect.xMin = rect.x;
      Rect position = new Rect(controlRect);
      position.width /= 2f;
      string[] sortingLayerNames = SortingLayerProperty.GetSortingLayerNames();
      List<int> list = ((IEnumerable<int>) SortingLayerProperty.GetSortingLayerUniqueIDs()).ToList<int>();
      int selectedIndex = Mathf.Max(0, list.IndexOf(sorting.layerID));
      sorting.layerID = list.Get<int>(EditorGUI.Popup(position, selectedIndex, sortingLayerNames));
      position.x += position.width;
      sorting.order = EditorGUI.IntField(position, sorting.order);
    }
  }
}
