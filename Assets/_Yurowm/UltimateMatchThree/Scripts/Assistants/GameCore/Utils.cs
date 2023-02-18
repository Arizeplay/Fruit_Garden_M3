// Decompiled with JetBrains decompiler
// Type: Yurowm.GameCore.Utils
// Assembly: GameCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 17029E20-0750-4BC6-A04D-8DF9126A1E7D
// Assembly location: D:\Desktop\GameCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Yurowm.GameCore
{
  public static class Utils
  {
    public static readonly List<Side> allSides;
    public static readonly Side[] straightSides;
    public static readonly Side[] slantedSides;
    private const float sin45 = 0.7071f;
    private static Regex addSpacesRegex = new Regex("(?<=.)(?=[A-Z])");

    static Utils()
    {
      Utils.allSides = new List<Side>()
      {
        Side.Right,
        Side.TopRight,
        Side.Top,
        Side.TopLeft,
        Side.Left,
        Side.BottomLeft,
        Side.Bottom,
        Side.BottomRight
      };
      Utils.straightSides = new Side[4]
      {
        Side.Right,
        Side.Top,
        Side.Left,
        Side.Bottom
      };
      Utils.slantedSides = new Side[4]
      {
        Side.TopRight,
        Side.TopLeft,
        Side.BottomLeft,
        Side.BottomRight
      };
    }

    public static Side RotateSide(this Side side, int steps) => (Side) Mathf.CeilToInt(Mathf.Repeat((float) (side + steps), (float) Utils.allSides.Count));

    public static Side MirrorSide(this Side side) => (Side) Mathf.CeilToInt(Mathf.Repeat((float) (side + 4), (float) Utils.allSides.Count));

    public static bool IsNotNull(this Side side) => side != Side.Null;

    public static bool IsStraight(this Side side)
    {
      int num = (int) side;
      return num >= 0 && num % 2 == 0;
    }

    public static int X(this Side s)
    {
      switch (s)
      {
        case Side.Right:
        case Side.TopRight:
        case Side.BottomRight:
          return 1;
        case Side.Top:
        case Side.Bottom:
          return 0;
        case Side.TopLeft:
        case Side.Left:
        case Side.BottomLeft:
          return -1;
        default:
          return 0;
      }
    }

    public static int Y(this Side s)
    {
      switch (s)
      {
        case Side.Right:
        case Side.Left:
          return 0;
        case Side.TopRight:
        case Side.Top:
        case Side.TopLeft:
          return 1;
        case Side.BottomLeft:
        case Side.Bottom:
        case Side.BottomRight:
          return -1;
        default:
          return 0;
      }
    }

    public static float Cos(this Side s)
    {
      switch (s)
      {
        case Side.Right:
          return 1f;
        case Side.TopRight:
        case Side.BottomRight:
          return 0.7071f;
        case Side.Top:
        case Side.Bottom:
          return 0.0f;
        case Side.TopLeft:
        case Side.BottomLeft:
          return -0.7071f;
        case Side.Left:
          return -1f;
        default:
          return 0.0f;
      }
    }

    public static float Sin(this Side s)
    {
      switch (s)
      {
        case Side.Right:
        case Side.Left:
          return 0.0f;
        case Side.TopRight:
        case Side.TopLeft:
          return 0.7071f;
        case Side.Top:
          return 1f;
        case Side.BottomLeft:
        case Side.BottomRight:
          return -0.7071f;
        case Side.Bottom:
          return -1f;
        default:
          return 0.0f;
      }
    }

    public static int2 ToInt2(this Side s)
    {
      switch (s)
      {
        case Side.Right:
          return new int2(1, 0);
        case Side.TopRight:
          return new int2(1, 1);
        case Side.Top:
          return new int2(0, 1);
        case Side.TopLeft:
          return new int2(-1, 1);
        case Side.Left:
          return new int2(-1, 0);
        case Side.BottomLeft:
          return new int2(-1, -1);
        case Side.Bottom:
          return new int2(0, -1);
        case Side.BottomRight:
          return new int2(1, -1);
        default:
          return new int2(0, 0);
      }
    }

    public static Side Horizontal(this Side s)
    {
      switch (s)
      {
        case Side.Right:
        case Side.TopRight:
        case Side.BottomRight:
          return Side.Right;
        case Side.TopLeft:
        case Side.Left:
        case Side.BottomLeft:
          return Side.Left;
        default:
          return Side.Null;
      }
    }

    public static Side Vertical(this Side s)
    {
      switch (s)
      {
        case Side.TopRight:
        case Side.Top:
        case Side.TopLeft:
          return Side.Top;
        case Side.BottomLeft:
        case Side.Bottom:
        case Side.BottomRight:
          return Side.Bottom;
        default:
          return Side.Null;
      }
    }

    public static float ToAngle(this Side s) => s.IsNotNull() ? (float) ((int) s * 45) : 0.0f;

    public static bool DistanceIsMoreThen(this Vector2 vector, float maxDistance) => !vector.DistanceIsLessThen(maxDistance);

    public static bool DistanceIsLessThen(this Vector2 vector, float maxDistance) => (double) Mathf.Abs(vector.x) <= (double) maxDistance && (double) Mathf.Abs(vector.y) <= (double) maxDistance && (double) vector.x * (double) vector.x + (double) vector.y * (double) vector.y <= (double) maxDistance * (double) maxDistance;

    public static void Set<K, V>(this IDictionary<K, V> dictionary, K key, V value)
    {
      if (dictionary.ContainsKey(key))
        dictionary[key] = value;
      else
        dictionary.Add(key, value);
    }

    public static V Get<K, V>(this IDictionary<K, V> dictionary, K key) => dictionary.ContainsKey(key) ? dictionary[key] : default (V);

    public static V GetAndAdd<K, V>(this IDictionary<K, V> dictionary, K key)
    {
      if (dictionary.ContainsKey(key))
        return dictionary[key];
      V instance = Activator.CreateInstance<V>();
      dictionary.Add(key, instance);
      return instance;
    }

    public static Dictionary<N, M> Unsort<N, M>(
      this IDictionary<N, M> dictionary,
      URandom random = null)
    {
      if (dictionary == null)
        return (Dictionary<N, M>) null;
      int[] numArray = new int[dictionary.Count];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = index;
      for (int maxExclusive = numArray.Length - 1; maxExclusive > 0; --maxExclusive)
      {
        int index = random == null ? UnityEngine.Random.Range(0, maxExclusive) : random.Range(0, maxExclusive - 1, (string) null);
        numArray[index] = numArray[index] + numArray[maxExclusive];
        numArray[maxExclusive] = numArray[index] - numArray[maxExclusive];
        numArray[index] = numArray[index] - numArray[maxExclusive];
      }
      Dictionary<N, M> dictionary1 = new Dictionary<N, M>();
      for (int index = 0; index < numArray.Length; ++index)
        dictionary1.Add(dictionary.Keys.ElementAt<N>(numArray[index]), dictionary.Values.ElementAt<M>(numArray[index]));
      return dictionary1;
    }

    public static Dictionary<N, M> RemoveAll<N, M>(
      this IDictionary<N, M> dictionary,
      Func<KeyValuePair<N, M>, bool> condition)
    {
      if (dictionary == null)
        return (Dictionary<N, M>) null;
      Dictionary<N, M> dictionary1 = new Dictionary<N, M>();
      foreach (KeyValuePair<N, M> keyValuePair in (IEnumerable<KeyValuePair<N, M>>) dictionary)
      {
        if (!condition(keyValuePair))
          dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return dictionary1;
    }

    public static void AddPairs<N, M>(this IDictionary<N, M> original, IDictionary<N, M> source)
    {
      foreach (N key in (IEnumerable<N>) source.Keys)
        original[key] = source[key];
    }

    public static T GetRandom<T>(this ICollection<T> collection) => collection == null || collection.Count == 0 ? default (T) : collection.ElementAtOrDefault<T>(UnityEngine.Random.Range(0, collection.Count));

    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
      if (collection == null)
        return default (T);
      T[] array = collection.ToArray<T>();
      return array.Length == 0 ? default (T) : array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static List<T> GetRandom<T>(this ICollection<T> collection, int count)
    {
      if (collection == null || collection.Count == 0 || count <= 0)
        return new List<T>();
      if (count == 1)
        return new List<T>()
        {
          Utils.GetRandom<T>(collection)
        };
      if (count == collection.Count)
        return new List<T>((IEnumerable<T>) collection);
      bool[] flagArray = new bool[collection.Count];
      int num1 = 0;
      while (count != num1)
      {
        int index = UnityEngine.Random.Range(0, collection.Count);
        if (!flagArray[index])
        {
          flagArray[index] = true;
          int num2 = index + 1;
        }
      }
      List<T> random = new List<T>(count);
      for (int index = 0; index < flagArray.Length; ++index)
      {
        if (flagArray[index])
          random.Add(collection.ElementAt<T>(index));
      }
      return random;
    }

    public static List<T> GetRandom<T>(this IEnumerable<T> collection, int count)
    {
      if (collection == null)
        return new List<T>();
      if (count != 1)
        return Utils.GetRandom<T>((ICollection<T>) collection.ToArray<T>(), count);
      return new List<T>() { collection.GetRandom<T>() };
    }

    public static T GetRandom<T>(this IEnumerable<T> collection, URandom random, string key = null)
    {
      if (collection == null)
        return default (T);
      T[] array = collection.ToArray<T>();
      return array.Length == 0 ? default (T) : array[random.Range(0, array.Length - 1, key)];
    }

    public static ICollection<T> Unsort<T>(
      this ICollection<T> collection,
      URandom random = null)
    {
      if (collection == null)
        return (ICollection<T>) null;
      int[] numArray = new int[collection.Count];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = index;
      for (int maxExclusive = numArray.Length - 1; maxExclusive > 0; --maxExclusive)
      {
        int index = random == null ? UnityEngine.Random.Range(0, maxExclusive) : random.Range(0, maxExclusive - 1, (string) null);
        numArray[index] = numArray[index] + numArray[maxExclusive];
        numArray[maxExclusive] = numArray[index] - numArray[maxExclusive];
        numArray[index] = numArray[index] - numArray[maxExclusive];
      }
      List<T> objList = new List<T>();
      for (int index = 0; index < numArray.Length; ++index)
        objList.Add(collection.ElementAt<T>(numArray[index]));
      return (ICollection<T>) objList;
    }

    public static int Count<T>(this ICollection<T> collection, Func<T, bool> filter)
    {
      if (collection == null)
        return 0;
      int num = 0;
      for (int index = 0; index < collection.Count; ++index)
      {
        if (filter(collection.ElementAt<T>(index)))
          ++num;
      }
      return num;
    }

    public static T GetMin<T>(this IEnumerable<T> collection, Func<T, int> filter)
    {
      if (collection == null)
        return default (T);
      int num1 = int.MaxValue;
      T min = default (T);
      foreach (T obj in collection)
      {
        int num2 = filter(obj);
        if (num2 < num1)
        {
          num1 = num2;
          min = obj;
        }
      }
      return min;
    }

    public static T GetMin<T>(this IEnumerable<T> collection, Func<T, float> filter)
    {
      if (collection == null)
        return default (T);
      float num1 = float.MaxValue;
      T min = default (T);
      foreach (T obj in collection)
      {
        float num2 = filter(obj);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          min = obj;
        }
      }
      return min;
    }

    public static T GetMax<T>(this IEnumerable<T> collection, Func<T, int> filter)
    {
      if (collection == null)
        return default (T);
      int num1 = int.MinValue;
      T max = default (T);
      foreach (T obj in collection)
      {
        int num2 = filter(obj);
        if (num2 > num1)
        {
          num1 = num2;
          max = obj;
        }
      }
      return max;
    }

    public static T GetMax<T>(this IEnumerable<T> collection, Func<T, float> filter)
    {
      if (collection == null)
        return default (T);
      float num1 = (float) int.MinValue;
      T max = default (T);
      foreach (T obj in collection)
      {
        float num2 = filter(obj);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          max = obj;
        }
      }
      return max;
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> function)
    {
      if (collection == null)
        return;
      foreach (T obj in collection)
        function(obj);
    }

    public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> function)
    {
      foreach (T obj in source)
      {
        if (function(obj))
          return true;
      }
      return false;
    }

    public static T Get<T>(this ICollection<T> collection, int index)
    {
      if (collection == null)
        throw new NullReferenceException("Collection is null");
      return collection.Count == 0 || index < 0 || collection.Count - 1 < index ? default (T) : collection.ElementAt<T>(index);
    }

    public static ICollection<T> Swap<T>(
      this ICollection<T> collection,
      int from,
      int to)
    {
      if (collection == null)
        throw new NullReferenceException("Collection is null");
      if (from < 0 || from >= collection.Count || to < 0 || to >= collection.Count)
        throw new IndexOutOfRangeException();
      List<T> objList = new List<T>();
      for (int index = 0; index < collection.Count; ++index)
      {
        if (index == from)
          objList.Add(collection.ElementAt<T>(to));
        else if (index == to)
          objList.Add(collection.ElementAt<T>(from));
        else
          objList.Add(collection.ElementAt<T>(index));
      }
      return (ICollection<T>) objList;
    }

    public static Dictionary<N, M> ToDictionary<N, M>(
      this IEnumerable<KeyValuePair<N, M>> collection)
    {
      Dictionary<N, M> dictionary = new Dictionary<N, M>();
      foreach (KeyValuePair<N, M> keyValuePair in collection)
        dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      return dictionary;
    }

    public static string Join(this IEnumerable<string> values, string separator)
    {
      string str1 = "";
      int num = 0;
      foreach (string str2 in values)
      {
        if (num++ > 0)
          str1 += separator;
        str1 += str2;
      }
      return str1;
    }

    public static bool IsNullOrEmpty(this ICollection collection) => collection == null || collection.Count == 0;

    public static List<T> ToList<T>(this IEnumerator<T> enumerator)
    {
      List<T> list = new List<T>();
      while (enumerator.MoveNext())
        list.Add(enumerator.Current);
      try
      {
        enumerator.Reset();
      }
      catch
      {
      }
      return list;
    }

    public static List<T> ToList<T>(this Func<IEnumerator<T>> enumeratorFunc)
    {
      List<T> list = new List<T>();
      IEnumerator<T> enumerator = enumeratorFunc();
      while (enumerator.MoveNext())
        list.Add(enumerator.Current);
      return list;
    }

    public static IEnumerator<T> Collect<T>(IEnumerator enumerator)
    {
      while (enumerator.MoveNext())
      {
        if (enumerator.Current is T)
          yield return (T) enumerator.Current;
        else if (enumerator.Current is IEnumerator)
        {
          IEnumerator subKeys = (IEnumerator) Utils.Collect<T>(enumerator.Current as IEnumerator);
          while (subKeys.MoveNext())
            yield return (T) subKeys.Current;
          subKeys = (IEnumerator) null;
        }
      }
    }

    public static int IndexOf<T>(this T[] array, T value)
    {
      if (array == null || array.Length == 0)
        throw new NullReferenceException("Array is null or empty");
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index].Equals((object) value))
          return index;
      }
      return -1;
    }

    public static Vector2 To2D(this Vector3 original, Asix2 plane = Asix2.XY, bool inverse = false)
    {
      switch (plane)
      {
        case Asix2.XY:
          return !inverse ? new Vector2(original.x, original.y) : new Vector2(original.y, original.x);
        case Asix2.XZ:
          return !inverse ? new Vector2(original.x, original.z) : new Vector2(original.z, original.x);
        case Asix2.YZ:
          return !inverse ? new Vector2(original.y, original.z) : new Vector2(original.z, original.y);
        default:
          return Vector2.zero;
      }
    }

    public static Vector3 To3D(this Vector2 original, float z = 0.0f, Asix2 plane = Asix2.XY, bool inverse = false)
    {
      switch (plane)
      {
        case Asix2.XY:
          return !inverse ? new Vector3(original.x, original.y, z) : new Vector3(original.y, original.x, z);
        case Asix2.XZ:
          return !inverse ? new Vector3(original.x, z, original.y) : new Vector3(original.y, z, original.x);
        case Asix2.YZ:
          return !inverse ? new Vector3(z, original.x, original.y) : new Vector3(z, original.y, original.x);
        default:
          return Vector3.zero;
      }
    }

    public static Vector3 Scale(this Vector3 original, float x = 1f, float y = 1f, float z = 1f) => new Vector3(original.x * x, original.y * y, original.z * z);

    public static Vector2 Rotate(this Vector2 v, float angle)
    {
      float f = angle * ((float) Math.PI / 180f);
      return new Vector2((float) ((double) v.x * (double) Mathf.Cos(f) - (double) v.y * (double) Mathf.Sin(f)), (float) ((double) v.x * (double) Mathf.Sin(f) + (double) v.y * (double) Mathf.Cos(f)));
    }

    public static float Angle(this Vector2 vector)
    {
      float num = Vector2.Angle(Vector2.right, vector);
      if ((double) vector.y < 0.0)
        num = 360f - num;
      return num;
    }

    public static void DestroyChilds(this Transform transform, bool immediate = false)
    {
      for (int index = 0; index < transform.childCount; ++index)
      {
        if (immediate)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) transform.GetChild(index).gameObject);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) transform.GetChild(index).gameObject);
      }
    }

    public static IEnumerable<Transform> AllChild(
      this Transform transform,
      bool all = true)
    {
      for (int i = 0; i < transform.childCount; ++i)
      {
        yield return transform.GetChild(i);
        if (all)
        {
          foreach (Transform transform1 in transform.GetChild(i).AllChild())
            yield return transform1;
        }
      }
    }

    public static Transform GetChildByPath(this Transform transform, string path)
    {
      Transform transform1 = transform;
      string str1 = path;
      char[] chArray = new char[2]{ '\\', '/' };
      foreach (string str2 in str1.Split(chArray))
      {
        string name = str2;
        if ((UnityEngine.Object) transform1 == (UnityEngine.Object) null)
          return (Transform) null;
        if (!name.IsNullOrEmpty())
          transform1 = transform1.AllChild(false).FirstOrDefault<Transform>((Func<Transform, bool>) (c => c.name == name));
      }
      if ((UnityEngine.Object) transform1 == (UnityEngine.Object) transform)
        transform1 = (Transform) null;
      return transform1;
    }

    public static IEnumerable<Transform> AndAllChild(
      this Transform transform,
      bool all = true)
    {
      yield return transform;
      foreach (Transform transform1 in transform.AllChild(all))
        yield return transform1;
    }

    public static void Reset(this Transform transform)
    {
      transform.localRotation = Quaternion.identity;
      transform.localPosition = Vector3.zero;
      transform.localScale = Vector3.one;
    }

    public static string NameFormat(
      this string name,
      string prefix,
      string suffix,
      bool addSpaces)
    {
      int startIndex = string.IsNullOrEmpty(prefix) || !name.StartsWith(prefix) ? 0 : prefix.Length;
      int length = string.IsNullOrEmpty(suffix) || !name.EndsWith(suffix) ? name.Length : name.Length - suffix.Length;
      name = name.Substring(startIndex, length);
      if (addSpaces)
        name = name.NameFormat();
      return name;
    }

    public static string NameFormat(this string name) => Utils.addSpacesRegex.Replace(name, " ");

    public static bool IsNullOrEmpty(this string text) => string.IsNullOrEmpty(text);

    public static string FormatText(this string text, params object[] values) => string.Format(text, values);

    public static double CheckSum(this string text)
    {
      double num1 = 0.0;
      int num2 = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        int utf32 = char.ConvertToUtf32(text[index].ToString(), 0);
        num1 += (double) ((utf32 + 1) * (num2 + 1) * (index + 1));
        num2 = utf32;
      }
      return num1;
    }

    public static string GenerateKey(int length)
    {
      StringBuilder stringBuilder = new StringBuilder();
      System.Random random = new System.Random();
      while (0 < length--)
        stringBuilder.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[random.Next("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length)]);
      return stringBuilder.ToString();
    }

    public static List<System.Type> FindInheritorTypes<T>()
    {
      System.Type type = typeof (T);
      return ((IEnumerable<System.Type>) type.Assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>) (x => type != x && type.IsAssignableFrom(x))).ToList<System.Type>();
    }

    public static T GetAttribute<T>(this System.Type type) where T : Attribute
    {
      System.Type type1 = typeof (T);
      foreach (object customAttribute in type.GetCustomAttributes(true))
      {
        if (type1.IsAssignableFrom(customAttribute.GetType()))
          return (T) customAttribute;
      }
      return default (T);
    }

    public static IEnumerable<T> GetAttributes<T>(this System.Type type) where T : Attribute
    {
      System.Type attributeType = typeof (T);
      return ((IEnumerable<object>) type.GetCustomAttributes(true)).Where<object>((Func<object, bool>) (x => attributeType.IsAssignableFrom(x.GetType()))).Cast<T>();
    }

    public static List<T> EnumValues<T>() => typeof (T).IsEnum ? Enum.GetValues(typeof (T)).OfType<T>().ToList<T>() : (List<T>) null;

    public static RectTransform rect(this Transform transform) => transform as RectTransform;
  }
}
