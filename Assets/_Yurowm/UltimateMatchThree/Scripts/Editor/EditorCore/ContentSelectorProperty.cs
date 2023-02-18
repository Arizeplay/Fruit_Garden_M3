// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ContentSelectorProperty
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [CustomPropertyDrawer(typeof (ContentSelector))]
  public class ContentSelectorProperty : PropertyDrawer
  {
    private const int buttonSize = 20;
    private List<Component> values;
    private System.Type targetType;
    private string[] names;

    public void Refresh(SerializedProperty property)
    {
      this.values = new List<Component>();
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
        return;
      System.Type type1 = typeof (Component);
      System.Type type = (this.attribute as ContentSelector).targetType;
      this.targetType = this.fieldInfo.FieldType;
      if (!type1.IsAssignableFrom(this.targetType))
      {
        System.Type[] genericArguments = this.fieldInfo.FieldType.GetGenericArguments();
        if (genericArguments.Length == 1)
          this.targetType = genericArguments[0];
      }
      if (!type1.IsAssignableFrom(this.targetType))
        this.targetType = (System.Type) null;
      if (this.targetType == null)
        return;
      Dictionary<Component, string> path = new Dictionary<Component, string>();
      if (!MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.IsInitialized)
        MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.Initialize();
      foreach (Yurowm.GameCore.Content.Item cItem in MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems)
      {
        if ((bool) (UnityEngine.Object) cItem.item)
        {
          Component key = type != null ? ((IEnumerable<Component>) cItem.item.GetComponents(this.targetType)).FirstOrDefault<Component>((Func<Component, bool>) (x => type.IsAssignableFrom(x.GetType()))) : cItem.item.GetComponent(this.targetType);
          if (!((UnityEngine.Object) key == (UnityEngine.Object) null))
          {
            this.values.Add(key);
            path.Set<Component, string>(key, (cItem.path.Length > 0 ? cItem.path + "/" : "") + cItem.item.name);
          }
        }
      }
      this.values.Sort((Comparison<Component>) ((x, y) => x.name.CompareTo(y.name)));
      this.values.Insert(0, (Component) null);
      this.names = this.values.Select<Component, string>((Func<Component, string>) (x => !(bool) (UnityEngine.Object) x ? "<Null>" : path.Get<Component, string>(x))).ToArray<string>();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (this.values == null)
        this.Refresh(property);
      Rect position1 = EditorGUI.PrefixLabel(position, label, property.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label);
      position1.xMin -= (float) (property.depth * 16);
      if (!typeof (Component).IsAssignableFrom(this.targetType))
        EditorGUI.HelpBox(position1, "Wrong usage of ContentSelector", MessageType.None);
      else if (this.values.Count == 0)
        EditorGUI.HelpBox(position1, "No Values", MessageType.None);
      else if (property.isArray)
      {
        while (property.NextVisible(true))
        {
          Component result = (Component) property.objectReferenceValue;
          if (this.DrawSelector(position1, result, property, out result))
            property.objectReferenceValue = (UnityEngine.Object) result;
          GUILayoutUtility.GetRect(0.0f, 20f);
        }
      }
      else
      {
        Component result = (Component) property.objectReferenceValue;
        if (!this.DrawSelector(position1, result, property, out result))
          return;
        property.objectReferenceValue = (UnityEngine.Object) result;
      }
    }

    private bool DrawSelector(
      Rect position,
      Component selected,
      SerializedProperty property,
      out Component result)
    {
      int selectedIndex = Mathf.Max(0, this.values.IndexOf(selected));
      if (property.hasMultipleDifferentValues)
      {
        int index = EditorGUI.Popup(position, -1, this.names);
        if (index != -1)
        {
          result = this.values[index];
          return true;
        }
        result = (Component) null;
        return false;
      }
      int index1 = EditorGUI.Popup(position, selectedIndex, this.names);
      result = this.values[index1];
      return true;
    }
  }
}
