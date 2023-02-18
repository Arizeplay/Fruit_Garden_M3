using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Yurowm.GameCore;

[Serializable]
public class LevelDesign 
{
    public static LevelDesign selected; // current level

    public int rawDesignIndex = 0; // Level number

    public const int maxSize = 12; // maximal playing field size
    public const int minSize = 4;// minimal playing field size
    public const int maxDeepHeight = 50;
    
    public int number = 0; // Level number

    public int firstStarScore = 100; // number of score points needed to get a first stars
    public int secondStarScore = 200; // number of score points needed to get a second stars
    public int thirdStarScore = 300;
    
    public int width
    {
        get => fieldSize.x;
        set => fieldSize.x = value;
    }
    public int height 
    {
        get => fieldSize.y;
        set => fieldSize.y = value;
    }
    
    public int2 fieldSize = new int2(5, 5);
    
    public bool deep = false;
    public int deepHeight = maxSize;

    public area activeArea => new area(int2.zero, new int2(width, height));
    public area area => new area(int2.zero, new int2(width, deep ? deepHeight : height));
    
    public string path = "";
    
    public SecondaryLevelDesign EasyLevelDesign;
    public SecondaryLevelDesign NormalLevelDesign;
    public SecondaryLevelDesign HardLevelDesign;
    public SecondaryLevelDesign ExtremeLevelDesign;
    public SecondaryLevelDesign InsaneLevelDesign;
    
    public List<ILevelExtension.LevelExtensionInfo> extensions = new ();

    public SecondaryLevelDesign GetCurrentSecondaryLevelDesign() => GetSecondaryLevelDesign(difficulty);

    public SecondaryLevelDesign GetSecondaryLevelDesign(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => EasyLevelDesign,
            Difficulty.Normal => NormalLevelDesign,
            Difficulty.Hard => HardLevelDesign,
            Difficulty.Extreme => ExtremeLevelDesign,
            Difficulty.Insane => InsaneLevelDesign,
            
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void SetSecondaryLevelDesign(Difficulty difficulty, SecondaryLevelDesign secondaryLevelDesign)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: EasyLevelDesign = secondaryLevelDesign; break;
            case Difficulty.Normal: NormalLevelDesign = secondaryLevelDesign; break;
            case Difficulty.Hard: HardLevelDesign = secondaryLevelDesign; break;
            case Difficulty.Extreme: ExtremeLevelDesign = secondaryLevelDesign; break;
            case Difficulty.Insane: InsaneLevelDesign = secondaryLevelDesign; break;
            
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public enum Difficulty
    {
        Easy = 0, 
        Normal = 1, 
        Hard = 2, 
        Extreme = 3, 
        Insane = 4
    }

    public Difficulty difficulty = Difficulty.Normal;

    public void DownDifficultyLevel()
    {
        if (difficulty > Difficulty.Easy)
        {
            difficulty -= 1;
        }
    }
    
    public void UpDifficultyLevel()
    {
        if (difficulty < Difficulty.Insane)
        {
            difficulty += 1;
        }
    }

    public LevelRule type
    {
        get => GetCurrentSecondaryLevelDesign().type;
        set => GetCurrentSecondaryLevelDesign().type = value;
    }

    public CompleteBonus bonus
    {
        get => GetCurrentSecondaryLevelDesign().bonus;
        set => GetCurrentSecondaryLevelDesign().bonus = value;
    }
    
    public string chipPhysic
    {
        get => GetCurrentSecondaryLevelDesign().chipPhysic;
        set => GetCurrentSecondaryLevelDesign().chipPhysic = value;
    }
    
    public int colorCount
    {
        get => GetCurrentSecondaryLevelDesign().colorCount;
        set => GetCurrentSecondaryLevelDesign().colorCount = value;
    }

    public List<LevelParameter> parameters
    {
        get => GetCurrentSecondaryLevelDesign().parameters;
        set => GetCurrentSecondaryLevelDesign().parameters = value;
    }

    public LevelParameter this[string index] {
        get
        {
            SecondaryLevelDesign secondaryLevelDesign = GetCurrentSecondaryLevelDesign();
            LevelParameter result = secondaryLevelDesign.parameters.Find(x => x.name == index);
            if (result == null) {
                result = new LevelParameter(index);
                secondaryLevelDesign.parameters.Add(result);
            }
            return result;
        }
    }

    public List<LevelGoalInfo> goals
    {
        get => GetCurrentSecondaryLevelDesign().goals;
        set => GetCurrentSecondaryLevelDesign().goals = value;
    }

    public int maxMoves => GetCurrentSecondaryLevelDesign().maxMoves;

    public int movesCount
    {
        get => GetCurrentSecondaryLevelDesign().movesCount;
        set => GetCurrentSecondaryLevelDesign().movesCount = value;
    }
    
    public List<SlotSettings>  slots
    {
        get => GetCurrentSecondaryLevelDesign().slots;
        set => GetCurrentSecondaryLevelDesign().slots = value;
    }

    public List<BigObjectSettings> bigObjects
    {
        get => GetCurrentSecondaryLevelDesign().bigObjects;
        set => GetCurrentSecondaryLevelDesign().bigObjects = value;
    }

    public XElement Serialize(string name) 
    {
        XElement xml = new XElement(name);
        
        xml.Add(new XAttribute("number", number));
        if (Application.isPlaying)
        {
            xml.Add(new XAttribute("difficulty", (int)difficulty));
        }
        
        xml.Add(new XAttribute("star1", firstStarScore));
        xml.Add(new XAttribute("star2", secondStarScore));
        xml.Add(new XAttribute("star3", thirdStarScore));
        xml.Add(new XAttribute("deep", deep ? "1" : "0"));
        xml.Add(new XAttribute("deepHeight", deepHeight));
        xml.Add(new XAttribute("size", fieldSize));
        xml.Add(new XAttribute("path", path));

        xml.Add(EasyLevelDesign.Serialize("EasyLevelDesign"));
        xml.Add(NormalLevelDesign.Serialize("NormalLevelDesign"));
        xml.Add(HardLevelDesign.Serialize("HardLevelDesign"));
        xml.Add(ExtremeLevelDesign.Serialize("InfernoLevelDesign"));
        xml.Add(InsaneLevelDesign.Serialize("InsaneLevelDesign"));
        
        var child = new XElement("extensions");
        xml.Add(child);
        foreach (var e in extensions) child.Add(e.Serialize("exten"));

        return xml;
    }
    
    public static LevelDesign DeserializeHalf(XElement xml)
    {
        var result = new LevelDesign();
        result.number = int.Parse(xml.Attribute("number").Value);

        result.firstStarScore = int.Parse(xml.Attribute("star1").Value);
        result.secondStarScore = int.Parse(xml.Attribute("star2").Value);
        result.thirdStarScore = int.Parse(xml.Attribute("star3").Value);
        
        return result;
    }

    public static LevelDesign DeserializeFull(XElement xml) 
    {
        var result = new LevelDesign
        {
            number = int.Parse(xml.Attribute("number").Value)
        };

        var difficultyXML = xml.Attribute("difficulty");
        if (difficultyXML != null)
        {
            result.difficulty = (Difficulty)int.Parse(difficultyXML.Value);
        }
        else
        {
            result.difficulty = Difficulty.Normal;
        }

        var normalXML = xml.Element("NormalLevelDesign");
        result.NormalLevelDesign = SecondaryLevelDesign.DeserializeFull(normalXML ?? xml);

        var firstStarScoreXml = xml.Attribute("star1");
        result.firstStarScore = firstStarScoreXml != null ? int.Parse(firstStarScoreXml.Value) : int.Parse(normalXML.Attribute("star1").Value);
        var secondStarScoreXml = xml.Attribute("star2");
        result.secondStarScore = secondStarScoreXml != null ? int.Parse(secondStarScoreXml.Value) : int.Parse(normalXML.Attribute("star2").Value);
        var thirdStarScoreXml = xml.Attribute("star3");
        result.thirdStarScore = thirdStarScoreXml != null ? int.Parse(thirdStarScoreXml.Value) : int.Parse(normalXML.Attribute("star3").Value);
        var deepXml = xml.Attribute("deep");
        result.deep = deepXml != null ? deepXml.Value == "1" : normalXML.Attribute("deep").Value == "1";
        var deepHeightXml = xml.Attribute("deepHeight");
        result.deepHeight = deepHeightXml != null ? int.Parse(deepHeightXml.Value) : int.Parse(normalXML.Attribute("deepHeight").Value);
        var fieldSizeXml = xml.Attribute("size");
        result.fieldSize = fieldSizeXml != null ? int2.Parse(fieldSizeXml.Value) : int2.Parse(normalXML.Attribute("size").Value);
        var pathXml = xml.Attribute("path");
        result.path = pathXml != null ? pathXml.Value : normalXML.Attribute("path").Value;
        
        var easyXML = xml.Element("EasyLevelDesign");
        result.EasyLevelDesign = easyXML != null ? SecondaryLevelDesign.DeserializeFull(easyXML) : result.NormalLevelDesign.Clone();
        
        var hardXML = xml.Element("HardLevelDesign");
        result.HardLevelDesign = hardXML != null ? SecondaryLevelDesign.DeserializeFull(hardXML) : result.NormalLevelDesign.Clone();
        
        var extremeXML = xml.Element("InfernoLevelDesign");
        result.ExtremeLevelDesign = extremeXML != null ? SecondaryLevelDesign.DeserializeFull(extremeXML) : result.NormalLevelDesign.Clone();
        
        var insaneHardXML = xml.Element("InsaneLevelDesign");
        result.InsaneLevelDesign = insaneHardXML != null ? SecondaryLevelDesign.DeserializeFull(insaneHardXML) : result.NormalLevelDesign.Clone();

        var extensionsXml = xml.Element("extensions");
        if (extensionsXml != null)
        {
            foreach (var e in extensionsXml.Elements())
                result.extensions.Add(ILevelExtension.LevelExtensionInfo.CteateAndDeserialize(e));
        }
        else
        {
            foreach (var e in normalXML.Element("extensions").Elements())
                result.extensions.Add(ILevelExtension.LevelExtensionInfo.CteateAndDeserialize(e));
        }
        
        return result;
    }

    public LevelDesign Clone()
    {
        LevelDesign clone = (LevelDesign) MemberwiseClone();
        EasyLevelDesign = clone.EasyLevelDesign?.Clone();
        NormalLevelDesign = clone.NormalLevelDesign?.Clone();
        HardLevelDesign = clone.HardLevelDesign?.Clone();
        ExtremeLevelDesign = clone.ExtremeLevelDesign?.Clone();
        InsaneLevelDesign = clone.InsaneLevelDesign?.Clone();
        
        clone.extensions = extensions.Select(x => x.Clone()).ToList();

        return clone;
    }

    public override string ToString() 
    {
        #if UNITY_EDITOR
        try 
        {
            return type.name + " (" + goals.Select(x => x.prefab.name).Join(", ") + ")";
        } 
        catch 
        {
            return "<color=red>Error</color>";
        }
        
        #else
        return base.ToString();
        #endif
    }
}