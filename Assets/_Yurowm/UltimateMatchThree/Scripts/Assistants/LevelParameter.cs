using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Yurowm.GameCore;

[Serializable]
public class LevelParameter 
{
    public string name;

    public float Float = 0;
    public AnimationCurve Curve = null;
    public ItemColor ItemColor = ItemColor.Unknown;
    public int2 Coordinate = new int2();
    public string Text = "";

    public int Int 
    {
        get => Mathf.RoundToInt(Float);
        set => Float = value;
    }
    
    public bool Bool
    {
        get => Float == 1;
        set => Float = value ? 1 : 0;
    }

    public LevelParameter(string name) 
    {
        this.name = name;
    }

    public XElement Serialize(string name)
    {
        var xml = new XElement(name);
        Serialize(xml);
        return xml;
    }

    public LevelParameter Clone() => (LevelParameter)MemberwiseClone();

    public void Serialize(XElement xml)
    {
        xml.Add(new XAttribute("name", name));
        xml.Add(new XAttribute("value", Float));
        xml.Add(new XAttribute("coord", Coordinate));
        xml.Add(new XAttribute("color", (int) ItemColor));
        xml.Value = Text;

        if (Curve == null) return;
        
        if (Curve.keys.Length == 2 && Curve[0].value == 0 && Curve[1].value == 0)
            return;
 
        var curve = new XElement("curve");
        foreach (var key in Curve.keys)
            curve.Add(key.Serialize("key"));
        xml.Add(curve);
    }

    public static LevelParameter Deserialize(XElement xml) 
    {
        var result = new LevelParameter(xml.Attribute("name").Value);
        result.Float = float.Parse(xml.Attribute("value").Value);
        result.Coordinate = int2.Parse(xml.Attribute("coord").Value);
        result.ItemColor = (ItemColor) int.Parse(xml.Attribute("color").Value);
        result.Text = xml.Value;

        if (xml.Attribute("curve") is { Value: { } s } && JsonConvert.DeserializeObject<AnimationCurve>(s) is { } c)
        {
            result.Curve = c;
        }
        
        var keyList = new List<Keyframe>();
        var curveXml = xml.Element("curve");
        if (curveXml != null)
        {
            foreach (var element in curveXml.Elements())
            {
                keyList.Add(KeyframeSerialization.Deserialize(element));
            }
        }
        
        if (keyList.Count > 0)
        {
            var curve = new AnimationCurve(keyList.ToArray());
            result.Curve = curve;
        }
        
        return result;
    }
}