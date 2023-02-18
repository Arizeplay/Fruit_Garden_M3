using System;
using System.Xml.Linq;
using Yurowm.GameCore;

[Serializable]
public class StartBoosterSettings 
{
    public IBooster booster = null;

    public StartBoosterSettings(IBooster booster)
    {
        this.booster = booster;
    }
    
    public StartBoosterSettings(string boosterName) 
    {
        this.booster = Content.GetPrefab<IBooster>(x => x.name == boosterName);
    }

    public XElement Serialize(string name) 
    {
        var xml = new XElement(name);
        xml.Add(new XAttribute("startBooster", booster.name));
        return xml;
    }

    public static StartBoosterSettings Deserialize(XElement xml)
    {
        var result = new StartBoosterSettings(xml.Attribute("startBooster").Value);
        return result;
    }
    
    public StartBoosterSettings Clone() 
    {
        return (StartBoosterSettings) MemberwiseClone();
    }
}