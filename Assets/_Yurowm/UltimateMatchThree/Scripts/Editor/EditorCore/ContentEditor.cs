// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ContentEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Content")]
  [BerryPanelTab("Content", "ContentTabIcon", -9999)]
  public class ContentEditor : MetaEditor<Yurowm.GameCore.Content>
  {
    private static Texture2D contentIcon;
    private ContentEditor.ContentList contentList;
    private GUIHelper.SearchPanel searchPanel;
    private Vector2 contentListScroll;
    private static bool needToBeUpdated;

    [InitializeOnLoadMethod]
    internal static void Start()
    {
      Yurowm.GameCore.Content main = MonoBehaviourAssistant<Yurowm.GameCore.Content>.main;
      if (!(bool) (UnityEngine.Object) main)
        return;
      string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Trim();
      Yurowm.GameCore.Content content = main;
      string[] strArray;
      if (!string.IsNullOrEmpty(str))
        strArray = ((IEnumerable<string>) str.Split(';')).Select<string, string>((Func<string, string>) (x => x.Trim().ToUpper())).ToArray<string>();
      else
        strArray = new string[0];
      content.SDSymbols = strArray;
    }

    public override bool Initialize()
    {
      ContentEditor.contentIcon = EditorIcons.GetBuildInIcon("ContentIcon");
      this.metaTarget.cItems.RemoveAll((Predicate<Yurowm.GameCore.Content.Item>) (x => (UnityEngine.Object) x.item == (UnityEngine.Object) null));
      this.contentList = new ContentEditor.ContentList(this.metaTarget.cItems, this.metaTarget.categories);
      this.contentList.onSelectedItemChanged = (Action<List<Yurowm.GameCore.Content.Item>>) (s => Selection.objects = (UnityEngine.Object[]) s.Select<Yurowm.GameCore.Content.Item, GameObject>((Func<Yurowm.GameCore.Content.Item, GameObject>) (x => x.item)).ToArray<GameObject>());
      this.searchPanel = new GUIHelper.SearchPanel("");
      return (bool) (UnityEngine.Object) this.metaTarget;
    }

    [MenuItem("Assets/Add To/Content")]
    public static void AddTo()
    {
      if ((UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main == (UnityEngine.Object) null)
      {
        Debug.Log((object) "Content manager is missing");
      }
      else
      {
        MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.Initialize();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
          GameObject go = gameObject;
          if (PrefabUtility.GetPrefabType((UnityEngine.Object) go) == PrefabType.Prefab && !MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems.Contains<Yurowm.GameCore.Content.Item>((Func<Yurowm.GameCore.Content.Item, bool>) (x => x.item.name == go.name)))
            MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems.Add(new Yurowm.GameCore.Content.Item(go));
        }
        ContentEditor.needToBeUpdated = true;
        MetaEditor<Yurowm.GameCore.Content>.Repaint();
      }
    }

    private void Sort()
    {
      MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems.Sort((Comparison<Yurowm.GameCore.Content.Item>) ((x, y) => x.item.name.CompareTo(y.item.name)));
      this.contentList.MarkAsChanged();
    }

    public override void OnGUI()
    {
      if (!(bool) (UnityEngine.Object) this.metaTarget)
      {
        EditorGUILayout.HelpBox("ContentAssistant is missing", MessageType.Error);
      }
      else
      {
        using (new GUIHelper.Horizontal(EditorStyles.toolbar, new GUILayoutOption[0]))
        {
          if (GUILayout.Button("New Group", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            this.contentList.AddNewFolder((GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.FolderInfo) null, "Folder{0}");
          if (GUILayout.Button("Sort", EditorStyles.toolbarButton, GUILayout.Width(50f)))
            this.Sort();
          this.searchPanel.OnGUI((Action<string>) (x => this.contentList.SetSearchFilter(x)), GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.HelpBox("Use drag and drop function to add new prefab to the Content manager. It can't be scene object, and it must have unique name.", MessageType.Info);
        if (ContentEditor.needToBeUpdated)
        {
          this.contentList.MarkAsChanged();
          ContentEditor.needToBeUpdated = false;
        }
        Undo.RecordObject((UnityEngine.Object) this.metaTarget, "");
        using (new GUIHelper.Vertical(Styles.area, new GUILayoutOption[2]
        {
          GUILayout.ExpandWidth(true),
          GUILayout.ExpandHeight(true)
        }))
        {
          this.contentListScroll = EditorGUILayout.BeginScrollView(this.contentListScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
          this.contentList.OnGUI(GUILayoutUtility.GetRect(100f, 100f, GUILayout.MinHeight(this.contentList.totalHeight + 200f), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)));
          EditorGUILayout.EndScrollView();
        }
      }
    }

    public override Yurowm.GameCore.Content FindTarget() => MonoBehaviourAssistant<Yurowm.GameCore.Content>.main;

    private class ContentList : GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item>
    {
      internal const string newGroupNameFormat = "Folder{0}";
      private UnityEngine.Color transparentColor = new UnityEngine.Color(1f, 1f, 1f, 0.5f);

      public ContentList(List<Yurowm.GameCore.Content.Item> collection, List<TreeFolder> folders)
        : base(collection, folders, new TreeViewState())
      {
      }

      public override void ContextMenu(
        GenericMenu menu,
        List<GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo> selected)
      {
        if (selected.Count == 0)
        {
          menu.AddItem(new GUIContent("New Group"), false, (GenericMenu.MenuFunction) (() => this.AddNewFolder((GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.FolderInfo) null, "Folder{0}")));
        }
        else
        {
          if (selected.Count == 1 && selected[0].isFolderKind)
          {
            GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.FolderInfo parent = selected[0].asFolderKind;
            menu.AddItem(new GUIContent("Add New Group"), false, (GenericMenu.MenuFunction) (() => this.AddNewFolder(parent, "Folder{0}")));
          }
          else
          {
            GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.FolderInfo parent = selected[0].parent;
            if (selected.All<GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo>((Func<GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo, bool>) (x => x.parent == parent)))
              menu.AddItem(new GUIContent("Group"), false, (GenericMenu.MenuFunction) (() => this.Group(selected, parent, "Folder{0}")));
            else
              menu.AddItem(new GUIContent("Group"), false, (GenericMenu.MenuFunction) (() => this.Group(selected, this.root, "Folder{0}")));
          }
          menu.AddItem(new GUIContent("Remove"), false, (GenericMenu.MenuFunction) (() => this.Remove(selected.ToArray())));
        }
      }

      public override void DrawItem(
        Rect rect,
        GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.ItemInfo info)
      {
        if ((UnityEngine.Object) ContentEditor.contentIcon == (UnityEngine.Object) null)
          ContentEditor.contentIcon = EditorIcons.GetBuildInIcon("ContentIcon");
        Rect position = new Rect(rect.x, rect.y, 16f, rect.height);
        GUI.DrawTexture(position, (Texture) ContentEditor.contentIcon);
        position = new Rect(rect.x + 16f, rect.y, rect.width - 16f, rect.height);
        if (string.IsNullOrEmpty(this.searchFilter))
          GUI.Label(position, info.content.item.name);
        else
          GUI.Label(position, GUIHelper.SearchPanel.Format(info.fullPath, this.searchFilter), GUIHelper.SearchPanel.keyItemStyle);
      }

      public override bool ObjectToItem(
        UnityEngine.Object o,
        out GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo result)
      {
        result = (GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo) null;
        if (!(o is GameObject) || PrefabUtility.GetPrefabType(o) != PrefabType.Prefab || MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems.Contains<Yurowm.GameCore.Content.Item>((Func<Yurowm.GameCore.Content.Item, bool>) (x => x.item.name == o.name)))
          return false;
        result = (GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.IInfo) new GUIHelper.HierarchyList<Yurowm.GameCore.Content.Item, TreeFolder>.ItemInfo(0)
        {
          content = new Yurowm.GameCore.Content.Item(o as GameObject)
        };
        return true;
      }

      public override string GetPath(Yurowm.GameCore.Content.Item element) => element.path;

      public override void SetPath(Yurowm.GameCore.Content.Item element, string path) => element.path = path;

      public override string GetName(Yurowm.GameCore.Content.Item element) => element.item.name;

      public override int GetUniqueID(Yurowm.GameCore.Content.Item element) => element.item.GetInstanceID();

      public override Yurowm.GameCore.Content.Item CreateItem() => (Yurowm.GameCore.Content.Item) null;
    }

    public class Selector : ContentEditor.ISelector<GameObject>
    {
      public Selector(Func<GameObject, bool> filter = null)
        : base(filter)
      {
      }

      protected override GameObject GetValue(GameObject gameObject) => gameObject;
    }

    public class Selector<T> : ContentEditor.ISelector<T> where T : Component
    {
      public Selector(Func<T, bool> filter = null)
        : base(filter)
      {
      }

      protected override T GetValue(GameObject gameObject) => gameObject.GetComponent<T>();
    }

    public abstract class ISelector<I> : IEnumerable<I>, IEnumerable where I : UnityEngine.Object
    {
      private Func<I, bool> filter;
      private List<I> values = new List<I>();
      private string[] names;

      public ISelector(Func<I, bool> filter = null)
      {
        this.filter = filter;
        this.Refresh();
      }

      public void Refresh()
      {
        this.values.Clear();
        if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
          return;
        if (!MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.IsInitialized)
          MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.Initialize();
        foreach (Yurowm.GameCore.Content.Item cItem in MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.cItems)
        {
          I i = this.GetValue(cItem.item);
          if (!((UnityEngine.Object) i == (UnityEngine.Object) null) && (this.filter == null || this.filter(i)))
            this.values.Add(i);
        }
        this.values.Sort((Comparison<I>) ((x, y) => x.name.CompareTo(y.name)));
        this.names = this.values.Select<I, string>((Func<I, string>) (x => x.name)).ToArray<string>();
      }

      protected abstract I GetValue(GameObject gameObject);

      public I Select(string label, I current, params GUILayoutOption[] options)
      {
        if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
        {
          EditorGUILayout.LabelField(label, "Content provider is missing", options);
          return default (I);
        }
        if (this.values.Count == 0)
        {
          EditorGUILayout.LabelField(label, "There isn't any options", options);
          return default (I);
        }
        int selectedIndex = Mathf.Max(0, this.values.IndexOf(current));
        return this.values[EditorGUILayout.Popup(label, selectedIndex, this.names, options)];
      }

      public IEnumerator<I> GetEnumerator()
      {
        foreach (I i in this.values)
          yield return i;
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public I this[int i] => i >= 0 && i < this.values.Count ? this.values[i] : default (I);

      public I this[string name] => this[this.names.IndexOf<string>(name)];

      public int Count => this.values.Count;

      public bool Contains(I item) => this.values.Contains(item);

      public bool Contains(int index) => index >= 0 && index < this.Count;

      public bool Contains(string name) => ((IEnumerable<string>) this.names).Contains<string>(name);

      public bool Contains(Func<I, bool> match) => this.values.Contains<I>(match);
    }
  }
}
