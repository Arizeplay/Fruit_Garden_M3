using System;
using System.Xml.Linq;
using Yurowm.GameCore;
[Serializable]
public class BigObjectSettings 
{
    public SlotContent content;
    public int2 position;

    public BigObjectSettings(int2 position)
    {
        this.position = position;
    }

    public XElement Serialize(string name) 
    {
        var xml = new XElement(name);
        xml.Add(new XAttribute("position", position));
        xml.Add(content.Serialize("c"));
        return xml;
    }

    public static BigObjectSettings Deserialize(XElement xml)
    {
        var result = new BigObjectSettings(int2.Parse(xml.Attribute("position").Value));
        result.content = SlotContent.Deserialize(xml.Element("c"));
        return result;
    }

    public BigObjectSettings Clone()
    {
        var result = (BigObjectSettings) MemberwiseClone();
        result.content = content.Clone();
        return result;
    }
}