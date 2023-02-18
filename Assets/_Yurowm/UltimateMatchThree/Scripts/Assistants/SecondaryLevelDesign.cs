using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Yurowm.GameCore;

public class SecondaryLevelDesign
{
    public LevelRule type = null;
    public CompleteBonus bonus = null;
    public string chipPhysic = "";
    
    public int colorCount = 4;
    
    public List<LevelParameter> parameters = new List<LevelParameter>();

    public LevelParameter this[string index] {
        get {
            LevelParameter result = parameters.Find(x => x.name == index);
            if (result == null) {
                result = new LevelParameter(index);
                parameters.Add(result);
            }
            return result;
        }
    }
    
    public List<LevelGoalInfo> goals = new ();

    public int maxMoves = 30;
    public int movesCount = 30;
    
    public List<SlotSettings> slots = new ();
    public List<BigObjectSettings> bigObjects = new ();
    
    
    public XElement Serialize(string name) 
    {
        XElement xml = new XElement(name);
        xml.Add(new XAttribute("type", type ? type.name : ""));
        xml.Add(new XAttribute("bonus", bonus ? bonus.name : ""));
        xml.Add(new XAttribute("color", colorCount));
        xml.Add(new XAttribute("moves", movesCount));
        xml.Add(new XAttribute("physic", chipPhysic));

        XElement child = new XElement("parameters");
        xml.Add(child);
        foreach (var p in parameters) child.Add(p.Serialize("param"));

        child = new XElement("goals");
        xml.Add(child);
        foreach (var g in goals) child.Add(g.Serialize("goal"));

        child = new XElement("slots");
        xml.Add(child);
        foreach (var s in slots) child.Add(s.Serialize("slot"));

        child = new XElement("bigObjects");
        xml.Add(child);
        foreach (var bo in bigObjects) child.Add(bo.Serialize("bigObject"));

        return xml;
    }
    
    public static SecondaryLevelDesign DeserializeHalf(XElement xml) 
    {
        var result = new SecondaryLevelDesign
        {
            type = Content.GetPrefab<LevelRule>(xml.Attribute("type").Value),
            bonus = Content.GetPrefab<CompleteBonus>(xml.Attribute("bonus").Value),
            colorCount = int.Parse(xml.Attribute("color").Value),
            movesCount = int.Parse(xml.Attribute("moves").Value),
        };
        result.maxMoves = result.movesCount;

        return result;
    }

    public static SecondaryLevelDesign DeserializeFull(XElement xml) 
    {
        var result = new SecondaryLevelDesign
        {
            type = Content.GetPrefab<LevelRule>(xml.Attribute("type").Value),
            bonus = Content.GetPrefab<CompleteBonus>(xml.Attribute("bonus").Value),
            colorCount = int.Parse(xml.Attribute("color").Value),
            movesCount = int.Parse(xml.Attribute("moves").Value),

        };
        result.maxMoves = result.movesCount;
        
        var attribute = xml.Attribute("physic");
        if (attribute != null) result.chipPhysic = attribute.Value;

        foreach (var e in xml.Element("parameters").Elements())
            result.parameters.Add(LevelParameter.Deserialize(e));

        foreach (var e in xml.Element("goals").Elements())
            result.goals.Add(LevelGoalInfo.Deserialize(e));

        foreach (var e in xml.Element("slots").Elements())
            result.slots.Add(SlotSettings.Deserialize(e));

        var boXML = xml.Element("bigObjects");
        if (boXML != null)
            foreach (var e in boXML.Elements())
                result.bigObjects.Add(BigObjectSettings.Deserialize(e));

        return result;
    }
    
    public SecondaryLevelDesign Clone() 
    {
        var clone = (SecondaryLevelDesign) MemberwiseClone();
        clone.slots = slots.Select(x => x.Clone()).ToList();
        clone.goals = goals.Select(x => x.Clone()).ToList();
        return clone;
    }
}