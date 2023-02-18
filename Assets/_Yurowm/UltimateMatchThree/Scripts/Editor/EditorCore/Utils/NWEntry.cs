// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWEntry
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;

namespace Yurowm.EditorCore.Utils
{
  internal abstract class NWEntry
  {
    public string name;
    public object value;

    public abstract string type { get; }

    public NWEntry(string name, object value)
    {
      this.name = name;
      this.value = value;
    }

    public abstract string ToRaw();

    public abstract void Parse(string raw);

    public static NWEntry Parse(string type, string name, string value)
    {
      if (type == "s")
        return (NWEntry) new NWEntryString(name, value);
      if (!(type == "i"))
      {
        if (!(type == "f"))
        {
          if (!(type == "b"))
          {
            if (!(type == "dt"))
              return (NWEntry) null;
            NWEntryDateTime nwEntryDateTime = new NWEntryDateTime(name, new DateTime());
            nwEntryDateTime.Parse(value);
            return (NWEntry) nwEntryDateTime;
          }
          NWEntryBool nwEntryBool = new NWEntryBool(name, false);
          nwEntryBool.Parse(value);
          return (NWEntry) nwEntryBool;
        }
        NWEntryFloat nwEntryFloat = new NWEntryFloat(name, 0.0f);
        nwEntryFloat.Parse(value);
        return (NWEntry) nwEntryFloat;
      }
      NWEntryInteger nwEntryInteger = new NWEntryInteger(name, 0);
      nwEntryInteger.Parse(value);
      return (NWEntry) nwEntryInteger;
    }

    public override string ToString() => string.Format("{0}: {1}", (object) this.name, (object) this.ToRaw());
  }
}
