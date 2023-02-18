// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.RangeDrawer
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [CustomPropertyDrawer(typeof (IntRange))]
  [CustomPropertyDrawer(typeof (FloatRange))]
  public class RangeDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
      int indentLevel = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;
      Rect position1 = new Rect(position.x, position.y, position.width / 2f, position.height);
      Rect position2 = new Rect(position1.x + position1.width, position.y, position1.width, position.height);
      EditorGUI.PropertyField(position1, property.FindPropertyRelative("min"), GUIContent.none);
      SerializedProperty propertyRelative = property.FindPropertyRelative("max");
      GUIContent none = GUIContent.none;
      EditorGUI.PropertyField(position2, propertyRelative, none);
      EditorGUI.indentLevel = indentLevel;
      EditorGUI.EndProperty();
    }
  }
}
