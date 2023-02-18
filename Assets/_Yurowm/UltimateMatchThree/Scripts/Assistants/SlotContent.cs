using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Yurowm.GameCore;

[Serializable]
public class SlotContent 
{
    public enum Type { Chip, Block, Modifier, Unknown }

    public static Type GetContentType(ISlotContent content) 
    {
        return content is IChip ? Type.Chip : 
            content is IBlock ? Type.Block :
            content is ISlotModifier ? Type.Modifier :
            Type.Unknown;
    }

    public string name;
    public Type type;

    public SlotContent(string name, Type type) 
    {
        this.name = name;
        this.type = type;
    }

    public List<LevelParameter> parameters = new ();
    public LevelParameter this[string index]
    {
        get 
        {
            var result = parameters.Find(x => x.name == index);
            if (result != null) return result;
            result = new LevelParameter(index);
            parameters.Add(result);
            return result;
        }
    }
    public bool HasParameter(string index) => parameters.Contains(x => x.name == index);


    public XElement Serialize(string name)
    {
        var xml = new XElement(name);
        xml.Add(new XAttribute("name", this.name));
        xml.Add(new XAttribute("type", (int) type));
        foreach (var p in parameters)
            xml.Add(p.Serialize("p"));
        return xml;
    }

    public static SlotContent Deserialize(XElement xml) 
    {
        var name = xml.Attribute("name").Value;
        var type = (Type) int.Parse(xml.Attribute("type").Value);
        var result = new SlotContent(name, type);
        foreach (var x in xml.Elements("p"))
            result.parameters.Add(LevelParameter.Deserialize(x));
        return result;
    }

    public SlotContent Clone()
    {
        var result = (SlotContent) MemberwiseClone();
        result.parameters = parameters.Select(x => x.Clone()).ToList();
        return result;
    }
}