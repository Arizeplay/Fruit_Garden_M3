// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryBrowerCache
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  internal static class BerryBrowerCache
  {
    public static DirectoryInfo mainFolder = (DirectoryInfo) null;
    public static Dictionary<string, Texture> images = new Dictionary<string, Texture>();

    public static void Initialize()
    {
      if (BerryBrowerCache.mainFolder != null)
        return;
      BerryBrowerCache.mainFolder = new DirectoryInfo(Path.Combine(Application.temporaryCachePath, "BPBrowserCache"));
      if (BerryBrowerCache.mainFolder.Exists)
        return;
      BerryBrowerCache.mainFolder.Create();
    }

    public static void Clear()
    {
      ((IEnumerable<FileInfo>) BerryBrowerCache.mainFolder.GetFiles()).ForEach<FileInfo>((Action<FileInfo>) (f => f.Delete()));
      BerryBrowerCache.images.Clear();
    }
  }
}
