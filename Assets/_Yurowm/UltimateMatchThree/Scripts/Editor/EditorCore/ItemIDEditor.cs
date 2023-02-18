// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ItemIDEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelTab("Item IDs", 0)]
  public class ItemIDEditor : MetaEditor
  {
    private const string fileName = "ItemID.cs";
    private const string defaultCode = "// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    coin = 0,\n    life = 1,\n    lifeslot = 2,\n    extramoves = 10,\n    paint = 11,\n    surprise = 12,\n    ring = 13,\n    needle = 100,\n    flower = 101,\n    shuffle = 102,\n    happylife = 200\n}";
    private const string codeFormat = "// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    *IDS*\n}";
    private FileInfo scriptFile;
    private string code;
    private Dictionary<string, int> ids = new Dictionary<string, int>();
    private Regex id_format = new Regex("^[a-zA-Z]{1,32}$");
    private string _id = "";
    private int _num;
    private bool changed;

    public override bool Initialize()
    {
      this.scriptFile = EUtils.ProjectFiles(Application.dataPath).FirstOrDefault<FileInfo>((Func<FileInfo, bool>) (x => x.Name == "ItemID.cs"));
      DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, "Generated Scriptes"));
      if (this.scriptFile == null || !this.scriptFile.Exists)
      {
        if (!directoryInfo.Exists)
          directoryInfo.Create();
        this.scriptFile = new FileInfo(Path.Combine(directoryInfo.FullName, "ItemID.cs"));
        using (StreamWriter text = this.scriptFile.CreateText())
          text.Write("// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    coin = 0,\n    life = 1,\n    lifeslot = 2,\n    extramoves = 10,\n    paint = 11,\n    surprise = 12,\n    ring = 13,\n    needle = 100,\n    flower = 101,\n    shuffle = 102,\n    happylife = 200\n}");
        this.code = "// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    coin = 0,\n    life = 1,\n    lifeslot = 2,\n    extramoves = 10,\n    paint = 11,\n    surprise = 12,\n    ring = 13,\n    needle = 100,\n    flower = 101,\n    shuffle = 102,\n    happylife = 200\n}";
      }
      else
        this.code = File.ReadAllText(this.scriptFile.FullName);
      if (!this.scriptFile.Exists)
        return false;
      Regex regex = new Regex("^\\/\\/.*\\s*public enum ItemID \\{(?<ids>(?:\\s*\\w+\\s*=\\s*\\d+,?)*)\\s*\\}");
      if (!regex.IsMatch(this.code))
        this.code = "// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    coin = 0,\n    life = 1,\n    lifeslot = 2,\n    extramoves = 10,\n    paint = 11,\n    surprise = 12,\n    ring = 13,\n    needle = 100,\n    flower = 101,\n    shuffle = 102,\n    happylife = 200\n}";
      Match match1 = regex.Match(this.code);
      if (!match1.Success)
        return false;
      this.ids.Clear();
      foreach (Match match2 in new Regex("(?<id>\\w+)\\s*=\\s*(?<num>\\d+)").Matches(match1.Groups["ids"].Value))
        this.ids.Set<string, int>(match2.Groups["id"].Value, int.Parse(match2.Groups["num"].Value));
      this._num = Mathf.Max(10, this.ids.Values.Max() + 1);
      this._id = "";
      return true;
    }

    public override void OnGUI()
    {
      using (new GUIHelper.Lock(EditorApplication.isCompiling || EditorApplication.isPlaying))
      {
        using (new GUIHelper.Lock(!this.changed))
        {
          using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
          {
            if (GUILayout.Button("Save", GUILayout.Width(75f)))
            {
              this.changed = false;
              this.code = "// Auto Generated script. Use \"Berry Panel > Item IDs\" to edit it.\npublic enum ItemID {\n    *IDS*\n}".Replace("*IDS*", string.Join(",\r\n\t", this.ids.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (x => string.Format("{0} = {1}", (object) x.Key, (object) x.Value))).ToArray<string>()));
              using (StreamWriter text = this.scriptFile.CreateText())
                text.Write(this.code);
              AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
            if (GUILayout.Button("Revert", GUILayout.Width(75f)))
            {
              this.changed = false;
              this.Initialize();
              return;
            }
          }
        }
        using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
        {
          GUILayout.Label("ID", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100f));
          GUILayout.Label("Num", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(50f));
        }
        foreach (string key in this.ids.Keys)
        {
          using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
          {
            GUILayout.Label(key, GUILayout.Width(100f));
            GUILayout.Label(this.ids[key].ToString(), GUILayout.Width(50f));
            if (this.ids[key] >= 10)
            {
              if (GUILayout.Button("X", GUILayout.Width(20f)))
              {
                this._id = key;
                this._num = this.ids[key];
                this.ids.Remove(key);
                this.changed = true;
                break;
              }
            }
          }
        }
        GUILayout.Label("New:", GUILayout.Width(50f));
        using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
        {
          this._id = EditorGUILayout.TextField(this._id, GUILayout.Width(100f));
          this._num = EditorGUILayout.IntField(this._num, GUILayout.Width(50f));
        }
        if (this._id == "")
          return;
        if (!this.id_format.IsMatch(this._id))
          EditorGUILayout.HelpBox("Wrong ID format. It must contain only letters (a-z and A-Z). Length between 1 and 32.", MessageType.Error);
        else if (this.ids.ContainsKey(this._id))
          EditorGUILayout.HelpBox("This ID is already existing", MessageType.Error);
        else if (this._num < 10 || this._num >= 1000)
          EditorGUILayout.HelpBox("Number must be between 10 and 999", MessageType.Error);
        else if (this.ids.ContainsValue(this._num))
        {
          EditorGUILayout.HelpBox("This Number already existing", MessageType.Error);
        }
        else
        {
          if (!GUILayout.Button("Add", GUILayout.Width(150f)))
            return;
          this.ids.Add(this._id, this._num);
          this.ids = this.ids.OrderBy<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>) (x => x.Value)).ToDictionary<string, int>();
          this._id = "";
          this._num = this.ids.Values.Max() + 1;
          GUI.FocusControl("");
          this.changed = true;
        }
      }
    }
  }
}
