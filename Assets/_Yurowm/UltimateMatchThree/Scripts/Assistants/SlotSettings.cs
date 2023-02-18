using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Yurowm.GameCore;

[Serializable]
public class SlotSettings 
{
    public SlotContent chip 
    {
        get => content.FirstOrDefault(x => x.type == SlotContent.Type.Chip);
        set 
        {
            content.RemoveAll(x => x.type == SlotContent.Type.Chip);
            if (value != null) 
            {
                value.type = SlotContent.Type.Chip;
                content.Add(value);
            }
        }
    }
    public SlotContent block 
    {
        get => content.FirstOrDefault(x => x.type == SlotContent.Type.Block);
        set 
        {
            content.RemoveAll(x => x.type == SlotContent.Type.Block);
            if (value != null)
            {
                value.type = SlotContent.Type.Block;
                content.Add(value);
            }
        }
    }
    
    public SlotContent current
    {
        get
        {
            var current = block;
            if (current != null) return current;
            current = chip;
            return current ?? null;
        }

    }

    public int2 position;
    public List<SlotContent> content = new ();

    public SlotSettings(int x, int y) 
    {
        position = new int2(x, y);
    }

    public SlotSettings(int2 position)
    {
        this.position = position;
    }

    public XElement Serialize(string name) 
    {
        var xml = new XElement(name);
        xml.Add(new XAttribute("position", position));
        foreach (var c in content)
            xml.Add(c.Serialize("c"));
        return xml;
    }

    public static SlotSettings Deserialize(XElement xml) 
    {
        var result = new SlotSettings(int2.Parse(xml.Attribute("position").Value));
        foreach (var x in xml.Elements("c"))
            result.content.Add(SlotContent.Deserialize(x));
        return result;
    }

    public SlotSettings Clone() 
    {
        var result = (SlotSettings) MemberwiseClone();
        result.content = content.Select(x => x.Clone()).ToList();
        if (chip != null) result.chip = chip.Clone();
        if (block != null) result.block = block.Clone();
        return result;
    }

    public bool HasContent(string name) 
    {
        for (int i = 0; i < content.Count; i++)
            if (content[i].name == name)
                return true;
        return false;
    }

    public SlotContent GetSlotContent(string name)
    {
        for (int i = 0; i < content.Count; i++)
            if (content[i].name == name)
                return content[i];
        return null;
    }
}