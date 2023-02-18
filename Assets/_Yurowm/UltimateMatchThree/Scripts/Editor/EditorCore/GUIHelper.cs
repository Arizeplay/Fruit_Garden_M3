// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.GUIHelper
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  public static class GUIHelper
  {
    public static void DrawSprite(Sprite sprite) => GUIHelper.DrawSprite(sprite, (float) sprite.texture.width, (float) sprite.texture.height);

    public static void DrawSprite(Sprite sprite, float width, float height)
    {
      Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.Width(width), GUILayout.Height(height));
      if (!((UnityEngine.Object) sprite != (UnityEngine.Object) null))
        return;
      Rect rect = sprite.rect;
      rect.x /= (float) sprite.texture.width;
      rect.width /= (float) sprite.texture.width;
      rect.y /= (float) sprite.texture.height;
      rect.height /= (float) sprite.texture.height;
      GUI.DrawTextureWithTexCoords(controlRect, (Texture) sprite.texture, rect);
    }

    public class Vertical : IDisposable
    {
      public Vertical(params GUILayoutOption[] options) => EditorGUILayout.BeginVertical(options);

      public Vertical(out Rect rect, params GUILayoutOption[] options) => rect = EditorGUILayout.BeginVertical(options);

      public Vertical(GUIStyle style, params GUILayoutOption[] options) => EditorGUILayout.BeginVertical(style, options);

      public Vertical(out Rect rect, GUIStyle style, params GUILayoutOption[] options) => rect = EditorGUILayout.BeginVertical(style, options);

      public void Dispose() => EditorGUILayout.EndVertical();
    }

    public class Horizontal : IDisposable
    {
      public Horizontal(params GUILayoutOption[] options) => EditorGUILayout.BeginHorizontal(options);

      public Horizontal(out Rect rect, params GUILayoutOption[] options) => rect = EditorGUILayout.BeginHorizontal(options);

      public Horizontal(GUIStyle style, params GUILayoutOption[] options) => EditorGUILayout.BeginHorizontal(style, options);

      public Horizontal(out Rect rect, GUIStyle style, params GUILayoutOption[] options) => rect = EditorGUILayout.BeginHorizontal(style, options);

      public void Dispose() => EditorGUILayout.EndHorizontal();
    }

    public class Scroll
    {
      private Vector2 _position;
      private GUILayoutOption[] options;
      private GUIStyle style;

      public Vector2 position => this._position;

      public Scroll(GUIStyle style, params GUILayoutOption[] options)
        : this(options)
      {
        this.style = style;
      }

      public Scroll(params GUILayoutOption[] options) => this.options = options;

      public GUIHelper.Scroll.ScrollDisposable Start() => new GUIHelper.Scroll.ScrollDisposable(ref this._position, this.style, this.options);

      public void ScrollTo(float y = 0.0f) => this._position.y = y;

      public void ScrollTo(float x = 0.0f, float y = 0.0f)
      {
        this._position.x = x;
        this._position.y = y;
      }

      public class ScrollDisposable : IDisposable
      {
        public ScrollDisposable(
          ref Vector2 position,
          GUIStyle style,
          params GUILayoutOption[] options)
        {
          if (style == null)
            position = EditorGUILayout.BeginScrollView(position, options);
          else
            position = EditorGUILayout.BeginScrollView(position, style, options);
        }

        public void Dispose() => EditorGUILayout.EndScrollView();
      }
    }

    public class Lock : IDisposable
    {
      private bool memory;

      public Lock(bool enabled)
      {
        this.memory = GUI.enabled;
        if (!this.memory)
          return;
        GUI.enabled = !enabled;
      }

      public void Dispose() => GUI.enabled = this.memory;
    }

    public class Color : IDisposable
    {
      private UnityEngine.Color memory;

      public Color(UnityEngine.Color color)
      {
        this.memory = GUI.color;
        GUI.color = color;
      }

      public void Dispose() => GUI.color = this.memory;
    }

    public class ContentColor : IDisposable
    {
      private UnityEngine.Color memory;

      public ContentColor(UnityEngine.Color color)
      {
        this.memory = GUI.contentColor;
        GUI.contentColor = color;
      }

      public void Dispose() => GUI.contentColor = this.memory;
    }

    public class BackgroundColor : IDisposable
    {
      private UnityEngine.Color memory;

      public BackgroundColor(UnityEngine.Color color)
      {
        this.memory = GUI.backgroundColor;
        GUI.backgroundColor = color;
      }

      public void Dispose() => GUI.backgroundColor = this.memory;
    }

    public class Change : IDisposable
    {
      private Action onChanged;

      public Change(Action onChanged)
      {
        EditorGUI.BeginChangeCheck();
        this.onChanged = onChanged;
      }

      public void Dispose()
      {
        if (!EditorGUI.EndChangeCheck())
          return;
        this.onChanged();
      }
    }

    public class LayoutSplitter
    {
      private int resize = -1;
      private int current;
      private float[] sizes;
      private Rect lastRect;
      public float thickness = 4f;
      public Action<Rect> drawCursor = (Action<Rect>) (_param1 => { });
      private OrientationLine orientation;
      private OrientationLine internalOrientation;
      private bool areaStarted;
      private bool firstArea;
      private bool[] mask;
      private int last = -1;

      public LayoutSplitter(
        OrientationLine orientation,
        OrientationLine internalOrientation,
        params float[] sizes)
      {
        this.sizes = sizes;
        this.orientation = orientation;
        this.internalOrientation = internalOrientation;
      }

      public bool Area(GUIStyle style = null)
      {
        if (this.IsLast() && !this.firstArea)
          return false;
        if (this.mask != null && !this.mask[this.current])
        {
          ++this.current;
          return false;
        }
        if (this.areaStarted)
        {
          this.areaStarted = false;
          this.EndLayout(this.internalOrientation);
        }
        if (!this.firstArea)
        {
          this.lastRect = GUILayoutUtility.GetLastRect();
          Rect controlRect = EditorGUILayout.GetControlRect(this.Expand(this.Anti(this.orientation)), this.Size(this.orientation, this.thickness));
          this.drawCursor(controlRect);
          EditorGUIUtility.AddCursorRect(controlRect, this.orientation == OrientationLine.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
          if (this.resize < 0 && UnityEngine.Event.current.type == UnityEngine.EventType.MouseDown && controlRect.Contains(UnityEngine.Event.current.mousePosition))
            this.resize = this.current;
          if (this.resize == this.current)
          {
            float num = 0.0f;
            switch (this.orientation)
            {
              case OrientationLine.Horizontal:
                num = Mathf.Max(this.lastRect.width - (float) ((double) controlRect.x - (double) UnityEngine.Event.current.mousePosition.x + (double) this.thickness / 2.0), 50f);
                break;
              case OrientationLine.Vertical:
                num = Mathf.Max(this.lastRect.height - (float) ((double) controlRect.y - (double) UnityEngine.Event.current.mousePosition.y + (double) this.thickness / 2.0), 50f);
                break;
            }
            this.sizes[this.current] += num - this.sizes[this.current];
            if ((bool) (UnityEngine.Object) EditorWindow.mouseOverWindow)
              EditorWindow.mouseOverWindow.Repaint();
            if (UnityEngine.Event.current.type == UnityEngine.EventType.MouseUp)
              this.resize = -1;
          }
        }
        if (this.firstArea)
          this.firstArea = false;
        else
          ++this.current;
        if (this.IsLast())
          this.BeginLayout(this.internalOrientation, style);
        else
          this.BeginLayout(this.internalOrientation, style, this.Size(this.orientation, this.sizes[this.current]));
        this.areaStarted = true;
        return this.mask == null || this.mask[this.current];
      }

      private OrientationLine Anti(OrientationLine o)
      {
        if (o == OrientationLine.Horizontal)
          return OrientationLine.Vertical;
        return o == OrientationLine.Vertical ? OrientationLine.Horizontal : OrientationLine.Horizontal;
      }

      private Rect BeginLayout(
        OrientationLine o,
        GUIStyle style = null,
        params GUILayoutOption[] options)
      {
        switch (o)
        {
          case OrientationLine.Horizontal:
            return style != null ? EditorGUILayout.BeginHorizontal(style, options) : EditorGUILayout.BeginHorizontal(options);
          case OrientationLine.Vertical:
            return style != null ? EditorGUILayout.BeginVertical(style, options) : EditorGUILayout.BeginVertical(options);
          default:
            return new Rect();
        }
      }

      private void EndLayout(OrientationLine o, params GUILayoutOption[] options)
      {
        if (o != OrientationLine.Horizontal)
        {
          if (o != OrientationLine.Vertical)
            return;
          EditorGUILayout.EndVertical();
        }
        else
          EditorGUILayout.EndHorizontal();
      }

      private GUILayoutOption Size(OrientationLine o, float size)
      {
        if (o == OrientationLine.Horizontal)
          return GUILayout.Width(size);
        return o == OrientationLine.Vertical ? GUILayout.Height(size) : (GUILayoutOption) null;
      }

      private GUILayoutOption Expand(OrientationLine o)
      {
        if (o == OrientationLine.Horizontal)
          return GUILayout.ExpandWidth(true);
        return o == OrientationLine.Vertical ? GUILayout.ExpandHeight(true) : (GUILayoutOption) null;
      }

      public GUIHelper.LayoutSplitter.Splitter Start(params bool[] mask)
      {
        this.last = this.sizes.Length;
        this.UpdateMask(mask);
        this.current = 0;
        this.areaStarted = false;
        this.firstArea = true;
        this.BeginLayout(this.orientation, (GUIStyle) null, this.Expand(this.orientation));
        return this.current >= this.sizes.Length ? new GUIHelper.LayoutSplitter.Splitter((Action) (() => this.EndLayout(this.orientation))) : new GUIHelper.LayoutSplitter.Splitter((Action) (() =>
        {
          if (this.areaStarted)
            this.EndLayout(this.internalOrientation);
          this.EndLayout(this.orientation);
        }));
      }

      public void UpdateMask(params bool[] mask)
      {
        if (mask != null && mask.Length == this.sizes.Length + 1)
        {
          this.mask = mask;
          while (this.last >= 0 && !mask[this.last])
            --this.last;
        }
        else
          this.mask = (bool[]) null;
      }

      private bool IsLast() => this.current >= this.last;

      public class Splitter : IDisposable
      {
        private Action onDispose;

        public Splitter(Action onDispose) => this.onDispose = onDispose;

        public void Dispose() => this.onDispose();
      }
    }

    public abstract class HierarchyList<I, F> : TreeView
    {
      protected GUIHelper.HierarchyList<I, F>.FolderInfo root;
      public List<I> itemCollection;
      public List<F> folderCollection;
      internal Dictionary<int, GUIHelper.HierarchyList<I, F>.IInfo> info = new Dictionary<int, GUIHelper.HierarchyList<I, F>.IInfo>();
      internal Dictionary<string, F> folders = new Dictionary<string, F>();
      public Action<List<I>> onSelectedItemChanged = (Action<List<I>>) (_param1 => { });
      public Action<List<GUIHelper.HierarchyList<I, F>.IInfo>> onSelectionChanged = (Action<List<GUIHelper.HierarchyList<I, F>.IInfo>>) (_param1 => { });
      public Action onRebuild = (Action) (() => { });
      public Action onChanged = (Action) (() => { });
      public Action<List<GUIHelper.HierarchyList<I, F>.IInfo>> onRemove = (Action<List<GUIHelper.HierarchyList<I, F>.IInfo>>) (_param1 => { });
      protected string listName;
      private string _searchFilter = "";
      private bool reloadRequired;
      private List<GUIHelper.HierarchyList<I, F>.IInfo> drag = new List<GUIHelper.HierarchyList<I, F>.IInfo>();
      private bool _isDrag;
      private bool isContextOnItem;

      public HierarchyList(List<I> collection, List<F> folders, TreeViewState state, string name = null)
        : base(state)
      {
        this.listName = name;
        this.itemCollection = collection;
        this.folderCollection = folders != null ? folders : new List<F>();
        this.Reload();
      }

      protected override TreeViewItem BuildRoot()
      {
        this.info.Clear();
        this.root = new GUIHelper.HierarchyList<I, F>.FolderInfo(-1);
        this.folders = this.folderCollection.GroupBy<F, string>((Func<F, string>) (x => this.GetFullPath(x))).ToDictionary<IGrouping<string, F>, string, F>((Func<IGrouping<string, F>, string>) (x => x.Key), (Func<IGrouping<string, F>, F>) (x => x.First<F>()));
        foreach (I element in this.itemCollection)
        {
          GUIHelper.HierarchyList<I, F>.FolderInfo parent = this.AddFolder(this.GetPath(element));
          GUIHelper.HierarchyList<I, F>.ItemInfo itemInfo = new GUIHelper.HierarchyList<I, F>.ItemInfo(parent.item.depth + 1, parent);
          itemInfo.content = element;
          itemInfo.item.displayName = this.GetName(element);
          parent.items.Add((GUIHelper.HierarchyList<I, F>.IInfo) itemInfo);
        }
        this.folderCollection.Clear();
        this.folderCollection.AddRange((IEnumerable<F>) this.folders.Values);
        foreach (F element in this.folderCollection.ToList<F>())
          this.AddFolder(this.GetFullPath(element));
        if (!string.IsNullOrEmpty(this._searchFilter))
        {
          string lower = this._searchFilter.ToLower();
          bool flag = this._searchFilter.Contains<char>('/');
          GUIHelper.HierarchyList<I, F>.FolderInfo folderInfo = new GUIHelper.HierarchyList<I, F>.FolderInfo(-1);
          foreach (GUIHelper.HierarchyList<I, F>.IInfo info in this.root.GetAllChild())
          {
            if ((flag ? this.GetFullPath(info) : this.GetName(info)).ToLower().Contains(lower))
            {
              info.item.parent = folderInfo.item;
              info.item.depth = 0;
              folderInfo.items.Add(info);
              if (info is GUIHelper.HierarchyList<I, F>.FolderInfo)
                (info as GUIHelper.HierarchyList<I, F>.FolderInfo).items.Clear();
            }
          }
          this.root = folderInfo;
        }
        this.root.GetAllChild().ForEach((Action<GUIHelper.HierarchyList<I, F>.IInfo>) (x => x.item.id = this.GetUniqueID(x)));
        this.info.Clear();
        foreach (GUIHelper.HierarchyList<I, F>.IInfo info in this.root.GetAllChild())
        {
          if (this.info.ContainsKey(info.item.id))
            throw new ArgumentException(string.Format("These two elements has the same ID ({0})\n{1}\n{2}", (object) info.item.id, (object) this.GetFullPath(info), (object) this.GetFullPath(this.info[info.item.id])));
          this.info.Add(info.item.id, info);
        }
        TreeView.SetupParentsAndChildrenFromDepths(this.root.item, (IList<TreeViewItem>) this.root.GetAllChild().Select<GUIHelper.HierarchyList<I, F>.IInfo, TreeViewItem>((Func<GUIHelper.HierarchyList<I, F>.IInfo, TreeViewItem>) (x => x.item)).ToList<TreeViewItem>());
        this.onRebuild();
        this.SelectionChanged((IList<int>) this.state.selectedIDs);
        return this.root.item;
      }

      public string searchFilter => this._searchFilter;

      public void SetSearchFilter(string filter)
      {
        if (!(this._searchFilter != filter))
          return;
        this._searchFilter = filter;
        this.Reload();
      }

      public GUIHelper.HierarchyList<I, F>.IInfo GetInfo(I item) => this.info.Values.FirstOrDefault<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x.isItemKind && x.asItemKind.content.Equals((object) item)));

      protected override void SelectionChanged(IList<int> selectedIds)
      {
        if (this.onSelectedItemChanged.GetInvocationList().Length == 0)
          return;
        List<I> iList = new List<I>();
        foreach (int selectedId in (IEnumerable<int>) selectedIds)
        {
          GUIHelper.HierarchyList<I, F>.IInfo iinfo = this.info.Get<int, GUIHelper.HierarchyList<I, F>.IInfo>(selectedId);
          if (iinfo != null && iinfo.isItemKind)
            iList.Add(iinfo.asItemKind.content);
        }
        this.onSelectedItemChanged(iList);
        this.onSelectionChanged(selectedIds.Select<int, GUIHelper.HierarchyList<I, F>.IInfo>((Func<int, GUIHelper.HierarchyList<I, F>.IInfo>) (x => this.info.Get<int, GUIHelper.HierarchyList<I, F>.IInfo>(x))).Where<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x != null)).ToList<GUIHelper.HierarchyList<I, F>.IInfo>());
      }

      public abstract bool ObjectToItem(UnityEngine.Object o, out GUIHelper.HierarchyList<I, F>.IInfo result);

      protected override bool CanStartDrag(TreeView.CanStartDragArgs args) => true;

      protected override void RowGUI(TreeView.RowGUIArgs args)
      {
        GUIHelper.HierarchyList<I, F>.IInfo iinfo = this.info.Get<int, GUIHelper.HierarchyList<I, F>.IInfo>(args.item.id);
        if (iinfo == null)
          return;
        Rect rect = new Rect(args.rowRect);
        float num = (float) ((args.item.depth + 1) * 16);
        rect.x += num;
        rect.width -= num;
        if (iinfo.isItemKind)
          this.DrawItem(rect, iinfo.asItemKind);
        else
          this.DrawFolder(rect, iinfo.asFolderKind);
      }

      protected override float GetCustomRowHeight(int row, TreeViewItem item) => !this.info[item.id].isItemKind ? this.FolderRowHeight() : this.ItemRowHeight();

      protected override bool CanRename(TreeViewItem item)
      {
        GUIHelper.HierarchyList<I, F>.IInfo iinfo = this.info.Get<int, GUIHelper.HierarchyList<I, F>.IInfo>(item.id);
        if (iinfo == null)
          return false;
        return !iinfo.isItemKind ? this.CanRename(iinfo.asFolderKind) : this.CanRename(iinfo.asItemKind);
      }

      public virtual bool CanRename(GUIHelper.HierarchyList<I, F>.ItemInfo info) => true;

      public virtual bool CanRename(GUIHelper.HierarchyList<I, F>.FolderInfo info) => true;

      protected override void RenameEnded(TreeView.RenameEndedArgs args)
      {
        if (args.originalName == args.newName || args.newName.Contains<char>('/'))
          return;
        GUIHelper.HierarchyList<I, F>.IInfo info = this.info[args.itemID];
        if (info.parent.items.Contains<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.GetName(x) == args.newName)))
          return;
        this.SetName(info, args.newName);
        info.item.displayName = args.newName;
        this.UpdatePath(new List<GUIHelper.HierarchyList<I, F>.IInfo>()
        {
          info
        });
        this.Reload();
        this.onChanged();
      }

      public virtual float FolderRowHeight() => 16f;

      public virtual float ItemRowHeight() => 16f;

      public virtual bool CanBeChild(
        GUIHelper.HierarchyList<I, F>.IInfo parent,
        GUIHelper.HierarchyList<I, F>.IInfo child)
      {
        if (!parent.isFolderKind)
          return false;
        return child.parent == parent || parent.asFolderKind.items.FirstOrDefault<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.GetName(x) == this.GetName(child))) == null;
      }

      public abstract void DrawItem(Rect rect, GUIHelper.HierarchyList<I, F>.ItemInfo info);

      public abstract void DrawFolder(Rect rect, GUIHelper.HierarchyList<I, F>.FolderInfo info);

      public abstract string GetPath(I element);

      public abstract string GetPath(F element);

      private string GetFullPath(GUIHelper.HierarchyList<I, F>.IInfo info) => info.isItemKind ? this.GetFullPath(info.asItemKind.content) : this.GetFullPath(info.asFolderKind.content);

      private string GetFullPath(I element)
      {
        string path = this.GetPath(element);
        string name = this.GetName(element);
        return path.Length > 0 ? path + "/" + name : name;
      }

      private string GetFullPath(F element)
      {
        string path = this.GetPath(element);
        string name = this.GetName(element);
        return path.Length > 0 ? path + "/" + name : name;
      }

      public void SetPath(GUIHelper.HierarchyList<I, F>.IInfo info, string path)
      {
        if (info.isItemKind)
          this.SetPath(info.asItemKind.content, path);
        else
          this.SetPath(info.asFolderKind.content, path);
      }

      public abstract void SetPath(I element, string path);

      public abstract void SetPath(F element, string path);

      public void SetName(GUIHelper.HierarchyList<I, F>.IInfo info, string name)
      {
        if (info.isItemKind)
          this.SetName(info.asItemKind.content, name);
        else
          this.SetName(info.asFolderKind.content, name);
      }

      public virtual void SetName(I element, string name)
      {
      }

      public virtual void SetName(F element, string name)
      {
      }

      public string GetName(GUIHelper.HierarchyList<I, F>.IInfo info) => !info.isItemKind ? this.GetName(info.asFolderKind.content) : this.GetName(info.asItemKind.content);

      public virtual string GetName(I element) => "Item";

      public virtual string GetName(F element)
      {
        string path = this.GetPath(element);
        int num = path.IndexOf("/");
        return num >= 0 ? path.Substring(num + 1, path.Length - num - 1) : path;
      }

      private int GetUniqueID(GUIHelper.HierarchyList<I, F>.IInfo element) => !(element is GUIHelper.HierarchyList<I, F>.ItemInfo) ? this.GetUniqueID((element as GUIHelper.HierarchyList<I, F>.FolderInfo).content) : this.GetUniqueID((element as GUIHelper.HierarchyList<I, F>.ItemInfo).content);

      public abstract int GetUniqueID(I element);

      public abstract int GetUniqueID(F element);

      protected GUIHelper.HierarchyList<I, F>.FolderInfo AddFolder(string fullPath)
      {
        GUIHelper.HierarchyList<I, F>.FolderInfo parent = this.root;
        if (!string.IsNullOrEmpty(fullPath))
        {
          string str1 = fullPath;
          char[] chArray = new char[1]{ '/' };
          foreach (string str2 in str1.Split(chArray))
          {
            string name = str2;
            if (!(name == ""))
            {
              GUIHelper.HierarchyList<I, F>.FolderInfo folderInfo = (GUIHelper.HierarchyList<I, F>.FolderInfo) parent.items.Find((Predicate<GUIHelper.HierarchyList<I, F>.IInfo>) (x => x.isFolderKind && this.GetName(x.asFolderKind.content) == name));
              if (folderInfo == null)
              {
                folderInfo = new GUIHelper.HierarchyList<I, F>.FolderInfo(parent.item.depth + 1, parent);
                parent.items.Add((GUIHelper.HierarchyList<I, F>.IInfo) folderInfo);
                folderInfo.parent = parent;
                folderInfo.item.displayName = name;
                string fullPath1 = folderInfo.fullPath;
                if (!this.folders.ContainsKey(fullPath1))
                {
                  F folder = this.CreateFolder();
                  this.SetPath(folder, parent.fullPath);
                  this.SetName(folder, name);
                  this.folderCollection.Add(folder);
                  this.folders.Add(fullPath1, folder);
                }
                folderInfo.content = this.folders[fullPath1];
              }
              parent = folderInfo;
            }
          }
        }
        return parent;
      }

      public void AddNewFolder(GUIHelper.HierarchyList<I, F>.FolderInfo folder, string nameFormat)
      {
        if (nameFormat == null || !nameFormat.Contains("{0}"))
          nameFormat = "Untitled{0}";
        if (folder == null)
          folder = this.root;
        string name = string.Format(nameFormat, (object) "");
        int num = 1;
        while (folder.items.Contains<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.GetName(x) == name)))
        {
          name = string.Format(nameFormat, (object) num);
          ++num;
        }
        string fullPath = folder.fullPath;
        F folder1 = this.CreateFolder();
        this.SetName(folder1, name);
        this.SetPath(folder1, fullPath);
        int uniqueId = this.GetUniqueID(folder1);
        this.folderCollection.Add(folder1);
        this.Reload();
        this.onChanged();
        TreeViewItem treeViewItem = this.FindItem(uniqueId, this.root.item);
        if (!this.CanRename(treeViewItem))
          return;
        this.BeginRename(treeViewItem);
      }

      public void Group(
        List<GUIHelper.HierarchyList<I, F>.IInfo> items,
        GUIHelper.HierarchyList<I, F>.FolderInfo parent,
        string nameFormat)
      {
        if (!nameFormat.Contains("{0}"))
          nameFormat = "Untitled{0}";
        if (parent == null)
          parent = this.root;
        string name = string.Format(nameFormat, (object) "");
        int num = 1;
        while (parent.items.Contains<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.GetName(x) == name)))
        {
          name = string.Format(nameFormat, (object) num);
          ++num;
        }
        GUIHelper.HierarchyList<I, F>.FolderInfo folderInfo = this.AddFolder(parent.fullPath + "/" + name);
        this.PutInFolder((GUIHelper.HierarchyList<I, F>.IInfo) folderInfo, parent, items.Min<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, int>) (x => x.index)));
        int uniqueId = this.GetUniqueID((GUIHelper.HierarchyList<I, F>.IInfo) folderInfo);
        foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in items)
          this.PutInFolder(iinfo, folderInfo);
        this.UpdatePath(items);
        this.Reload();
        this.onChanged();
        TreeViewItem treeViewItem = this.FindItem(uniqueId, this.root.item);
        if (!this.CanRename(treeViewItem))
          return;
        this.BeginRename(treeViewItem);
      }

      public abstract F CreateFolder();

      public void Remove(params GUIHelper.HierarchyList<I, F>.IInfo[] items)
      {
        if (items == null || items.Length == 0 || !EditorUtility.DisplayDialog(nameof (Remove), "Are you sure want to remove these items", nameof (Remove), "Cancel"))
          return;
        List<GUIHelper.HierarchyList<I, F>.IInfo> source = new List<GUIHelper.HierarchyList<I, F>.IInfo>();
        foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in items)
        {
          source.Add(iinfo);
          if (iinfo.isFolderKind)
            source.AddRange((IEnumerable<GUIHelper.HierarchyList<I, F>.IInfo>) iinfo.asFolderKind.GetAllChild());
        }
        List<GUIHelper.HierarchyList<I, F>.IInfo> list = source.Distinct<GUIHelper.HierarchyList<I, F>.IInfo>().ToList<GUIHelper.HierarchyList<I, F>.IInfo>();
        this.onRemove(list);
        foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in list)
        {
          if (iinfo.isItemKind)
            this.itemCollection.Remove(iinfo.asItemKind.content);
          if (iinfo.isFolderKind)
            this.folderCollection.Remove(iinfo.asFolderKind.content);
        }
        this.MarkAsChanged();
      }

      public void MarkAsChanged() => this.reloadRequired = true;

      public override void OnGUI(Rect rect)
      {
        base.OnGUI(rect);
        if (!this.reloadRequired)
          return;
        this.Reload();
        this.onChanged();
        this.reloadRequired = false;
      }

      public void OnGUI(params GUILayoutOption[] options) => this.OnGUI(EditorGUILayout.GetControlRect(options));

      public void OnGUI() => this.OnGUI(GUILayout.ExpandWidth(true), GUILayout.Height(Mathf.Max(this.rowHeight, this.totalHeight)));

      public void AddNewItem(GUIHelper.HierarchyList<I, F>.FolderInfo folder, string nameFormat)
      {
        if (nameFormat == null || !nameFormat.Contains("{0}"))
          nameFormat = "Untitled{0}";
        if (folder == null)
          folder = this.root;
        string name = string.Format(nameFormat, (object) "");
        int num = 1;
        while (folder.items.Contains<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.GetName(x) == name)))
        {
          name = string.Format(nameFormat, (object) num);
          ++num;
        }
        string fullPath = folder.fullPath;
        I element = this.CreateItem();
        this.SetName(element, name);
        this.SetPath(element, fullPath);
        int uniqueId = this.GetUniqueID(element);
        this.itemCollection.Add(element);
        this.Reload();
        this.onChanged();
        TreeViewItem treeViewItem = this.FindItem(uniqueId, this.root.item);
        if (!this.CanRename(treeViewItem))
          return;
        this.BeginRename(treeViewItem);
      }

      public abstract I CreateItem();

      protected override void SetupDragAndDrop(TreeView.SetupDragAndDropArgs args)
      {
        DragAndDrop.PrepareStartDrag();
        this.drag.Clear();
        foreach (int draggedItemId in (IEnumerable<int>) args.draggedItemIDs)
          this.drag.Add(this.root.Find(draggedItemId));
        DragAndDrop.paths = (string[]) null;
        DragAndDrop.objectReferences = new UnityEngine.Object[0];
        DragAndDrop.SetGenericData(typeof (I).Name, (object) this.drag.Where<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x.isItemKind)).Select<GUIHelper.HierarchyList<I, F>.IInfo, I>((Func<GUIHelper.HierarchyList<I, F>.IInfo, I>) (x => x.asItemKind.content)).ToList<I>());
        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        DragAndDrop.StartDrag(nameof (HierarchyList<I, F>));
        this._isDrag = true;
      }

      public bool isDrag => this._isDrag;

      protected override DragAndDropVisualMode HandleDragAndDrop(
        TreeView.DragAndDropArgs args)
      {
        if (!this._isDrag)
        {
          this.drag.Clear();
          this._isDrag = true;
          foreach (UnityEngine.Object objectReference in DragAndDrop.objectReferences)
          {
            GUIHelper.HierarchyList<I, F>.IInfo result;
            if (this.ObjectToItem(objectReference, out result) && result != null)
              this.drag.Add(result);
          }
        }
        DragAndDropVisualMode andDropVisualMode = DragAndDropVisualMode.None;
        if (args.performDrop || this.drag.Count == 0)
          this._isDrag = false;
        if (this.drag.Count == 0)
          return DragAndDropVisualMode.Rejected;
        if (args.parentItem == null)
          args.parentItem = this.root.item;
        GUIHelper.HierarchyList<I, F>.IInfo parent = this.root.item.id == args.parentItem.id ? (GUIHelper.HierarchyList<I, F>.IInfo) this.root : this.root.Find(args.parentItem.id);
        if (parent != null && !this.drag.All<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.CanBeChild(parent, x))))
          return DragAndDropVisualMode.Rejected;
        switch (args.dragAndDropPosition)
        {
          case TreeView.DragAndDropPosition.UponItem:
            if (parent.isItemKind || this.drag.Contains<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x.isFolderKind && x.asFolderKind.IsParentOf(parent.asFolderKind))))
              return DragAndDropVisualMode.Rejected;
            if (args.performDrop && parent != null && parent.isFolderKind && this.drag.All<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.CanBeChild(parent, x))))
            {
              int count = parent.asFolderKind.items.Count;
              foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in this.drag)
                this.PutInFolder(iinfo, parent.asFolderKind, count++);
              this.UpdatePath(this.drag);
              this.BakeCollections();
              this.onChanged();
              this.Reload();
            }
            return DragAndDropVisualMode.Move;
          case TreeView.DragAndDropPosition.BetweenItems:
          case TreeView.DragAndDropPosition.OutsideItems:
            if (parent.isFolderKind && args.performDrop && parent != null && parent.isFolderKind && this.drag.All<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => this.CanBeChild(parent, x))))
            {
              int num = 0;
              foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in this.drag)
                this.PutInFolder(iinfo, parent.asFolderKind, args.insertAtIndex + num++);
              this.UpdatePath(this.drag);
              this.BakeCollections();
              this.onChanged();
              this.Reload();
            }
            return DragAndDropVisualMode.Move;
          default:
            return andDropVisualMode;
        }
      }

      protected bool PutInFolder(
        GUIHelper.HierarchyList<I, F>.IInfo item,
        GUIHelper.HierarchyList<I, F>.FolderInfo folder)
      {
        if (item == folder)
          return false;
        if (item.parent != null)
          item.parent.items.Remove(item);
        item.parent = folder;
        folder.items.Add(item);
        return true;
      }

      protected bool PutInFolder(
        GUIHelper.HierarchyList<I, F>.IInfo item,
        GUIHelper.HierarchyList<I, F>.FolderInfo folder,
        int index)
      {
        if (item == folder)
          return false;
        if (item.parent == folder && folder.items.IndexOf(item) < index)
          --index;
        if (item.parent != null)
          item.parent.items.Remove(item);
        item.parent = folder;
        folder.items.Insert(Mathf.Clamp(index, 0, folder.items.Count), item);
        return true;
      }

      public GUIHelper.HierarchyList<I, F>.FolderInfo FindFolder(string path)
      {
        if (string.IsNullOrEmpty(path))
          return (GUIHelper.HierarchyList<I, F>.FolderInfo) null;
        GUIHelper.HierarchyList<I, F>.FolderInfo root = this.root;
        string str1 = path;
        char[] chArray = new char[1]{ '/' };
        foreach (string str2 in str1.Split(chArray))
        {
          string folder = str2;
          if (string.IsNullOrEmpty(folder))
            return (GUIHelper.HierarchyList<I, F>.FolderInfo) null;
          root = (GUIHelper.HierarchyList<I, F>.FolderInfo) root.items.Find((Predicate<GUIHelper.HierarchyList<I, F>.IInfo>) (x => x is GUIHelper.HierarchyList<I, F>.FolderInfo && (x as GUIHelper.HierarchyList<I, F>.FolderInfo).item.displayName == folder));
          if (root == null)
            return (GUIHelper.HierarchyList<I, F>.FolderInfo) null;
        }
        return root;
      }

      private void BakeCollections()
      {
        this.itemCollection.Clear();
        this.folderCollection.Clear();
        foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in this.root.GetAllChild())
        {
          if (iinfo.isItemKind)
            this.itemCollection.Add(iinfo.asItemKind.content);
          else
            this.folderCollection.Add(iinfo.asFolderKind.content);
        }
      }

      protected void UpdatePath(List<GUIHelper.HierarchyList<I, F>.IInfo> items)
      {
        List<GUIHelper.HierarchyList<I, F>.IInfo> source = new List<GUIHelper.HierarchyList<I, F>.IInfo>();
        foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in items)
        {
          source.Add(iinfo);
          if (iinfo.isFolderKind)
            source.AddRange((IEnumerable<GUIHelper.HierarchyList<I, F>.IInfo>) iinfo.asFolderKind.GetAllChild());
        }
        source.Distinct<GUIHelper.HierarchyList<I, F>.IInfo>();
        source.Where<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x.parent != null)).ForEach<GUIHelper.HierarchyList<I, F>.IInfo>((Action<GUIHelper.HierarchyList<I, F>.IInfo>) (x => this.SetPath(x, x.parent.fullPath)));
      }

      protected override void ContextClicked()
      {
        if (this.isContextOnItem)
        {
          this.isContextOnItem = false;
        }
        else
        {
          GenericMenu menu = new GenericMenu();
          this.ContextMenu(menu, new List<GUIHelper.HierarchyList<I, F>.IInfo>());
          menu.ShowAsContext();
        }
      }

      protected override void ContextClickedItem(int id)
      {
        this.isContextOnItem = true;
        GenericMenu menu = new GenericMenu();
        List<GUIHelper.HierarchyList<I, F>.IInfo> list = this.GetSelection().Select<int, GUIHelper.HierarchyList<I, F>.IInfo>((Func<int, GUIHelper.HierarchyList<I, F>.IInfo>) (x => this.info.Get<int, GUIHelper.HierarchyList<I, F>.IInfo>(x))).Where<GUIHelper.HierarchyList<I, F>.IInfo>((Func<GUIHelper.HierarchyList<I, F>.IInfo, bool>) (x => x != null)).ToList<GUIHelper.HierarchyList<I, F>.IInfo>();
        this.ContextMenu(menu, list);
        menu.ShowAsContext();
      }

      public abstract void ContextMenu(
        GenericMenu menu,
        List<GUIHelper.HierarchyList<I, F>.IInfo> selected);

      public class ItemInfo : GUIHelper.HierarchyList<I, F>.IInfo
      {
        public I content;

        public ItemInfo(int depth, GUIHelper.HierarchyList<I, F>.FolderInfo parent = null)
        {
          this.item = new TreeViewItem(0, depth, "Item");
          this.parent = parent;
        }
      }

      public class FolderInfo : GUIHelper.HierarchyList<I, F>.IInfo
      {
        public F content;
        public List<GUIHelper.HierarchyList<I, F>.IInfo> items = new List<GUIHelper.HierarchyList<I, F>.IInfo>();

        public FolderInfo(int depth, GUIHelper.HierarchyList<I, F>.FolderInfo parent = null)
        {
          this.item = new TreeViewItem(0, depth, "Folder");
          this.parent = parent;
        }

        public List<GUIHelper.HierarchyList<I, F>.IInfo> GetAllChild()
        {
          List<GUIHelper.HierarchyList<I, F>.IInfo> allChild = new List<GUIHelper.HierarchyList<I, F>.IInfo>();
          foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo in this.items)
          {
            allChild.Add(iinfo);
            if (iinfo is GUIHelper.HierarchyList<I, F>.FolderInfo)
              allChild.AddRange((IEnumerable<GUIHelper.HierarchyList<I, F>.IInfo>) (iinfo as GUIHelper.HierarchyList<I, F>.FolderInfo).GetAllChild());
          }
          return allChild;
        }

        public GUIHelper.HierarchyList<I, F>.IInfo Find(int id)
        {
          foreach (GUIHelper.HierarchyList<I, F>.IInfo iinfo1 in this.items)
          {
            if (iinfo1.item.id == id)
              return iinfo1;
            if (iinfo1 is GUIHelper.HierarchyList<I, F>.FolderInfo)
            {
              GUIHelper.HierarchyList<I, F>.IInfo iinfo2 = (iinfo1 as GUIHelper.HierarchyList<I, F>.FolderInfo).Find(id);
              if (iinfo2 != null)
                return iinfo2;
            }
          }
          return (GUIHelper.HierarchyList<I, F>.IInfo) null;
        }

        public bool IsChildOf(GUIHelper.HierarchyList<I, F>.FolderInfo folder)
        {
          for (GUIHelper.HierarchyList<I, F>.FolderInfo parent = this.parent; parent != null; parent = parent.parent)
          {
            if (parent == folder)
              return true;
          }
          return false;
        }

        public bool IsParentOf(GUIHelper.HierarchyList<I, F>.FolderInfo folder) => folder.IsChildOf(this);
      }

      public abstract class IInfo
      {
        public TreeViewItem item;
        public GUIHelper.HierarchyList<I, F>.FolderInfo parent;

        public string path => this.parent == null ? "" : this.parent.fullPath;

        public string fullPath
        {
          get
          {
            if (this.parent == null)
              return "";
            string path = this.path;
            return path.Length > 0 ? path + "/" + this.name : this.name;
          }
        }

        public string name => this.item.displayName;

        public int index => this.parent != null ? this.parent.items.IndexOf(this) : -1;

        public bool isItemKind => this is GUIHelper.HierarchyList<I, F>.ItemInfo;

        public bool isFolderKind => this is GUIHelper.HierarchyList<I, F>.FolderInfo;

        public GUIHelper.HierarchyList<I, F>.ItemInfo asItemKind => this as GUIHelper.HierarchyList<I, F>.ItemInfo;

        public GUIHelper.HierarchyList<I, F>.FolderInfo asFolderKind => this as GUIHelper.HierarchyList<I, F>.FolderInfo;

        public override string ToString() => (this.isItemKind ? "I:" : "F:") + this.fullPath;
      }
    }

    public abstract class HierarchyList<I> : GUIHelper.HierarchyList<I, TreeFolder>
    {
      private static Texture2D folderIcon;

      public HierarchyList(List<I> collection, List<TreeFolder> folders, TreeViewState state)
        : base(collection, folders, state)
      {
      }

      public override TreeFolder CreateFolder() => new TreeFolder();

      public override void DrawFolder(
        Rect rect,
        GUIHelper.HierarchyList<I, TreeFolder>.FolderInfo info)
      {
        if ((UnityEngine.Object) GUIHelper.HierarchyList<I>.folderIcon == (UnityEngine.Object) null)
          GUIHelper.HierarchyList<I>.folderIcon = EditorIcons.GetBuildInIcon("Folder");
        GUI.DrawTexture(new Rect(rect.x, rect.y, 16f, rect.height), (Texture) GUIHelper.HierarchyList<I>.folderIcon);
        GUI.Label(new Rect(rect.x + 16f, rect.y, rect.width - 16f, rect.height), info.item.displayName);
      }

      public override string GetPath(TreeFolder element) => element.path;

      public override int GetUniqueID(TreeFolder element) => element.GetHashCode();

      public override void SetPath(TreeFolder element, string path) => element.path = path;

      public override string GetName(TreeFolder element) => element.name;

      public override void SetName(TreeFolder element, string name) => element.name = name;
    }

    public abstract class NonHierarchyList<I> : GUIHelper.HierarchyList<I, TreeFolder>
    {
      private static Texture2D folderIcon;

      public NonHierarchyList(List<I> collection, TreeViewState state, string name = null)
        : base(collection, (List<TreeFolder>) null, state, name)
      {
      }

      public override void ContextMenu(
        GenericMenu menu,
        List<GUIHelper.HierarchyList<I, TreeFolder>.IInfo> selected)
      {
        selected = selected.Where<GUIHelper.HierarchyList<I, TreeFolder>.IInfo>((Func<GUIHelper.HierarchyList<I, TreeFolder>.IInfo, bool>) (x => x.isItemKind)).ToList<GUIHelper.HierarchyList<I, TreeFolder>.IInfo>();
        if (selected.Count == 0)
          menu.AddItem(new GUIContent("New Item"), false, (GenericMenu.MenuFunction) (() => this.AddNewItem(this.headFolder, (string) null)));
        else
          menu.AddItem(new GUIContent("Remove"), false, (GenericMenu.MenuFunction) (() => this.Remove(selected.ToArray())));
      }

      public GUIHelper.HierarchyList<I, TreeFolder>.FolderInfo rootFolder => this.AddFolder("");

      public GUIHelper.HierarchyList<I, TreeFolder>.FolderInfo headFolder => !string.IsNullOrEmpty(this.listName) ? this.AddFolder("root") : this.rootFolder;

      public override TreeFolder CreateFolder() => new TreeFolder();

      public override void DrawFolder(
        Rect rect,
        GUIHelper.HierarchyList<I, TreeFolder>.FolderInfo info)
      {
        if ((UnityEngine.Object) GUIHelper.NonHierarchyList<I>.folderIcon == (UnityEngine.Object) null)
          GUIHelper.NonHierarchyList<I>.folderIcon = EditorIcons.GetBuildInIcon("Folder");
        GUI.DrawTexture(new Rect(rect.x, rect.y, 16f, rect.height), (Texture) GUIHelper.NonHierarchyList<I>.folderIcon);
        GUI.Label(new Rect(rect.x + 16f, rect.y, rect.width - 16f, rect.height), this.listName);
      }

      protected override void RowGUI(TreeView.RowGUIArgs args)
      {
        if (string.IsNullOrEmpty(this.listName))
        {
          GUIHelper.HierarchyList<I, TreeFolder>.IInfo iinfo = this.info.Get<int, GUIHelper.HierarchyList<I, TreeFolder>.IInfo>(args.item.id);
          if (iinfo == null || !iinfo.isItemKind)
            return;
          this.DrawItem(args.rowRect, iinfo.asItemKind);
        }
        else
          base.RowGUI(args);
      }

      public override string GetPath(TreeFolder element) => "";

      public override string GetPath(I element) => !string.IsNullOrEmpty(this.listName) ? "root" : "";

      public override string GetName(TreeFolder element) => !string.IsNullOrEmpty(this.listName) ? "root" : "";

      public override string GetName(I element) => element.GetHashCode().ToString();

      public override int GetUniqueID(TreeFolder element) => -2;

      public override void SetPath(TreeFolder element, string path)
      {
      }

      public override void SetPath(I element, string path)
      {
      }
    }

    public class SearchPanel
    {
      private static GUIStyle _searchStyle;
      private static GUIStyle _searchXStyle;
      private static GUIStyle _keyItemStyle;
      private string search;

      private static GUIStyle searchStyle
      {
        get
        {
          if (GUIHelper.SearchPanel._searchStyle == null)
            GUIHelper.SearchPanel._searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
          return GUIHelper.SearchPanel._searchStyle;
        }
      }

      private static GUIStyle searchXStyle
      {
        get
        {
          if (GUIHelper.SearchPanel._searchXStyle == null)
            GUIHelper.SearchPanel._searchXStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
          return GUIHelper.SearchPanel._searchXStyle;
        }
      }

      public static GUIStyle keyItemStyle
      {
        get
        {
          if (GUIHelper.SearchPanel._keyItemStyle == null)
          {
            GUIHelper.SearchPanel._keyItemStyle = new GUIStyle(EditorStyles.label);
            GUIHelper.SearchPanel._keyItemStyle.richText = true;
          }
          return GUIHelper.SearchPanel._keyItemStyle;
        }
      }

      public string value
      {
        get => this.search;
        set => this.search = value;
      }

      public SearchPanel(string search) => this.search = search;

      public void OnGUI(Action<string> onChanged, params GUILayoutOption[] options)
      {
        using (new GUIHelper.Change((Action) (() => onChanged(this.search))))
        {
          this.search = EditorGUILayout.TextField(this.search, GUIHelper.SearchPanel.searchStyle, options);
          if (!GUILayout.Button("", GUIHelper.SearchPanel.searchXStyle))
            return;
          this.search = "";
          EditorGUI.FocusTextInControl("");
        }
      }

      public static string Format(string text, string search)
      {
        int num = text.ToLower().IndexOf(search.ToLower());
        return num < 0 ? text : text.Substring(0, num) + string.Format(Styles.highlightStrongBlue, (object) text.Substring(num, search.Length)) + text.Substring(num + search.Length);
      }
    }
  }
}
