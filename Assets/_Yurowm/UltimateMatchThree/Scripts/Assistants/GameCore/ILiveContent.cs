// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.ILiveContent
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yurowm.GameCore
{
  public abstract class ILiveContent : MonoBehaviour
  {
    private static List<ILiveContent> all = new List<ILiveContent>();
    internal GameObject _original;

    public GameObject original => this._original;

    public void Kill()
    {
      ILiveContent.all.Remove(this);
      this.OnKill();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public bool EqualContent(ILiveContent content)
    {
      if (!(bool) (UnityEngine.Object) content)
        return false;
      if ((UnityEngine.Object) content == (UnityEngine.Object) this)
        return true;
      if ((bool) (UnityEngine.Object) content._original && (bool) (UnityEngine.Object) this._original)
        return (UnityEngine.Object) content._original == (UnityEngine.Object) this._original;
      return (UnityEngine.Object) content._original == (UnityEngine.Object) this.gameObject || (UnityEngine.Object) this._original == (UnityEngine.Object) content.gameObject;
    }

    private static bool Search<T>(
      ILiveContent content,
      System.Type type,
      T original,
      Func<T, bool> condition)
      where T : ILiveContent
    {
      if ((bool) (UnityEngine.Object) original && !((UnityEngine.Object) content._original == (UnityEngine.Object) original.gameObject) || !type.IsAssignableFrom(content.GetType()))
        return false;
      return condition == null || condition((T) content);
    }

    public static List<T> GetAll<T>(Func<T, bool> condition = null, T original = null) where T : ILiveContent
    {
      System.Type type = typeof (T);
      return ILiveContent.all.Where<ILiveContent>((Func<ILiveContent, bool>) (x => ILiveContent.Search<T>(x, type, original, condition))).Cast<T>().ToList<T>();
    }

    public static int Count<T>(Func<T, bool> condition = null, T original = null) where T : ILiveContent
    {
      System.Type type = typeof (T);
      return ILiveContent.all.Count<ILiveContent>((Func<ILiveContent, bool>) (x => ILiveContent.Search<T>(x, type, original, condition)));
    }

    public static bool Contains<T>(Func<T, bool> condition = null, T original = null) where T : ILiveContent
    {
      System.Type type = typeof (T);
      return ILiveContent.all.Contains<ILiveContent>((Func<ILiveContent, bool>) (x => ILiveContent.Search<T>(x, type, original, condition)));
    }

    public static void KillEverything()
    {
      new List<ILiveContent>((IEnumerable<ILiveContent>) ILiveContent.all).Where<ILiveContent>((Func<ILiveContent, bool>) (x => (UnityEngine.Object) x != (UnityEngine.Object) null)).ForEach<ILiveContent>((Action<ILiveContent>) (x => x.Kill()));
      ILiveContent.all.Clear();
    }

    internal static void Add(ILiveContent clone)
    {
      clone.Initialize();
      ILiveContent.all.Add(clone);
    }

    public virtual void Initialize()
    {
    }

    public virtual void OnKill()
    {
    }
  }
}
