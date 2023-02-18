using System.Collections.Generic;
using System.Linq;
using Ninsar.Store;
using UnityEngine;
using Yurowm.GameCore;

namespace Ninsar.Networking
{
    public class StoreParser : MonoBehaviour
    {
        private void Start()
        {
            SetPrices();

            UIAssistant.onShowPage += page =>
            {
                if (page.name == "Store")
                {
                    SetPrices();
                }
            };
        }

        private async void SetPrices()
        {
            var httpClient = new HttpClient(new JsonSerializationOption());
            var items = await httpClient.Get<List<Item>>("https://tariroplay.com/Ghjdk_jkhg&678_&*kghgklhglkjh.json");

            if (items.IsNullOrEmpty()) return;
            
            foreach (var item in items)
            {
                Debug.Log(item.ItemID + " : " + item.Cost);
            }

            if (!Application.isEditor || (Application.isEditor && Application.isPlaying))
            {
                var store = FindObjectOfType<BerryStore>();

                if (!store) return;

                foreach (var item in items)
                {
                    var storeItem = store.items.FirstOrDefault(x => x.id == item.ItemID);
                    if (storeItem != default)
                    {
                        storeItem.cost = item.Cost;
                    }
                }

                ItemCounter.RefreshAll();
            }
        }
    }
}