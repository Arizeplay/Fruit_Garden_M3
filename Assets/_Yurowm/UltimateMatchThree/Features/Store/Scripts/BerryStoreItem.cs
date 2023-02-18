using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BerryStoreItem : MonoBehaviour
{
    public BerryStoreItemLayout[] layouts;
    
    [HideInInspector]
    public BerryStore.Item item;

    public Sprite coin;
    
    void Awake ()
    {
        foreach (var layout in layouts)
        {
            layout.iapButton.purchaseButton.onClick.AddListener(Purchase);
            layout.iapButton.purchaseButton.gameObject.SetActive(false);
            layout.coinsButton.purchaseButton.onClick.AddListener(Purchase);
            layout.coinsButton.purchaseButton.gameObject.SetActive(false);
            layout.rewardButton.purchaseButton.onClick.AddListener(Purchase);
            layout.rewardButton.purchaseButton.gameObject.SetActive(false);
        }

        ItemCounter.refresh += Refresh;
    }
     
    void OnEnable() {
        Refresh();
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
                        "Price:" + (item.cost));
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
    }

    public void Refresh()
    {
        var layout = layouts.FirstOrDefault(x => x.icon.Length <= item.icons.Count);
        if (layout == null)
            return;
        
        layout.gameObject.SetActive(true);
        
        string name = item.localized ? LocalizationAssistant.main[item.localization_Name] : item.Name;
        string description = item.localized ? LocalizationAssistant.main[item.localization_Description] : item.Descrition;

        layout.title.text = name;
        if (!string.IsNullOrEmpty(description))
        {
            if (layout.description) {
                layout.description.text = description;
            }
            else {
                layout.title.text += string.Format("\n<size=60%>{0}</size>", description);
            }
        }

        if (layout.infoButton)
            layout.infoButton.SetActive(item.showInfoButton);

        if (layout.tape)
        {
            layout.tape.SetActive(item.showTape);
            layout.tapeDescription.text = item.tapeDescription;
        }

        BerryStoreItemLayout.PurchaseButton purchaseButton = null;
        
        switch (item.purchaseType)
        {
            case BerryStore.Item.PurchaseType.IAP: purchaseButton = layout.iapButton; break;
            case BerryStore.Item.PurchaseType.RewardedVideo: purchaseButton = layout.rewardButton; break;
            case BerryStore.Item.PurchaseType.CoinCurrency: purchaseButton = layout.coinsButton; break;
        }

        purchaseButton.purchaseButton.gameObject.SetActive(true);
        purchaseButton.purchaseButton.interactable = item.alwaysAvaliable || item.avaliableWhen.GetResult();

        if (purchaseButton.purchaseButton.interactable) {
            switch (item.purchaseType) {
                case BerryStore.Item.PurchaseType.CoinCurrency:
                    purchaseButton.purchaseButton.interactable = CurrentUser.main[ItemID.coin] >= item.cost;
                    break;
            }
        }

        for (int iconIndex = 0; iconIndex < layout.icon.Length; iconIndex++) {
            if (item.icons[iconIndex] != null) {
                layout.icon[iconIndex].icon.sprite = item.icons[iconIndex].icon;
                var iconDescription = item.icons[iconIndex].localized
                    ? LocalizationAssistant.main[item.localization_Icon(iconIndex)] : item.icons[iconIndex].description;

                if (!string.IsNullOrEmpty(iconDescription)) {
                    if (layout.icon[iconIndex].description) {
                        layout.icon[iconIndex].description.gameObject.SetActive(true);
                        layout.icon[iconIndex].description.text = iconDescription;
                    }
                }else {
                    if (layout.icon[iconIndex].description) {
                        layout.icon[iconIndex].description.gameObject.SetActive(false);
                    }
                }
            }
            else {
                layout.icon[iconIndex].icon.gameObject.SetActive(false);
            }
        }
        
        

        switch (item.purchaseType) {
            case BerryStore.Item.PurchaseType.IAP:
                var iap = BerryStore.main.marketItemPrices.ContainsKey(item.iap);
                purchaseButton.price.text = iap ? BerryStore.main.marketItemPrices[item.iap] : "N/A";

                break;
            case BerryStore.Item.PurchaseType.CoinCurrency:
                purchaseButton.icon.sprite = coin;
                purchaseButton.price.text = $"{item.cost}";
                break;
            case BerryStore.Item.PurchaseType.RewardedVideo:
                purchaseButton.price.text = LocalizationAssistant.main["store/watch"];
                break;
        }
    }
}
