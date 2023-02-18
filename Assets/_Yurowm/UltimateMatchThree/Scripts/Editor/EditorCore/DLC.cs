// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.DLC
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("DLC", 0)]
  public class DLC : MetaEditor
  {
    private FileInfo cache;
    private List<DLC.DLCItem> dlcList = new List<DLC.DLCItem>();
    private bool refreshing;
    private DLC.DLCTag filter = (DLC.DLCTag) 255;
    private GUIHelper.Scroll scroll = new GUIHelper.Scroll(new GUILayoutOption[2]
    {
      GUILayout.ExpandHeight(true),
      GUILayout.ExpandWidth(true)
    });

    public override bool Initialize()
    {
      if (CoreSettings.IsCoreUpdateEnable)
      {
        this.cache = new FileInfo(Path.Combine(new DirectoryInfo(Application.temporaryCachePath).FullName, "dlc.cache"));
        if (!this.cache.Exists || (DateTime.Now - this.cache.LastWriteTime).TotalHours > 1.0)
          EditorCoroutine.start(this.Refresh());
        else
          this.Load(File.ReadAllText(this.cache.FullName));
      }
      return true;
    }

    private IEnumerator Refresh()
    {
      this.refreshing = true;
      LogicRequestInstruction instruction = new LogicRequestInstruction("DLCListRequest", "{0},{1}".FormatText((object) 27990, (object) ProjectInfo.coreVersion), (string) null);
      while (instruction.keepWaiting)
        yield return (object) 0;
      if (instruction.error.IsNullOrEmpty())
        this.Load(instruction.result);
      else
        Debug.LogError((object) instruction.error);
      MetaEditor<MonoBehaviour>.Repaint();
      this.refreshing = false;
    }

    private void Load(string raw)
    {
      this.dlcList = new List<DLC.DLCItem>();
      foreach (XElement element in XDocument.Parse(raw).Root.Elements((XName) "dlc"))
        this.dlcList.Add(DLC.DLCItem.Deserialize(element));
      this.dlcList.Sort((Comparison<DLC.DLCItem>) ((a, b) => a.name.CompareTo(b.name)));
      File.WriteAllText(this.cache.FullName, raw);
    }

    public override void OnGUI()
    {
      if (!CoreSettings.IsCoreUpdateEnable)
        EditorGUILayout.HelpBox("The Core updates is disabled. Fixer and DLC features isn't work anymore.", MessageType.Info);
      else if (!this.refreshing)
      {
        if (this.dlcList.Count == 0)
        {
          GUILayout.Label("No DLC found", Styles.centeredMiniLabel, GUILayout.ExpandWidth(true));
        }
        else
        {
          using (new GUIHelper.Horizontal(EditorStyles.toolbar, new GUILayoutOption[1]
          {
            GUILayout.ExpandWidth(true)
          }))
          {
            this.filter = (DLC.DLCTag) EditorGUILayout.EnumMaskPopup("Filter", (Enum) this.filter, EditorStyles.toolbarPopup, GUILayout.MaxWidth(300f));
            GUILayout.FlexibleSpace();
          }
          using (this.scroll.Start())
          {
            foreach (DLC.DLCItem dlc in this.dlcList)
              dlc.Draw(this.filter);
          }
        }
      }
      else
        GUILayout.Label("Downloading...", Styles.centeredMiniLabel, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
    }

    [System.Flags]
    private enum DLCTag
    {
      SDK = 1,
      ContentDLC = 2,
      Pack = 4,
    }

    private class DLCItem
    {
      public string name;
      private string displayedName;
      public string description;
      public string link;
      public int fixerLevel;
      public int versionCode = -1;
      private long size;
      private string info;
      private DLC.DLCTag tags;
      private Texture2D icon;
      private static Texture2D defaultIcon = (Texture2D) null;
      private string iconName = "";
      private static Dictionary<DLC.DLCTag, string> tagColors = new Dictionary<DLC.DLCTag, string>()
      {
        {
          DLC.DLCTag.SDK,
          "53d0ff"
        },
        {
          DLC.DLCTag.ContentDLC,
          "c053fe"
        },
        {
          DLC.DLCTag.Pack,
          "83fe53"
        }
      };
      private bool downloading;

      public static DLC.DLCItem Deserialize(XElement xml)
      {
        DLC.DLCItem dlcItem = new DLC.DLCItem();
        dlcItem.name = xml.Attribute((XName) "name").Value;
        dlcItem.description = xml.Attribute((XName) "description").Value;
        dlcItem.link = xml.Attribute((XName) "link").Value;
        dlcItem.size = long.Parse(xml.Attribute((XName) "size").Value);
        XAttribute xattribute1 = xml.Attribute((XName) "fixerLevel");
        if (xattribute1 != null)
          dlcItem.fixerLevel = int.Parse(xattribute1.Value);
        XAttribute xattribute2 = xml.Attribute((XName) "versionCode");
        if (xattribute2 != null)
          dlcItem.versionCode = int.Parse(xattribute2.Value);
        XAttribute xattribute3 = xml.Attribute((XName) "icon");
        if (xattribute3 != null)
          dlcItem.iconName = xattribute3.Value;
        if (dlcItem.iconName.IsNullOrEmpty())
        {
          if ((UnityEngine.Object) DLC.DLCItem.defaultIcon == (UnityEngine.Object) null)
            DLC.DLCItem.defaultIcon = EditorIcons.GetBuildInIcon("DLCDefaultIcon");
          dlcItem.icon = DLC.DLCItem.defaultIcon;
        }
        else
          EditorCoroutine.start(dlcItem.DownloadIcon());
        XAttribute xattribute4 = xml.Attribute((XName) "tags");
        byte result;
        if (xattribute4 != null && byte.TryParse(xattribute4.Value, out result))
          dlcItem.tags = (DLC.DLCTag) result;
        dlcItem.displayedName = "<b>{0}</b>".FormatText((object) dlcItem.name.Replace('_', ' '));
        if (dlcItem.tags != (DLC.DLCTag) 0)
        {
          List<string> stringList1 = new List<string>();
          foreach (DLC.DLCTag key in Enum.GetValues(typeof (DLC.DLCTag)))
          {
            if ((key & dlcItem.tags) != (DLC.DLCTag) 0)
            {
              List<string> stringList2 = stringList1;
              string str;
              if (!DLC.DLCItem.tagColors.ContainsKey(key))
                str = key.ToString();
              else
                str = "<color=#{0}ff>{1}</color>".FormatText((object) DLC.DLCItem.tagColors[key], (object) key);
              stringList2.Add(str);
            }
          }
          if (stringList1.Count > 0)
            dlcItem.displayedName += " <size=10>({0})</size>".FormatText((object) string.Join(", ", stringList1.ToArray()));
        }
        dlcItem.info = "File Size: {0}".FormatText((object) EUtils.BytesToString(dlcItem.size));
        return dlcItem;
      }

      private IEnumerator DownloadIcon()
      {
        this.downloading = true;
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.temporaryCachePath, "BPBrowserCache"));
        if (!directoryInfo.Exists)
          directoryInfo.Create();
        FileInfo cacheFile = new FileInfo(Path.Combine(directoryInfo.FullName, this.iconName + ".jpg"));
        if (!cacheFile.Exists || cacheFile.Length == 0L || (cacheFile.LastWriteTime - DateTime.Now).TotalDays > 3.0)
        {
          WWW www = new WWW("http://jellymobile.net/jellymobile.net/yurowm/bpbrowser/images/{0}.jpg".FormatText((object) this.iconName));
          while (!www.isDone && www.error.IsNullOrEmpty())
            yield return (object) null;
          if (!www.error.IsNullOrEmpty())
          {
            this.icon = DLC.DLCItem.defaultIcon;
            this.downloading = false;
            yield break;
          }
          else
          {
            File.WriteAllBytes(cacheFile.FullName, www.bytes);
            www = (WWW) null;
          }
        }
        byte[] data = File.ReadAllBytes(cacheFile.FullName);
        if (data != null)
        {
          this.icon = new Texture2D(1, 1);
          Upgradable.LoadImage(this.icon, data);
        }
        this.downloading = false;
      }

      public void Draw(DLC.DLCTag filter)
      {
        if ((filter & this.tags) == (DLC.DLCTag) 0)
          return;
        using (new GUIHelper.Horizontal(Styles.levelArea, new GUILayoutOption[1]
        {
          GUILayout.MaxWidth(500f)
        }))
        {
          if ((UnityEngine.Object) DLC.DLCItem.defaultIcon == (UnityEngine.Object) null)
            DLC.DLCItem.defaultIcon = EditorIcons.GetBuildInIcon("DLCDefaultIcon");
          if (!this.downloading && (UnityEngine.Object) this.icon == (UnityEngine.Object) null && !this.iconName.IsNullOrEmpty())
            EditorCoroutine.start(this.DownloadIcon());
          GUI.DrawTexture(EditorGUILayout.GetControlRect(false, GUILayout.Height(64f), GUILayout.Width(64f)), (UnityEngine.Object) this.icon == (UnityEngine.Object) null || this.downloading ? (Texture) DLC.DLCItem.defaultIcon : (Texture) this.icon);
          using (new GUIHelper.Vertical(new GUILayoutOption[0]))
          {
            GUILayout.Label(this.displayedName, Styles.whiteRichLabel);
            EditorGUILayout.HelpBox(this.description, MessageType.None);
            GUILayout.Label(this.info, Styles.whiteLabel);
            if (Fixer.state.level < this.fixerLevel)
            {
              using (new GUIHelper.Color(UnityEngine.Color.red))
                GUILayout.Label("Fixer level must be equal or higher then " + (object) this.fixerLevel, Styles.whiteLabel);
            }
            else if (this.versionCode > 0 && 2 > this.versionCode)
            {
              using (new GUIHelper.Color(UnityEngine.Color.yellow))
                GUILayout.Label("Your version of the project already contains this content" + (object) this.fixerLevel, Styles.whiteLabel);
            }
            else
            {
              if (!GUILayout.Button("Install", GUILayout.Width(100f)))
                return;
              EditorCoroutine.start(this.Install());
            }
          }
        }
      }

      private IEnumerator Install()
      {
        WWW www = new WWW(this.link);
        DelayedAccess repaintAccess = new DelayedAccess(0.06f);
        while (!www.isDone)
        {
          if (repaintAccess.GetAccess())
          {
            if (EditorUtility.DisplayCancelableProgressBar(this.name + " Installation", "Downloading... {0}/{1} ({2:F2}%)".FormatText((object) EUtils.BytesToString((long) Mathf.CeilToInt((float) this.size * www.progress)), (object) EUtils.BytesToString((long) Mathf.CeilToInt((float) this.size)), (object) (float) ((double) www.progress * 100.0)), www.progress))
            {
              www.Dispose();
              EditorUtility.ClearProgressBar();
              yield break;
            }
            else
              yield return (object) 0;
          }
        }
        if (www.isDone)
        {
          string text = www.text;
          Match match = new Regex("^\\<head[^\\>]*\\>.+\\<\\/head\\>").Match(text);
          XDocument xdocument = match.Success ? XDocument.Parse(match.Value) : throw new Exception("Head is not found");
          int position = 0;
          byte[] rawData = Convert.FromBase64String(text.Substring(match.Length));
          Dictionary<string, byte[]> coreData = new Dictionary<string, byte[]>();
          List<XElement> fileXml = xdocument.Root.Elements((XName) "file").ToList<XElement>();
          for (int i = 0; i < fileXml.Count; ++i)
          {
            string key = fileXml[i].Attribute((XName) "name").Value;
            byte[] destinationArray = new byte[long.Parse(fileXml[i].Attribute((XName) "size").Value)];
            Array.Copy((Array) rawData, position, (Array) destinationArray, 0, destinationArray.Length);
            coreData.Add(key, destinationArray);
            position += destinationArray.Length;
            if (repaintAccess.GetAccess())
            {
              yield return (object) 0;
              EditorUtility.DisplayProgressBar(this.name + " Installation", "Installing ({0}/{1})".FormatText((object) i, (object) fileXml.Count), www.progress);
            }
          }
          int p = 0;
          foreach (KeyValuePair<string, byte[]> keyValuePair in coreData)
          {
            FileInfo fileInfo = new FileInfo(Path.Combine(Application.dataPath, keyValuePair.Key).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
            if (!fileInfo.Directory.Exists)
              fileInfo.Directory.Create();
            File.WriteAllBytes(fileInfo.FullName, keyValuePair.Value);
            ++p;
            if (repaintAccess.GetAccess())
            {
              yield return (object) 0;
              EditorUtility.DisplayProgressBar(this.name + " Installation", "Installing ({0}/{1})".FormatText((object) p, (object) coreData.Count), www.progress);
            }
          }
          yield return (object) 0;
          EditorUtility.ClearProgressBar();
          AssetDatabase.Refresh();
          rawData = (byte[]) null;
          coreData = (Dictionary<string, byte[]>) null;
          fileXml = (List<XElement>) null;
        }
      }
    }
  }
}
