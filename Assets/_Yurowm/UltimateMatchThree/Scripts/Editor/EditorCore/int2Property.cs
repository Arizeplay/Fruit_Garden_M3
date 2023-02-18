// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.int2Property
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [CustomPropertyDrawer(typeof (int2))]
  public class int2Property : PropertyDrawer
  {
    private List<string> pathes = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
      int indentLevel = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;
      Rect position1 = new Rect(position.x, position.y, 12f, position.height);
      Rect position2 = new Rect(position1.xMax, position.y, position.width / 2f - position1.width, position.height);
      Rect position3 = new Rect(position2.xMax, position.y, 12f, position.height);
      Rect position4 = new Rect(position3.xMax, position.y, position.width / 2f - position1.width, position.height);
      GUI.Label(position1, "X");
      EditorGUI.PropertyField(position2, property.FindPropertyRelative("x"), GUIContent.none);
      GUI.Label(position3, "Y");
      SerializedProperty propertyRelative = property.FindPropertyRelative("y");
      GUIContent none = GUIContent.none;
      EditorGUI.PropertyField(position4, propertyRelative, none);
      EditorGUI.indentLevel = indentLevel;
      EditorGUI.EndProperty();
    }
  }
}
