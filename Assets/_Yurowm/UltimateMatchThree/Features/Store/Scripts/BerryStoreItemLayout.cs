using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BerryStoreItemLayout : MonoBehaviour
{
    public GameObject infoButton;

    public Icons[] icon;
    [Space] 
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    [Space]
    public PurchaseButton iapButton;
    public PurchaseButton coinsButton;
    public PurchaseButton rewardButton;
    [Space]
    public GameObject tape;
    public TextMeshProUGUI tapeDescription;

    [Serializable]
    public class PurchaseButton
    {
        public Button purchaseButton;
        public TextMeshProUGUI price;
        [FormerlySerializedAs("Icon")]
        public Image icon;
    }
    
    [Serializable]
    public class Icons
    {
        public Image icon;
        public TextMeshProUGUI description;
    }
}