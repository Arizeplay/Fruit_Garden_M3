using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
namespace Ninsar.Store.Editor
{
    public static class StoreExporter
    {
        [MenuItem("Store/Export Store Items")]
        public static void ExportJson()
        {
            var baryStore = Object.FindObjectOfType<BerryStore>();

            if (baryStore == null)
                return;
            
            var path = EditorUtility.SaveFolderPanel("Export Store Items", "", "");
            
            if (path.Length > 0)
            {
                using var writer = new StreamWriter(Path.Combine(path, "store_items.json"));

                var items = baryStore.items.Where(x => x.purchaseType == BerryStore.Item.PurchaseType.CoinCurrency)
                    .Select(x => new Item
                    {
                        ItemID = x.id,
                        Cost = x.cost
                    });
                
                var json = JsonConvert.SerializeObject(items);

                writer.Write(json);
            }
        }
    }
}