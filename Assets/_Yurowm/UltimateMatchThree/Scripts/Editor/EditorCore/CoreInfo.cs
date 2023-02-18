// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.CoreInfo
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

namespace Yurowm.EditorCore
{
  internal static class CoreInfo
  {
    public const int version = 50;
    private static bool downloading;

    public static void CoreUpdate()
    {
      if (!CoreSettings.IsCoreUpdateEnable)
        return;
      EditorCoroutine.start(CoreInfo.Update());
    }

    private static IEnumerator Update()
    {
      if (!CoreInfo.downloading)
      {
        CoreInfo.downloading = true;
        LogicRequestInstruction request = new LogicRequestInstruction("LinkRequest", string.Format("core_{0}", (object) 27990), (string) null);
        while (request.keepWaiting)
          yield return (object) 0;
        string url = (string) null;
        if (!string.IsNullOrEmpty(request.error))
          Debug.LogError((object) request.error);
        else
          url = request.result;
        if (!string.IsNullOrEmpty(url))
        {
          EditorUtility.DisplayProgressBar("Core Updating", "Progress", 0.0f);
          WWW www = new WWW(url);
          while (!www.isDone)
          {
            EditorUtility.DisplayProgressBar("Core Updating", string.Format("Progress {0:F1}%", (object) (float) ((double) www.progress * 100.0)), www.progress);
            if (!string.IsNullOrEmpty(www.error))
            {
              Debug.LogError((object) www.error);
              ProjectInfo.last_check = new DateTime();
              EditorUtility.ClearProgressBar();
              break;
            }
            yield return (object) 0;
          }
          EditorUtility.ClearProgressBar();
          if (www.isDone)
          {
            string text = www.text;
            Match match = new Regex("^\\<head[^\\>]*\\>.+\\<\\/head\\>").Match(text);
            XDocument xdocument = match.Success ? XDocument.Parse(match.Value) : throw new Exception("Head is not found");
            int sourceIndex = 0;
            byte[] sourceArray = Convert.FromBase64String(text.Substring(match.Length));
            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
            foreach (XElement element in xdocument.Root.Elements((XName) "file"))
            {
              string key = element.Attribute((XName) "name").Value;
              byte[] destinationArray = new byte[long.Parse(element.Attribute((XName) "size").Value)];
              Array.Copy((Array) sourceArray, sourceIndex, (Array) destinationArray, 0, destinationArray.Length);
              dictionary.Add(key, destinationArray);
              sourceIndex += destinationArray.Length;
            }
            List<FileInfo> source = EUtils.SearchFiles(Application.dataPath);
            foreach (KeyValuePair<string, byte[]> keyValuePair in dictionary)
            {
              KeyValuePair<string, byte[]> c = keyValuePair;
              File.WriteAllBytes(source.First<FileInfo>((Func<FileInfo, bool>) (x => x.Name == c.Key)).FullName, c.Value);
            }
            ProjectInfo.last_check = new DateTime();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
          }
          www = (WWW) null;
        }
        CoreInfo.downloading = false;
      }
    }
  }
}
