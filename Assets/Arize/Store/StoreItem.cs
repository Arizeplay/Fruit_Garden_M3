using System.Collections.Generic;
using System.Linq;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Ninsar.Store
{
    public class StoreItem : MonoBehaviour
    {
        public static ItemID selected;
    
        public BerryStorePurchaser Prefab;

        public Transform PurchaserHolder;

        public Image Icon;
        public TMP_Text Title;
        public TMP_Text Description;
    
        private List<GameObject> _storeItems;
    
        private void OnEnable()
        {
            _storeItems = new List<GameObject>();

            var group = BerryStore.main.items.First(x => x.group.IsItemGroup && x.group.ItemGroup == selected).@group;
            var items = BerryStore.main.items.Where(x => x.group.IsItemGroup && x.group.ItemGroup == selected).ToArray();

            Icon.sprite = ItemIcons.main.GetIconOrNull(selected);
            Title.text = LocalizationAssistant.main[group.localization_Name];
            Description.text = LocalizationAssistant.main[group.localization_Description];
        
            foreach (var item in items)
            {
                var purchaser = Instantiate(Prefab, PurchaserHolder);
                purchaser.SetItem(item);
                purchaser.Refresh();
            
                _storeItems.Add(purchaser.gameObject);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _storeItems.Count; i++)
            {
                Destroy(_storeItems[i]);
            }
        }
    }
}
