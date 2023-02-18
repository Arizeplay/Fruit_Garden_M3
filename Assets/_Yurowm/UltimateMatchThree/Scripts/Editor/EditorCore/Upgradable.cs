// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Upgradable
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.EditorCore
{
  internal static class Upgradable
  {
    private static readonly Upgradable.UnityVersion currentVersion = new Upgradable.UnityVersion(Application.unityVersion);
    private static Upgradable.Method loadImage;

    public static void LoadImage(Texture2D texture, byte[] data)
    {
      if (Upgradable.loadImage == null)
      {
        Upgradable.loadImage = new Upgradable.Method(Upgradable.LoadMethod("UnityEngine.ImageConversionModule", "UnityEngine.ImageConversion", nameof (LoadImage), new System.Type[2]
        {
          typeof (Texture2D),
          typeof (byte[])
        }, BindingFlags.Static | BindingFlags.Public), Upgradable.VersionType.New);
        if (Upgradable.loadImage.info == null)
          Upgradable.loadImage = new Upgradable.Method(Upgradable.LoadMethod("UnityEngine", "UnityEngine.ImageConversion", nameof (LoadImage), new System.Type[2]
          {
            typeof (Texture2D),
            typeof (byte[])
          }, BindingFlags.Static | BindingFlags.Public), Upgradable.VersionType.New);
        if (Upgradable.loadImage.info == null)
          Upgradable.loadImage = new Upgradable.Method(Upgradable.LoadMethod("UnityEngine", "UnityEngine.Texture2D", nameof (LoadImage), new System.Type[1]
          {
            typeof (byte[])
          }), Upgradable.VersionType.Old);
      }
      if (Upgradable.loadImage.info == null)
        return;
      switch (Upgradable.loadImage.versionType)
      {
        case Upgradable.VersionType.New:
          Upgradable.loadImage.info.Invoke((object) null, new object[2]
          {
            (object) texture,
            (object) data
          });
          break;
        case Upgradable.VersionType.Old:
          Upgradable.loadImage.info.Invoke((object) texture, new object[1]
          {
            (object) data
          });
          break;
      }
    }

    private static MethodInfo LoadMethod(
      string assemblyName,
      string typeName,
      string methodName,
      System.Type[] parameters = null,
      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
    {
      Assembly assembly;
      try
      {
        assembly = Assembly.Load(assemblyName);
      }
      catch (Exception ex)
      {
        return (MethodInfo) null;
      }
      if (assembly == null)
        return (MethodInfo) null;
      System.Type type = assembly.GetType(typeName);
      if (type == null)
        return (MethodInfo) null;
      foreach (MethodInfo method in type.GetMethods(bindingFlags))
      {
        if (!(method.Name != methodName))
        {
          if (parameters != null)
          {
            ParameterInfo[] parameterInfoArray = method.GetParameters();
            if (parameterInfoArray.Length == parameters.Length)
            {
              for (int index = 0; index < parameterInfoArray.Length; ++index)
              {
                if (parameterInfoArray[index].ParameterType != parameters[index])
                {
                  parameterInfoArray = (ParameterInfo[]) null;
                  break;
                }
              }
              if (parameterInfoArray == null)
                continue;
            }
            else
              continue;
          }
          return method;
        }
      }
      return (MethodInfo) null;
    }

    private enum VersionType
    {
      New,
      Old,
    }

    private class Method
    {
      public readonly MethodInfo info;
      public readonly Upgradable.VersionType versionType;

      public Method(MethodInfo info, Upgradable.VersionType versionType)
      {
        this.info = info;
        this.versionType = versionType;
      }
    }

    private class UnityVersion
    {
      private static readonly Regex format = new Regex("^[\\d\\.]*");
      private int[] numbers;
      private string name;

      public UnityVersion(string name)
      {
        Match match = Upgradable.UnityVersion.format.Match(name);
        if (!match.Success)
          throw new FormatException(name);
        this.numbers = ((IEnumerable<string>) match.Value.Split('.')).Select<string, int>((Func<string, int>) (s => int.Parse(s))).ToArray<int>();
        this.name = string.Join(".", ((IEnumerable<int>) this.numbers).Select<int, string>((Func<int, string>) (x => x.ToString())).ToArray<string>());
      }

      private int GetNumber(int index) => index < 0 || index >= this.numbers.Length ? 0 : this.numbers[index];

      public override bool Equals(object obj) => (object) (obj as Upgradable.UnityVersion) != null && this == obj as Upgradable.UnityVersion;

      public override int GetHashCode() => this.name.GetHashCode();

      public override string ToString() => this.name;

      public static bool operator ==(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        if (versionA == (Upgradable.UnityVersion) null || versionB == (Upgradable.UnityVersion) null)
          throw new NullReferenceException();
        int num = Mathf.Max(versionA.numbers.Length, versionB.numbers.Length);
        for (int index = 0; index < num; ++index)
        {
          if (versionA.GetNumber(index) != versionB.GetNumber(index))
            return false;
        }
        return true;
      }

      public static bool operator !=(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        return !(versionA == versionB);
      }

      public static bool operator >(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        int num = Mathf.Max(versionA.numbers.Length, versionB.numbers.Length);
        for (int index = 0; index < num; ++index)
        {
          if (versionA.GetNumber(index) != versionB.GetNumber(index))
            return versionA.GetNumber(index) > versionB.GetNumber(index);
        }
        return false;
      }

      public static bool operator <(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        return versionB > versionA;
      }

      public static bool operator >=(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        return !(versionA < versionB);
      }

      public static bool operator <=(
        Upgradable.UnityVersion versionA,
        Upgradable.UnityVersion versionB)
      {
        return !(versionA > versionB);
      }
    }
  }
}
