using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Yurowm.GameCore;

[Serializable]
public class SlotExtension 
{
    public int2 coord;

    public SlotExtension(int2 coord)
    {
        this.coord = coord;
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

    public SlotExtension Clone()
    {
        var result = (SlotExtension) MemberwiseClone();
        result.parameters = parameters.Select(x => x.Clone()).ToList();
        return result;
    }

    public void Serialize(XElement xml)
    {
        xml.Add(new XAttribute("coord", coord));
        foreach (var parameter in parameters) {
            var element = new XElement("param");
            parameter.Serialize(element);
            xml.Add(element);
        }
    }

    public static SlotExtension Deserialize(XElement xml)
    {
        var result = new SlotExtension(int2.Parse(xml.Attribute("coord").Value));
        foreach (var element in xml.Elements("param"))
            result.parameters.Add(LevelParameter.Deserialize(element));
        return result;
    }
}