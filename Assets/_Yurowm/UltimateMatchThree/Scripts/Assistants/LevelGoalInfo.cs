using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Yurowm.GameCore;

[Serializable]
public class LevelGoalInfo 
{
    public ILevelGoal prefab;

    public List<LevelParameter> parameters = new ();

    public LevelGoalInfo(ILevelGoal prefab) 
    {
        this.prefab = prefab;
    }

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
        xml.Add(new XAttribute("type", prefab ? prefab.name : ""));
        foreach (var p in parameters)
            xml.Add(p.Serialize("param"));
        return xml;
    }

    public static LevelGoalInfo Deserialize(XElement xml) {
        var result = new LevelGoalInfo(Content.GetPrefab<ILevelGoal>(xml.Attribute("type").Value));
        foreach (var p in xml.Elements("param"))
            result.parameters.Add(LevelParameter.Deserialize(p));
        return result;
    }

    public LevelGoalInfo Clone() => (LevelGoalInfo) MemberwiseClone();
}