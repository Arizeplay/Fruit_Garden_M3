// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.SDSMask
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yurowm.GameCore
{
  public class SDSMask : MonoBehaviour
  {
    public string[] requirements = new string[0];

    private void Awake()
    {
      if (!(bool) (Object) MonoBehaviourAssistant<Content>.main)
        Debug.LogError((object) "Content manager is not found");
      foreach (string requirement in this.requirements)
      {
        if (!((IEnumerable<string>) MonoBehaviourAssistant<Content>.main.SDSymbols).Contains<string>(requirement))
        {
          Object.Destroy((Object) this.gameObject);
          break;
        }
      }
    }
  }
}
