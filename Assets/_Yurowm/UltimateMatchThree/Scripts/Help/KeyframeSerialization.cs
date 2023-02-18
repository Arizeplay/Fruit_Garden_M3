using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

public static class KeyframeSerialization 
{
    public static XElement Serialize(this Keyframe keyframe, string name)
    {
        var xml = new XElement(name);

        xml.Add(new XAttribute("time", (float)keyframe.time));
        xml.Add(new XAttribute("value", (float)keyframe.value));
        xml.Add(new XAttribute("inTangent", (float)keyframe.inTangent));
        xml.Add(new XAttribute("outTangent", (float)keyframe.outTangent));
        xml.Add(new XAttribute("inWeight", (float)keyframe.inWeight));
        xml.Add(new XAttribute("outWeight", (float)keyframe.outWeight));
        
        return xml;
    }

    public static Keyframe Deserialize(XElement xml)
    {
        var keyframe = new Keyframe();

        var timeXml = xml.Attribute("time");
        if (timeXml != null)
        {
            keyframe.time = float.Parse(timeXml.Value, CultureInfo.InvariantCulture);
        }

        var valueXml = xml.Attribute("value");
        if (valueXml != null)
        {
            keyframe.value = float.Parse(valueXml.Value, CultureInfo.InvariantCulture);
        }
        
        var inTangentXml = xml.Attribute("inTangent");
        if (inTangentXml != null)
        {
            keyframe.inTangent = float.Parse(inTangentXml.Value, CultureInfo.InvariantCulture);
        }
        
        var outTangentXml = xml.Attribute("outTangent");
        if (outTangentXml != null)
        {
            keyframe.outTangent = float.Parse(outTangentXml.Value, CultureInfo.InvariantCulture);
        }
        
        var inWeightXml = xml.Attribute("inWeight");
        if (inWeightXml != null)
        {
            keyframe.inWeight = float.Parse(inWeightXml.Value, CultureInfo.InvariantCulture);
        }
        
        var outWeightXml = xml.Attribute("outWeight");
        if (outWeightXml != null)
        {
            keyframe.outWeight = float.Parse(outWeightXml.Value, CultureInfo.InvariantCulture);
        }
        
        return keyframe;
    }
}
