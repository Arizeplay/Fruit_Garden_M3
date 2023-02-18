// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ItemColorEditor
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelTab("Item Colors", 0)]
  public class ItemColorEditor : MetaEditor
  {
    private const string fileName = "ItemColor.cs";
    private const string defaultCode = "// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    Red = 0,\n    Green = 1,\n    Blue = 2,\n    Yellow = 3,\n    Purple = 4,\n    Orange = 5,\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        {ItemColor.Red, new Color(1.00f, 0.50f, 0.50f, 1.00f)},\n        {ItemColor.Green, new Color(0.50f, 1.00f, 0.60f, 1.00f)},\n        {ItemColor.Blue, new Color(0.40f, 0.80f, 1.00f, 1.00f)},\n        {ItemColor.Yellow, new Color(1.00f, 0.90f, 0.30f, 1.00f)},\n        {ItemColor.Purple, new Color(0.80f, 0.40f, 1.00f, 1.00f)},\n        {ItemColor.Orange, new Color(1.00f, 0.70f, 0.00f, 1.00f)}\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}";
    private const string codeFormat = "// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    *IDS*\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        *COLORS*\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}";
    private FileInfo scriptFile;
    private string code;
    private Dictionary<string, ItemColorEditor.ColorInfo> ids = new Dictionary<string, ItemColorEditor.ColorInfo>();
    private Regex id_format = new Regex("^[a-zA-Z]{1,32}$");
    private ItemColorEditor.ColorInfo newInfo = new ItemColorEditor.ColorInfo();
    private bool changed;

    public override bool Initialize()
    {
      this.scriptFile = EUtils.ProjectFiles(Application.dataPath).FirstOrDefault<FileInfo>((Func<FileInfo, bool>) (x => x.Name == "ItemColor.cs"));
      DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, "Generated Scriptes"));
      if (this.scriptFile == null || !this.scriptFile.Exists)
      {
        if (!directoryInfo.Exists)
          directoryInfo.Create();
        this.scriptFile = new FileInfo(Path.Combine(directoryInfo.FullName, "ItemColor.cs"));
        using (StreamWriter text = this.scriptFile.CreateText())
          text.Write("// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    Red = 0,\n    Green = 1,\n    Blue = 2,\n    Yellow = 3,\n    Purple = 4,\n    Orange = 5,\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        {ItemColor.Red, new Color(1.00f, 0.50f, 0.50f, 1.00f)},\n        {ItemColor.Green, new Color(0.50f, 1.00f, 0.60f, 1.00f)},\n        {ItemColor.Blue, new Color(0.40f, 0.80f, 1.00f, 1.00f)},\n        {ItemColor.Yellow, new Color(1.00f, 0.90f, 0.30f, 1.00f)},\n        {ItemColor.Purple, new Color(0.80f, 0.40f, 1.00f, 1.00f)},\n        {ItemColor.Orange, new Color(1.00f, 0.70f, 0.00f, 1.00f)}\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}");
        this.code = "// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    Red = 0,\n    Green = 1,\n    Blue = 2,\n    Yellow = 3,\n    Purple = 4,\n    Orange = 5,\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        {ItemColor.Red, new Color(1.00f, 0.50f, 0.50f, 1.00f)},\n        {ItemColor.Green, new Color(0.50f, 1.00f, 0.60f, 1.00f)},\n        {ItemColor.Blue, new Color(0.40f, 0.80f, 1.00f, 1.00f)},\n        {ItemColor.Yellow, new Color(1.00f, 0.90f, 0.30f, 1.00f)},\n        {ItemColor.Purple, new Color(0.80f, 0.40f, 1.00f, 1.00f)},\n        {ItemColor.Orange, new Color(1.00f, 0.70f, 0.00f, 1.00f)}\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}";
      }
      else
        this.code = File.ReadAllText(this.scriptFile.FullName);
      if (!this.scriptFile.Exists)
        return false;
      Regex regex = new Regex("^\\/\\/.*\\s(using\\s+[A-Za-z\\.0-1]+;\\s)*[\\s\\w]*\\{(?<ids>(?:\\s*\\w+\\s*=\\s*\\d+,?)*)\\s*\\}[\\s\\w]*\\{[\\s*\\w<,>=()]*\\{(?<colors>(?:\\s*\\{[\\w.,\\s]*\\([\\d.f,\\s]*\\)\\},?)*)\\s*\\}");
      if (!regex.IsMatch(this.code))
        this.code = "// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    Red = 0,\n    Green = 1,\n    Blue = 2,\n    Yellow = 3,\n    Purple = 4,\n    Orange = 5,\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        {ItemColor.Red, new Color(1.00f, 0.50f, 0.50f, 1.00f)},\n        {ItemColor.Green, new Color(0.50f, 1.00f, 0.60f, 1.00f)},\n        {ItemColor.Blue, new Color(0.40f, 0.80f, 1.00f, 1.00f)},\n        {ItemColor.Yellow, new Color(1.00f, 0.90f, 0.30f, 1.00f)},\n        {ItemColor.Purple, new Color(0.80f, 0.40f, 1.00f, 1.00f)},\n        {ItemColor.Orange, new Color(1.00f, 0.70f, 0.00f, 1.00f)}\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}";
      Match match1 = regex.Match(this.code);
      if (!match1.Success)
        return false;
      this.ids.Clear();
      foreach (Match match2 in new Regex("(?<id>\\w+)\\s*=\\s*(?<num>\\d+)").Matches(match1.Groups["ids"].Value))
      {
        ItemColorEditor.ColorInfo colorInfo = new ItemColorEditor.ColorInfo();
        colorInfo.id = match2.Groups["id"].Value;
        colorInfo.num = int.Parse(match2.Groups["num"].Value);
        this.ids.Set<string, ItemColorEditor.ColorInfo>(colorInfo.id, colorInfo);
      }
      foreach (Match match3 in new Regex("\\{ItemColor\\.(?<id>[\\w]+),\\snew\\sColor\\((?<r>\\d\\.\\d\\d)f,\\s(?<g>\\d\\.\\d\\d)f,\\s(?<b>\\d\\.\\d\\d)f,\\s(?<a>\\d\\.\\d\\d)f\\)\\}").Matches(match1.Groups["colors"].Value))
      {
        string key = match3.Groups["id"].Value;
        if (this.ids.ContainsKey(key))
        {
          this.ids[key].color = new UnityEngine.Color(float.Parse(match3.Groups["r"].Value, CultureInfo.InvariantCulture), float.Parse(match3.Groups["g"].Value, CultureInfo.InvariantCulture), float.Parse(match3.Groups["b"].Value, CultureInfo.InvariantCulture), float.Parse(match3.Groups["a"].Value, CultureInfo.InvariantCulture));
        }
      }
      this.ids.Where<KeyValuePair<string, ItemColorEditor.ColorInfo>>((Func<KeyValuePair<string, ItemColorEditor.ColorInfo>, bool>) (x => !x.Value.color.HasValue)).ForEach<KeyValuePair<string, ItemColorEditor.ColorInfo>>((Action<KeyValuePair<string, ItemColorEditor.ColorInfo>>) (x => x.Value.color = new UnityEngine.Color?(UnityEngine.Color.white)));
      this.newInfo.num = this.ids.Values.Where<ItemColorEditor.ColorInfo>((Func<ItemColorEditor.ColorInfo, bool>) (x => x.num < 100)).Max<ItemColorEditor.ColorInfo>((Func<ItemColorEditor.ColorInfo, int>) (x => x.num)) + 1;
      this.newInfo.id = "";
      return true;
    }

    public override void OnGUI()
    {
      using (new GUIHelper.Lock(EditorApplication.isCompiling || EditorApplication.isPlaying))
      {
        using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
        {
          using (new GUIHelper.Lock(!this.changed))
          {
            if (GUILayout.Button("Save", GUILayout.Width(75f)))
            {
              this.changed = false;
              List<ItemColorEditor.ColorInfo> list = this.ids.Values.Where<ItemColorEditor.ColorInfo>((Func<ItemColorEditor.ColorInfo, bool>) (x => x.num >= 0 && x.num < 100)).ToList<ItemColorEditor.ColorInfo>();
              this.code = "// Auto Generated script. Use \"Berry Panel > Item Colors\" to edit it.\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\r\n\r\npublic enum ItemColor {\n    *IDS*\n    Unknown = 100,\n    Uncolored = 101,\n    Universal = 102\n}\n\npublic static class RealColors {\r\n\r\n    static Dictionary<ItemColor, Color> colors = new Dictionary<ItemColor, Color>() {\n        *COLORS*\n    };\r\n\r\n    public static Color Get(ItemColor color) {\n        try {\r\n            if (color.IsPhysicalColor())\n                return colors[color];\n        } catch (System.Exception) {\r\n        }\n        return Color.white;\n    }\n}".Replace("*IDS*", string.Join("\r\n\t", list.Select<ItemColorEditor.ColorInfo, string>((Func<ItemColorEditor.ColorInfo, string>) (x => string.Format("{0} = {1},", (object) x.id, (object) x.num))).ToArray<string>())).Replace("*COLORS*", string.Join(",\r\n\t\t", list.Select<ItemColorEditor.ColorInfo, string>((Func<ItemColorEditor.ColorInfo, string>) (x => string.Format("{{ItemColor.{0}, new Color({1:F2}f, {2:F2}f, {3:F2}f, 1.00f)}}", (object) x.id, (object) x.color.Value.r, (object) x.color.Value.g, (object) x.color.Value.b))).ToArray<string>()));
              using (StreamWriter text = this.scriptFile.CreateText())
                text.Write(this.code);
              AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
            if (GUILayout.Button("Revert", GUILayout.Width(75f)))
            {
              this.changed = false;
              this.Initialize();
              AssetDatabase.Refresh(ImportAssetOptions.Default);
              return;
            }
          }
          if (GUILayout.Button("Default", GUILayout.Width(80f)))
          {
            this.changed = false;
            this.scriptFile.Delete();
            this.Initialize();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            return;
          }
        }
        using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
        {
          GUILayout.Label("ID", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100f));
          GUILayout.Label("Num", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(50f));
          GUILayout.Label("Color", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100f));
        }
        foreach (ItemColorEditor.ColorInfo colorInfo in this.ids.Values)
        {
          using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
          {
            using (new GUIHelper.Lock(colorInfo.num <= 5 || colorInfo.num >= 100))
            {
              if (GUILayout.Button("X", GUILayout.Width(20f)))
              {
                this.newInfo.id = colorInfo.id;
                this.newInfo.num = colorInfo.num;
                this.ids.Remove(colorInfo.id);
                this.changed = true;
                break;
              }
            }
            GUILayout.Label(colorInfo.id, GUILayout.Width(100f));
            GUILayout.Label(colorInfo.num.ToString(), GUILayout.Width(50f));
            if (colorInfo.num >= 0)
            {
              if (colorInfo.num < 100)
              {
                using (new GUIHelper.Change((Action) (() => this.changed = true)))
                  colorInfo.color = new UnityEngine.Color?(this.ColorField(colorInfo.color.HasValue ? colorInfo.color.Value : UnityEngine.Color.white));
              }
            }
          }
        }
        GUILayout.Label("New:", GUILayout.Width(50f));
        using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
        {
          this.newInfo.id = EditorGUILayout.TextField(this.newInfo.id, GUILayout.Width(100f));
          this.newInfo.num = EditorGUILayout.IntField(this.newInfo.num, GUILayout.Width(50f));
        }
        if (this.newInfo.id == "")
          return;
        if (!this.id_format.IsMatch(this.newInfo.id))
          EditorGUILayout.HelpBox("Wrong ID format. It must contain only letters (a-z and A-Z). Length between 1 and 32.", MessageType.Error);
        else if (this.ids.ContainsKey(this.newInfo.id))
          EditorGUILayout.HelpBox("This ID is already existing", MessageType.Error);
        else if (this.newInfo.num <= 5 || this.newInfo.num >= 100)
          EditorGUILayout.HelpBox("Number must be between 6 and 99", MessageType.Error);
        else if (this.ids.Contains<KeyValuePair<string, ItemColorEditor.ColorInfo>>((Func<KeyValuePair<string, ItemColorEditor.ColorInfo>, bool>) (x => x.Value.num == this.newInfo.num)))
        {
          EditorGUILayout.HelpBox("This Number already existing", MessageType.Error);
        }
        else
        {
          if (!GUILayout.Button("Add", GUILayout.Width(150f)))
            return;
          this.ids.Add(this.newInfo.id, this.newInfo);
          this.newInfo = new ItemColorEditor.ColorInfo();
          this.ids = this.ids.OrderBy<KeyValuePair<string, ItemColorEditor.ColorInfo>, int>((Func<KeyValuePair<string, ItemColorEditor.ColorInfo>, int>) (x => x.Value.num)).ToDictionary<string, ItemColorEditor.ColorInfo>();
          this.newInfo.num = this.ids.Values.Where<ItemColorEditor.ColorInfo>((Func<ItemColorEditor.ColorInfo, bool>) (x => x.num < 100)).Max<ItemColorEditor.ColorInfo>((Func<ItemColorEditor.ColorInfo, int>) (x => x.num)) + 1;
          GUI.FocusControl("");
          this.changed = true;
        }
      }
    }

    private UnityEngine.Color ColorField(UnityEngine.Color color)
    {
      try
      {
        return EditorGUILayout.ColorField(color, GUILayout.Width(100f));
      }
      catch (ExitGUIException ex)
      {
        return color;
      }
    }

    private class ColorInfo
    {
      public string id = "";
      public int num;
      public UnityEngine.Color? color;
    }
  }
}
