// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.MonoBehaviourAssistant`1
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using UnityEngine;

namespace Yurowm.GameCore
{
  public abstract class MonoBehaviourAssistant<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T _main;

    public static T main => MonoBehaviourAssistant<T>._main;

    public MonoBehaviourAssistant() => MonoBehaviourAssistant<T>._main = this as T;
  }
}
