// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.EUtils
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  public static class EUtils
  {
    public static void DrawMixedProperty<T>(
      IEnumerable<int2> selected,
      Func<int2, bool> mask,
      Func<int2, T> getValue,
      Action<int2, T> setValue,
      Func<int2, T, T> drawSingle,
      Func<Action<T>, bool> drawMixed,
      Action drawEmpty = null)
    {
      bool flag1 = false;
      bool flag2 = false;
      T value = default (T);
      int2 int2_1 = (int2) null;
      foreach (int2 int2_2 in selected)
      {
        if (mask(int2_2))
        {
          if (!flag2)
          {
            value = getValue(int2_2);
            int2_1 = int2_2;
            flag2 = true;
          }
          else
          {
            int2_1 = (int2) null;
            T obj = getValue(int2_2);
            if (!value.Equals((object) obj))
            {
              flag1 = true;
              break;
            }
          }
        }
      }
      if (!flag2)
      {
        if (drawEmpty == null)
          return;
        drawEmpty();
      }
      else
      {
        if (flag1)
        {
          EditorGUI.showMixedValue = true;
          Action<T> action = (Action<T>) (t => value = t);
          if (drawMixed(action))
            flag1 = false;
          EditorGUI.showMixedValue = false;
        }
        else
          value = drawSingle(int2_1, value);
        if (flag1)
          return;
        foreach (int2 int2_3 in selected)
        {
          if (mask(int2_3))
            setValue(int2_3, value);
        }
      }
    }

    public static List<FileInfo> SearchFiles(string directoryPath)
    {
      List<FileInfo> fileInfoList = new List<FileInfo>();
      DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
      if (directoryInfo.Exists)
      {
        fileInfoList.AddRange((IEnumerable<FileInfo>) ((IEnumerable<FileInfo>) directoryInfo.GetFiles()).ToList<FileInfo>());
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
          fileInfoList.AddRange((IEnumerable<FileInfo>) EUtils.SearchFiles(directory.FullName));
      }
      return fileInfoList;
    }

    public static string BytesToString(long byteCount)
    {
      string[] strArray = new string[7]
      {
        "B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB"
      };
      if (byteCount == 0L)
        return "0" + strArray[0];
      long num1;
      int int32 = Convert.ToInt32(Math.Floor(Math.Log((double) (num1 = Math.Abs(byteCount)), 1024.0)));
      double num2 = Math.Round((double) num1 / Math.Pow(1024.0, (double) int32), 1);
      return ((double) Math.Sign(byteCount) * num2).ToString() + strArray[int32];
    }

    public static IEnumerable<FileInfo> ProjectFiles(DirectoryInfo directory)
    {
      FileInfo[] fileInfoArray = directory.GetFiles();
      int index;
      for (index = 0; index < fileInfoArray.Length; ++index)
        yield return fileInfoArray[index];
      fileInfoArray = (FileInfo[]) null;
      DirectoryInfo[] directoryInfoArray = directory.GetDirectories();
      for (index = 0; index < directoryInfoArray.Length; ++index)
      {
        foreach (FileInfo projectFile in EUtils.ProjectFiles(directoryInfoArray[index]))
          yield return projectFile;
      }
      directoryInfoArray = (DirectoryInfo[]) null;
    }

    public static IEnumerable<FileInfo> ProjectFiles(string directory) => EUtils.ProjectFiles(new DirectoryInfo(directory));

    public static string CombinePath(params string[] paths)
    {
      if (paths == null)
        throw new ArgumentNullException(nameof (paths));
      string path1 = paths.Length != 0 ? paths[0] : throw new IndexOutOfRangeException(nameof (paths));
      for (int index = 1; index < paths.Length; ++index)
        path1 = Path.Combine(path1, paths[index]);
      return path1;
    }
  }
}
