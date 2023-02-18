using System.Collections.Generic;
using System.Xml.Linq;

public class RewardContainer
{
    public List<RewardItem> Items = new List<RewardItem>();

    public void Obtain()
    {
        Items.ForEach(i => i.Obtain());
    }
        
    public XElement Serialize(string name)
    {
        XElement xml = new XElement(name);
        var itemsXml = new XElement("items");
        xml.Add(itemsXml);
        foreach (var item in Items)
        {
            var itemXml = new XElement("item");
            itemsXml.Add(itemXml);

            itemXml.Add(new XAttribute("id", (int)item.ItemID));
            itemXml.Add(new XAttribute("count", item.Count));
        }
            
        return xml;
    }
        
    public static RewardContainer Deserialize(XElement xml)
    {
        var itemsXml = xml.Element("items");

        var items = new List<RewardItem>();
        foreach (var item in itemsXml.Elements())
        {
            var itemID = (ItemID)int.Parse(item.Attribute("id").Value);
            var count = int.Parse(item.Attribute("count").Value);
                
            items.Add(new RewardItem
            {
                ItemID = itemID,
                Count = count,
            });
        }
            
        return new RewardContainer
        {
            Items = items
        };
    }
}