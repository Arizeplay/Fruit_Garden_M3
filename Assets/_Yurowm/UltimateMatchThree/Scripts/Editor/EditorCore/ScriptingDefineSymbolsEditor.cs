// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ScriptingDefineSymbolsEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("S.D.Symbols", "ScriptingTabIcon", 0)]
  public class ScriptingDefineSymbolsEditor : MetaEditor
  {
    private List<IScriptingDefineSymbol> symbols;
    private ScriptingDefineSymbolsEditor.SymbolsTree tree;
    private GUIHelper.LayoutSplitter splitter;
    private GUIHelper.Scroll scroll;
    private UnityEngine.Color greenButton = new UnityEngine.Color(0.5f, 1f, 0.5f);
    private UnityEngine.Color redButton = new UnityEngine.Color(1f, 0.5f, 0.5f);

    public override bool Initialize()
    {
      string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Trim();
      List<string> stringList;
      if (!string.IsNullOrEmpty(str))
        stringList = ((IEnumerable<string>) str.Split(';')).Select<string, string>((Func<string, string>) (x => x.Trim().ToUpper())).ToList<string>();
      else
        stringList = new List<string>();
      List<string> defines = stringList;
      defines.Sort();
      this.symbols = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, System.Type>((Func<Assembly, IEnumerable<System.Type>>) (x => (IEnumerable<System.Type>) x.GetTypes())).Where<System.Type>((Func<System.Type, bool>) (x => !x.IsAbstract && typeof (IScriptingDefineSymbol).IsAssignableFrom(x))).Select<System.Type, IScriptingDefineSymbol>((Func<System.Type, IScriptingDefineSymbol>) (x => (IScriptingDefineSymbol) Activator.CreateInstance(x))).ToList<IScriptingDefineSymbol>();
      this.symbols.ForEach((Action<IScriptingDefineSymbol>) (s => s.enable = defines.Contains<string>((Func<string, bool>) (x => x == s.GetSybmol().ToUpper()))));
      this.symbols.Sort((Comparison<IScriptingDefineSymbol>) ((x, y) => x.GetSybmol().CompareTo(y.GetSybmol())));
      this.tree = new ScriptingDefineSymbolsEditor.SymbolsTree(defines.Select<string, ScriptingDefineSymbolsEditor.Symbol>((Func<string, ScriptingDefineSymbolsEditor.Symbol>) (x => new ScriptingDefineSymbolsEditor.Symbol(x))).ToList<ScriptingDefineSymbolsEditor.Symbol>(), (string) null);
      this.splitter = new GUIHelper.LayoutSplitter(OrientationLine.Horizontal, OrientationLine.Vertical, new float[1]
      {
        200f
      });
      this.splitter.drawCursor = (Action<Rect>) (x => GUI.Box(x, "", Styles.separator));
      this.scroll = new GUIHelper.Scroll(new GUILayoutOption[1]
      {
        GUILayout.ExpandHeight(true)
      });
      return true;
    }

    public override void OnGUI()
    {
      using (new GUIHelper.Lock(EditorApplication.isCompiling))
      {
        if (GUILayout.Button("Save", GUILayout.Width(100f)))
        {
          PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join("; ", this.tree.itemCollection.Select<ScriptingDefineSymbolsEditor.Symbol, string>((Func<ScriptingDefineSymbolsEditor.Symbol, string>) (x => x.symbol)).ToArray<string>()));
          if ((bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
            MonoBehaviourAssistant<Yurowm.GameCore.Content>.main.SDSymbols = this.tree.itemCollection.Select<ScriptingDefineSymbolsEditor.Symbol, string>((Func<ScriptingDefineSymbolsEditor.Symbol, string>) (x => x.symbol)).ToArray<string>();
          AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        using (this.splitter.Start(true, this.symbols.Count > 0))
        {
          if (this.splitter.Area(Styles.area))
          {
            GUILayout.Label("Symbols", Styles.title);
            this.tree.OnGUI(GUILayout.ExpandHeight(true));
          }
          if (!this.splitter.Area())
            return;
          using (this.scroll.Start())
          {
            foreach (IScriptingDefineSymbol symbol1 in this.symbols)
            {
              IScriptingDefineSymbol symbol = symbol1;
              using (new GUIHelper.Vertical(Styles.levelArea, new GUILayoutOption[1]
              {
                GUILayout.ExpandWidth(true)
              }))
              {
                GUILayout.Label(symbol.GetSybmol(), Styles.whiteBoldLabel);
                GUILayout.Box(symbol.GetDescription(), Styles.textAreaLineBreaked, GUILayout.ExpandWidth(true));
                using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
                {
                  string berryLink = symbol.GetBerryLink();
                  if (symbol.enable)
                  {
                    using (new GUIHelper.Color(this.redButton))
                    {
                      if (GUILayout.Button("Disable", GUILayout.Width(100f)))
                      {
                        symbol.enable = false;
                        this.tree.itemCollection.RemoveAll((Predicate<ScriptingDefineSymbolsEditor.Symbol>) (x => x.symbol == symbol.GetSybmol().ToUpper()));
                        this.tree.Reload();
                      }
                    }
                  }
                  else
                  {
                    using (new GUIHelper.Color(this.greenButton))
                    {
                      if (GUILayout.Button("Enable", GUILayout.Width(100f)))
                      {
                        symbol.enable = true;
                        this.tree.itemCollection.Add(new ScriptingDefineSymbolsEditor.Symbol(symbol.GetSybmol().ToUpper()));
                        this.tree.Reload();
                      }
                    }
                  }
                  if (!berryLink.IsNullOrEmpty())
                  {
                    if (GUILayout.Button("More Info", GUILayout.Width(100f)))
                      ManualWindow.OpenPage(berryLink);
                  }
                }
              }
            }
            GUILayout.FlexibleSpace();
          }
        }
      }
    }

    private class Symbol
    {
      public string symbol;
      public int id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

      public Symbol(string symbol)
      {
        this.symbol = symbol;
        if (symbol.IsNullOrEmpty())
          return;
        this.id = symbol.GetHashCode();
      }
    }

    private class SymbolsTree : GUIHelper.NonHierarchyList<ScriptingDefineSymbolsEditor.Symbol>
    {
      public SymbolsTree(
        List<ScriptingDefineSymbolsEditor.Symbol> collection,
        string name)
        : base(collection, new TreeViewState(), name)
      {
      }

      public override ScriptingDefineSymbolsEditor.Symbol CreateItem() => new ScriptingDefineSymbolsEditor.Symbol((string) null);

      public override void DrawItem(
        Rect rect,
        GUIHelper.HierarchyList<ScriptingDefineSymbolsEditor.Symbol, TreeFolder>.ItemInfo info)
      {
        rect.xMin += 16f;
        if (info.content.symbol.IsNullOrEmpty())
          return;
        GUI.Label(rect, info.content.symbol);
      }

      public override int GetUniqueID(ScriptingDefineSymbolsEditor.Symbol element) => element.id;

      public override bool ObjectToItem(
        UnityEngine.Object o,
        out GUIHelper.HierarchyList<ScriptingDefineSymbolsEditor.Symbol, TreeFolder>.IInfo result)
      {
        result = (GUIHelper.HierarchyList<ScriptingDefineSymbolsEditor.Symbol, TreeFolder>.IInfo) null;
        return false;
      }

      public override string GetName(ScriptingDefineSymbolsEditor.Symbol element) => element.symbol;

      public override void SetName(ScriptingDefineSymbolsEditor.Symbol element, string name) => element.symbol = name;

      protected override void RenameEnded(TreeView.RenameEndedArgs args)
      {
        args.newName = args.newName.ToUpper();
        base.RenameEnded(args);
      }
    }
  }
}
