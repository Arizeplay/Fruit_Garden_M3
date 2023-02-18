using System;
using System.Xml.Linq;
using Yurowm.GameCore;
[Serializable]
public class BoosterSettings 
{
    public IBooster booster = null;

    public BoosterSettings(IBooster booster) 
    {
        this.booster = booster;
    }
    
    public BoosterSettings(string boosterName)
    {
        this.booster = Content.GetPrefab<IBooster>(x => x.name == boosterName);
    }

    public XElement Serialize(string name) 
    {
        var xml = new XElement(name);
        xml.Add(new XAttribute("booster", booster.name));
        return xml;
    }

    public static BoosterSettings Deserialize(XElement xml)
    {
        var result = new BoosterSettings(xml.Attribute("booster").Value);
        return result;
    }
    
    public BoosterSettings Clone() => (BoosterSettings) MemberwiseClone();
}