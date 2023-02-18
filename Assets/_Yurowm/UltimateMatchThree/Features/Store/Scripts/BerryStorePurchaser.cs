using System;
using System.Linq;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BerryStorePurchaser : MonoBehaviour {
    public string itemID;
    [Space]
    public TextMeshProUGUI price;
    public Button purchaseButton;
    public Image icon;
    public TextMeshProUGUI title;
    public bool withoutDescription;
    [HideInInspector]
    public BerryStore.Item item;
    [Space]
    public UnityEvent onPurchase;

    public Image CurrencyIcon;

    void Awake () {
        item = BerryStore.main.items.FirstOrDefault(i => i.id == itemID);
        if (item != null)
        {
            ItemCounter.refresh += Refresh;
        }
        
        purchaseButton.onClick.AddListener(Purchase);

        SetItem(item);
    }

    public void SetItem(BerryStore.Item item)
    {
        this.item = item;
    }
     
    private void OnEnable() {
        if (item != null)
        {
            item.onPurchase.AddListener(OnPurchase);
            Refresh();
        }
    }

    private void OnDisable()
    {
        if (item != null)
            item.onPurchase.RemoveListener(OnPurchase);
        
        ItemCounter.refresh -= Refresh;
    }

    private void OnPurchase()
    {
        onPurchase.Invoke();
    }
    
	public void Purchase () {
        switch (item.purchaseType) {
            case BerryStore.Item.PurchaseType.IAP:
                Debug.Log(item.iap);
                AppMetrica p = GameObject.Find("AppMetrica").GetComponent<AppMetrica>();
                string json = $"{{\"Purchase Button Pressed\":\"{item.iap}\"}}";

                p.SendEvent("Purchase Button Pressed", json);

                BerryStore.main.PurchaseIAP(item, item.iap, item.pack, item.onPurchase); break;
            case BerryStore.Item.PurchaseType.CoinCurrency: {
                    BerryAnalytics.Event("Purchase",
                        "PurchaseID:" + item.id,
                        "Price:" + item.cost);
                    BerryStore.ItemPack.Purchase(item, item.pack, item.onPurchase, item.cost); break;
                }
            case BerryStore.Item.PurchaseType.RewardedVideo: {
                Debug.Log("Show ADS in Pig ");
                    BerryAnalytics.Event("Rewarded Ad Request");
                    Advertising.main.ShowAds(AdType.PigInAPoke,() => {
                        BerryAnalytics.Event("Rewarded Ad Reward", "PurchaseID:" + item.id);
                        BerryStore.ItemPack.Purchase(item.pack, item.onPurchase);
                    });
                } break;
        }

        Refresh();
    }

    public void Refresh() {
        string name = item.localized ? LocalizationAssistant.main[item.localization_Name] : item.Name;
        string description = item.localized ? LocalizationAssistant.main[item.localization_Description] : item.Descrition;
        
        if (title)
        {
            title.text = name;
            if (!withoutDescription)
            {
                if (!string.IsNullOrEmpty(description))
                    title.text += string.Format("\n<size=60%>{0}</size>", description);
            }
        }

        purchaseButton.interactable = item.alwaysAvaliable || item.avaliableWhen.GetResult();

        if (purchaseButton.interactable) {
            switch (item.purchaseType) {
                case BerryStore.Item.PurchaseType.CoinCurrency:
                    purchaseButton.interactable = CurrentUser.main[ItemID.coin] >= item.cost;
                    break;
            }
        }

        if (icon)
        {
            icon.sprite = item.icons[0].icon;
        }

        if (price)
        {
            switch (item.purchaseType) {
                case BerryStore.Item.PurchaseType.IAP:
                    {
                        CurrencyIcon.gameObject.SetActive(false);
                        price.text = BerryStore.main.marketItemPrices.ContainsKey(item.iap) ? BerryStore.main.marketItemPrices[item.iap] : "N/A";
                    }
                    break;
                case BerryStore.Item.PurchaseType.CoinCurrency:
                    CurrencyIcon.gameObject.SetActive(true);
                    CurrencyIcon.sprite = ItemIcons.main.GetIconOrNull(ItemID.coin);
                    var coinCost = item.cost;
                    price.text = string.Format(coinCost.ToString());
                    break;
                case BerryStore.Item.PurchaseType.RewardedVideo:
                    CurrencyIcon.gameObject.SetActive(false);
                    price.text = LocalizationAssistant.main["store/watch"];
                    break;
            }
        }
    }
}
