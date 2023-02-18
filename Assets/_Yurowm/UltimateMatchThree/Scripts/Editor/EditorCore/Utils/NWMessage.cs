// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Utils.NWMessage
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Yurowm.EditorCore.Utils
{
  internal class NWMessage : ICollection<NWEntry>, IEnumerable, IEnumerable<NWEntry>
  {
    private List<NWEntry> list = new List<NWEntry>();
    public List<string> tags = new List<string>();
    private NWEntryProtocolName protocol;
    private NWEntryResult result;

    public NWMessage()
    {
    }

    public NWMessage(Type protocol)
      : this()
    {
      this.protocol = new NWEntryProtocolName(protocol);
    }

    public int Count => this.list.Count;

    public bool IsReadOnly => true;

    public void Add(NWEntry item) => this.list.Add(item);

    public void Clear() => this.list.Clear();

    public bool Contains(NWEntry item) => this.list.Contains(item);

    public bool ContainsName(string name) => this.list.Find((Predicate<NWEntry>) (x => x.name == name)) != null;

    public void CopyTo(NWEntry[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

    public bool Remove(NWEntry item) => this.list.Remove(item);

    public string ToJSON()
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.AppendChild((XmlNode) xmlDocument.CreateElement("m"));
      XmlElement documentElement = xmlDocument.DocumentElement;
      if (this.protocol != null)
        documentElement.SetAttribute("p", this.protocol.ToRaw());
      if (this.result != null)
        documentElement.SetAttribute("r", this.result.ToRaw());
      if (this.tags.Count > 0)
        documentElement.SetAttribute("t", string.Join("~", this.tags.ToArray()));
      foreach (NWEntry nwEntry in this.list)
      {
        XmlElement element = xmlDocument.CreateElement(nwEntry.type);
        element.SetAttribute("n", nwEntry.name);
        element.SetAttribute("v", nwEntry.ToRaw());
        documentElement.AppendChild((XmlNode) element);
      }
      return xmlDocument.OuterXml;
    }

    public static NWMessage FromJSON(string raw)
    {
      if (string.IsNullOrEmpty(raw))
        return new NWMessage()
        {
          (NWEntry) new NWEntryResult(Result.Error)
        };
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(raw);
      XmlElement documentElement = xmlDocument.DocumentElement;
      NWMessage nwMessage = new NWMessage();
      XmlAttribute attributeNode1 = documentElement.GetAttributeNode("p");
      if (attributeNode1 != null)
      {
        NWEntryProtocolName protocol = new NWEntryProtocolName();
        protocol.Parse(attributeNode1.Value);
        nwMessage.SetProtocol(protocol);
      }
      XmlAttribute attributeNode2 = documentElement.GetAttributeNode("r");
      if (attributeNode2 != null)
      {
        NWEntryResult result = new NWEntryResult();
        result.Parse(attributeNode2.Value);
        nwMessage.SetResult(result);
      }
      XmlAttribute attributeNode3 = documentElement.GetAttributeNode("t");
      if (attributeNode3 != null && !string.IsNullOrEmpty(attributeNode3.Value))
        nwMessage.tags = ((IEnumerable<string>) attributeNode3.Value.Split('~')).ToList<string>();
      foreach (XmlElement childNode in documentElement.ChildNodes)
        nwMessage.Add(NWEntry.Parse(childNode.Name, childNode.GetAttributeNode("n").Value, childNode.GetAttributeNode("v").Value));
      return nwMessage;
    }

    public void SetProtocol(NWEntryProtocolName protocol) => this.protocol = protocol;

    public NWProtocol CreateProtocol(SocketConnection connection) => this.protocol == null ? (NWProtocol) null : this.protocol.GetProtocol(connection);

    public void SetResult(NWEntryResult result) => this.result = result;

    public Result GetResult() => this.result != null ? this.result.GetResult() : Result.Unknown;

    public Reason GetReason() => this.result != null ? this.result.GetReason() : Reason.Unknown;

    public IEnumerator<NWEntry> GetEnumerator() => this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.list.GetEnumerator();

    public object this[string index]
    {
      get
      {
        try
        {
          return this.list.Find((Predicate<NWEntry>) (x => x.name == index)).value;
        }
        catch (Exception ex)
        {
          return (object) null;
        }
      }
    }
  }
}
