// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.Content
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Yurowm.GameCore
{
  public class Content : MonoBehaviourAssistant<Content>
  {
    [SerializeField]
    private string key = "";
    public List<Content.Item> cItems = new List<Content.Item>();
    public List<TreeFolder> categories = new List<TreeFolder>();
    private Dictionary<string, GameObject> content = new Dictionary<string, GameObject>();
    public string[] SDSymbols;
    [NonSerialized]
    private bool initialized;

    [RuntimeInitializeOnLoadMethod]
    internal static void StartRuntime()
    {
      if (Application.isEditor)
        return;
      MonoBehaviourAssistant<Content>.main.StartCoroutine(MonoBehaviourAssistant<Content>.main.Informer());
    }

    private IEnumerator Informer()
    {
      int[] touchPass = new int[25]
      {
        1,
        2,
        3,
        4,
        3,
        4,
        3,
        2,
        3,
        2,
        1,
        2,
        3,
        2,
        3,
        4,
        3,
        2,
        1,
        2,
        3,
        4,
        3,
        2,
        1
      };
label_1:
      while (true)
      {
        int index = 0;
        while (Input.touchCount == 0)
          yield return (object) 0;
        while (true)
        {
          if (Input.touchCount != 0)
          {
            if (index < touchPass.Length - 1)
            {
              if (Input.touchCount == touchPass[index + 1])
                ++index;
              else if (Input.touchCount == touchPass[index])
                yield return (object) 0;
              else
                break;
            }
            else
              goto label_11;
          }
          else
            goto label_1;
        }
        yield return (object) 0;
      }
label_11:
      GameObject gameObject = new GameObject("Infomer");
      Canvas canvas = gameObject.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvas.sortingOrder = 99999;
      CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
      canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      canvasScaler.referenceResolution = Vector2.one * 600f;
      canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
      Image image = new GameObject("BG").AddComponent<Image>();
      image.rectTransform.SetParent(canvas.transform);
      image.rectTransform.anchorMin = Vector2.zero;
      image.rectTransform.anchorMax = Vector2.one;
      image.rectTransform.offsetMin = Vector2.zero;
      image.rectTransform.offsetMax = Vector2.zero;
      image.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
      UnityEngine.UI.Text text = new GameObject("Text").AddComponent<UnityEngine.UI.Text>();
      text.rectTransform.SetParent(canvas.transform);
      text.rectTransform.anchorMin = Vector2.zero;
      text.rectTransform.anchorMax = Vector2.one;
      text.rectTransform.offsetMin = Vector2.zero;
      text.rectTransform.offsetMax = Vector2.zero;
      text.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
      text.color = Color.white;
      text.alignment = TextAnchor.MiddleCenter;
      text.text = string.Format("Access Key: {0}\nCheckSum: {1}\nBundle ID: {2}", (object) new Regex("\\d{4}-\\d{3}").Replace(this.key, "XXXX-XXX"), (object) this.key.CheckSum(), (object) Application.identifier);
    }

    private void Awake() => this.Initialize();

    public bool IsInitialized => this.initialized;

    public void Initialize()
    {
      if (!Application.isEditor && this.key.IsNullOrEmpty())
        this.cItems.Clear();
      else
        this.cItems.RemoveAll((Predicate<Content.Item>) (x => (UnityEngine.Object) x.item == (UnityEngine.Object) null));
      this.content = this.cItems.ToDictionary<Content.Item, string, GameObject>((Func<Content.Item, string>) (x => x.item.name), (Func<Content.Item, GameObject>) (x => x.item));
      this.initialized = true;
    }

    public static T GetItem<T>(string key) where T : Component
    {
      GameObject gameObject = Content.GetItem(key);
      return (bool) (UnityEngine.Object) gameObject ? gameObject.GetComponent<T>() : default (T);
    }

    public static T GetItem<T>() where T : Component
    {
      T prefab = Content.GetPrefab<T>((Func<T, bool>) (p => (bool) (UnityEngine.Object) p.GetComponent<T>()));
      if (!(bool) (UnityEngine.Object) prefab)
        return default (T);
      GameObject gameObject = Content.GetItem(prefab.name);
      return (bool) (UnityEngine.Object) gameObject ? gameObject.GetComponent<T>() : default (T);
    }

    public static T GetPrefab<T>(string key) where T : Component
    {
      GameObject prefab = Content.GetPrefab(key);
      return (bool) (UnityEngine.Object) prefab ? prefab.GetComponent<T>() : default (T);
    }

    public static GameObject GetItem(string key) => MonoBehaviourAssistant<Content>.main.content.ContainsKey(key) ? UnityEngine.Object.Instantiate<GameObject>(MonoBehaviourAssistant<Content>.main.content[key]) : (GameObject) null;

    public static GameObject GetPrefab(string key)
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Content>.main)
        return (GameObject) null;
      if (!MonoBehaviourAssistant<Content>.main.initialized)
        MonoBehaviourAssistant<Content>.main.Initialize();
      return MonoBehaviourAssistant<Content>.main.content.ContainsKey(key) ? MonoBehaviourAssistant<Content>.main.content[key] : (GameObject) null;
    }

    public static L Emit<L>(L reference = null) where L : ILiveContent
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Content>.main)
        return default (L);
      if (!MonoBehaviourAssistant<Content>.main.initialized)
        MonoBehaviourAssistant<Content>.main.Initialize();
      GameObject original = (GameObject) null;
      if (!(bool) (UnityEngine.Object) reference)
      {
        L prefab = Content.GetPrefab<L>();
        if ((bool) (UnityEngine.Object) prefab)
          original = prefab.gameObject;
      }
      else if ((bool) (UnityEngine.Object) reference._original)
      {
        original = reference._original;
      }
      else
      {
        if (!MonoBehaviourAssistant<Content>.main.content.Values.Contains<GameObject>((Func<GameObject, bool>) (x => (UnityEngine.Object) x.gameObject == (UnityEngine.Object) ((L) reference).gameObject)))
          throw new Exception("This is a wrong reference. Use only original references from Content manager or instances which was created by original reference.");
        original = reference.gameObject;
      }
      L component = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<L>();
      component.name = original.name;
      component._original = original;
      ILiveContent.Add((ILiveContent) component);
      return component;
    }

    public static ILiveContent Emit(string name) => Content.Emit<ILiveContent>(name);

    public static L Emit<L>(string name) where L : ILiveContent
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Content>.main)
        return default (L);
      if (!MonoBehaviourAssistant<Content>.main.initialized)
        MonoBehaviourAssistant<Content>.main.Initialize();
      if (!MonoBehaviourAssistant<Content>.main.content.Keys.Contains<string>((Func<string, bool>) (x => x == name)))
        throw new Exception("This is a wrong name. Content manager dosn't contain anything like this.");
      L clone = Content.GetItem<L>(name);
      clone._original = Content.GetPrefab(name);
      clone.name = name;
      ILiveContent.Add((ILiveContent) clone);
      return clone;
    }

    public static ILiveContent Emit(System.Type refType, string name)
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Content>.main)
        return (ILiveContent) null;
      if (!MonoBehaviourAssistant<Content>.main.initialized)
        MonoBehaviourAssistant<Content>.main.Initialize();
      if (name != null && !MonoBehaviourAssistant<Content>.main.content.Keys.Contains<string>((Func<string, bool>) (x => x == name)))
        throw new Exception("This is a wrong name. Content manager dosn't contain anything like this.");
      if (!typeof (ILiveContent).IsAssignableFrom(refType))
        throw new Exception("The ref type must be assignable from ILiveContent");
      GameObject original = MonoBehaviourAssistant<Content>.main.content.First<KeyValuePair<string, GameObject>>((Func<KeyValuePair<string, GameObject>, bool>) (x => (name == null || x.Key == name) && (bool) (UnityEngine.Object) x.Value.GetComponent(refType))).Value;
      ILiveContent component = (ILiveContent) UnityEngine.Object.Instantiate<GameObject>(original).GetComponent(refType);
      component.name = name;
      component._original = original;
      ILiveContent.Add(component);
      return component;
    }

    public static T GetItem<T>(string key, Vector3 position) where T : Component
    {
      GameObject gameObject = Content.GetItem(key);
      gameObject.transform.position = position;
      return gameObject.GetComponent<T>();
    }

    public static GameObject GetItem(string key, Vector3 position)
    {
      GameObject gameObject = Content.GetItem(key);
      gameObject.transform.position = position;
      return gameObject;
    }

    public static GameObject GetItem(string key, Vector3 position, Quaternion rotation)
    {
      GameObject gameObject = Content.GetItem(key, position);
      gameObject.transform.rotation = rotation;
      return gameObject;
    }

    public static List<T> GetPrefabList<T>(Func<T, bool> condition = null) where T : Component
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Content>.main)
        return new List<T>();
      if (!MonoBehaviourAssistant<Content>.main.initialized)
        MonoBehaviourAssistant<Content>.main.Initialize();
      List<T> list = MonoBehaviourAssistant<Content>.main.cItems.Select<Content.Item, T>((Func<Content.Item, T>) (x => x.item.GetComponent<T>())).Where<T>((Func<T, bool>) (x => (UnityEngine.Object) x != (UnityEngine.Object) null)).ToList<T>();
      if (condition != null)
        list.RemoveAll((Predicate<T>) (x => !condition(x)));
      return list;
    }

    public static T GetPrefab<T>(Func<T, bool> condition = null) where T : Component => Content.GetPrefabList<T>(condition).FirstOrDefault<T>();

    [Serializable]
    public class Item
    {
      public GameObject item;
      public string path = "";

      public Item(GameObject item) => this.item = item;
    }
  }
}
