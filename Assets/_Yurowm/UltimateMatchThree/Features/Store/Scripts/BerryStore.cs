using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yurowm.GameCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif
#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
#endif

public class BerryStore : MonoBehaviourAssistant<BerryStore>, ILocalized
    #if UNITY_PURCHASING
    , IStoreListener
    #endif
    {

    #if UNITY_EDITOR
    public TreeViewState storeListState = new TreeViewState();
    #endif

    #if UNITY_PURCHASING
    static IStoreController store_controller;
    static IExtensionProvider store_extension_provider;
    #endif
    [HideInInspector]
    public UnityEvent onPurchase;
    [HideInInspector]
    public UnityEvent fillStores;

    //[HideInInspector]
    public List<Item> items = new List<Item>();
    //[HideInInspector]
    public List<Group> groups = new List<Group>();

    public List<IAP> iaps = new List<IAP>();
    public IAP GetIAPByID(string id) {
        return iaps.Find(x => x.id == id);
    }
    
	public Dictionary<string, string> marketItemPrices = new Dictionary<string, string>();
	public Dictionary<string, decimal> marketItemPricesDecimals = new Dictionary<string, decimal>();

    Action iap_reward = delegate{};
    public string storeItemContent = "";
    public string storeGroupContent = "";

    void Start() {
        #if UNITY_PURCHASING
        if (store_controller == null)
            InitializePurchasing();
        #endif
        fillStores.Invoke();
    }

    public void FillTheStore(LayoutGroup container, List<string> groupsIDs)
    {
        var groupsList = new List<Group>();

        foreach (var id in groupsIDs)
        {
            var group = groups.FirstOrDefault(g => g.id == id);
            if (group != null)
                groupsList.Add(group);
            else
                Debug.LogWarning($"Group with id:{group.id} not found!");
        }

        FillTheStore(container, groupsList);
    }
    
    private void FillTheStore(LayoutGroup container, List<Group> groups)
    {
        if (!container) return;
        
        BerryStoreItem itemPrefab = Content.GetPrefab<BerryStoreItem>(storeItemContent);
        if (!itemPrefab) return;

        BerryStoreGroup groupPrefab = Content.GetPrefab<BerryStoreGroup>(storeGroupContent);
        if (!groupPrefab) return;

#if !UNITY_PURCHASING
        items.RemoveAll(item => item.purchaseType == Item.PurchaseType.IAP);
#endif
        List<BerryStoreItem> berryStoreItems = new List<BerryStoreItem>();
        foreach (Group group in groups) {
            List<Item> groupContent = items.Where(x => x.group == group).ToList();

            if (groupContent.Count == 0) continue;

            BerryStoreGroup groupInstance = Instantiate(groupPrefab);
            groupInstance.group = group;
            groupInstance.name = "Group " + group.id;
            groupInstance.transform.SetParent(container.transform);
            groupInstance.transform.localScale = Vector3.one;
            foreach (Item item in groupContent) {
                BerryStoreItem itemInstance = Instantiate(itemPrefab);
                berryStoreItems.Add(itemInstance);
                itemInstance.item = item;
                itemInstance.name = "Item " + item.id;
                itemInstance.transform.SetParent(container.transform);
                itemInstance.transform.localScale = Vector3.one;
            }
        }
    }

    void InitializePurchasing() {
        #if UNITY_PURCHASING
        if (IsInitialized())
            return;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (IAP iap in iaps)
            builder.AddProduct(iap.id, ProductType.Consumable, new IDs() {
                { iap.sku, AppleAppStore.Name},
                { iap.sku, GooglePlay.Name}});

        UnityPurchasing.Initialize(this, builder);        
        #endif
    }

    #if UNITY_PURCHASING
    bool IsInitialized() {
        return store_controller != null && store_extension_provider != null;
    }
    #endif
        

    void UpdatePrices() {
        #if UNITY_PURCHASING
        if (store_controller == null)
            return;

        foreach (IAP iap in iaps) {
            Product product = store_controller.products.WithID(iap.id);
            if (product == null)
                continue;

            marketItemPrices.Add(iap.id, product.metadata.localizedPriceString);
            marketItemPricesDecimals.Add(iap.id, product.metadata.localizedPrice);
        }
        #endif
    }
    
    // Function item purchase
    public void PurchaseIAP (Item item, string id, List<ItemPack> packs, UnityEvent action) {
        BerryAnalytics.Event("IAP Purchase Button Pressed", "PurchaseID:" + id);
        IAP iap = GetIAPByID(id); 
        Debug.Log(marketItemPrices[iap.id]);
        if (iap != null) {
            iap_reward = () => {
                ItemPack.Purchase(packs, action);
                UserUtils.WriteProfileOnDevice(CurrentUser.main);

                ItemCounter.RefreshAll();
                
                var p = FindObjectOfType<AppMetrica>();
                var json = $"{{\"IAP Purchase\":\"{iap.sku}\"}}";
                p.SendEvent("IAP Purchase", json);
                
                IYandexAppMetrica a_m = AppMetrica.Instance;
                YandexAppMetricaUserProfile n_u = new YandexAppMetricaUserProfile();
                n_u.Apply(new YandexAppMetricaCounterAttribute("USD").WithDelta((float)marketItemPricesDecimals[iap.id]));
                n_u.Apply(new YandexAppMetricaCounterAttribute("IAP Purchased").WithDelta(1));
                a_m.ReportUserProfile(n_u);
                
                BerryAnalytics.Event("IAP Purchased",
                    "PurchaseID:" + id,
                    "SKU:" + iap.sku,
                    "Price:" + marketItemPrices[iap.id]);
            };
            BuyProductID(iap.id);
        }
    }

    //TODO: Purchasing
    void BuyProductID(string id) {
        #if UNITY_PURCHASING
        try {
            if (IsInitialized() && Project.main.md5Valid) {
                Product product = store_controller.products.WithID(id);
                if (product != null && product.availableToPurchase) {
                    Debug.Log("Purchasing product asychronously:'" + product.definition.id + "'");// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    store_controller.InitiatePurchase(product);
                } else
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
        } else
                Debug.Log("BuyProductID FAIL. Not initialized.");
        }
        catch (Exception e) {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
        #endif
    }

#region Unity IAP Implementation
    
    #if UNITY_PURCHASING
    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.LogError("Unity IAP Purchasing initializing is failed: " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
        iap_reward.Invoke();
        iap_reward = null;
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {

        AppMetrica appmetric = GameObject.Find("AppMetrica").GetComponent<AppMetrica>();

        string json = $"{{\"Purchase Failed\":\"{i.definition.id}\"}}";

        appmetric.SendEvent("Purchase Failed", json);
        Debug.Log("Purchase failed: " + p);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("Unity IAP Purchasing is initialized");

        store_controller = controller;
        store_extension_provider = extensions;

        UpdatePrices();
    }
    #endif
#endregion

    public IEnumerator RequriedLocalizationKeys() {
        foreach (Item item in items) {
            if (item.localized) {
                yield return item.localization_Name;
                yield return item.localization_Description;
            }
        }
        foreach (Group group in groups) {
            if (group.localized) {
                yield return group.localization_Name;
                yield return group.localization_Description;
            }
        }
    }

    [Serializable]
    public class Stuff {
        public string id = "";

        public bool localized = false;
        public string Name = "";
        public string Descrition = "";
        public string localization_Name {
            get {
                return string.Format("item/{0}/name", id);
            }
        }
        public string localization_Description {
            get {
                return string.Format("item/{0}/description", id);
            }
        }
        public string localization_Icon(int iconIndex) {
            return string.Format("item/{0}/icon{1}", id, iconIndex);
        }

        virtual public Stuff Clone() {
            Stuff stuff = (Stuff) MemberwiseClone();
            stuff.id += "-clone";
            stuff.instanceID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            return stuff;
        }

        public override string ToString() {
            return id == "" ? "-" : id;
        }

        public int instanceID = 0;

        public override bool Equals(object obj) {
            if (obj is Stuff)
                return instanceID == ((Stuff) obj).instanceID;
            return false;
        }

        public override int GetHashCode() {
            return instanceID;
        }

        public static bool operator ==(Stuff a, Stuff b) {
            if ((object) a == null)
                return (object) b == null;
            return a.Equals(b);
        }

        public static bool operator !=(Stuff a, Stuff b) {
            if ((object) a == null)
                return (object) b != null;
            return !a.Equals(b);
        }
    }

    [Serializable]
    public class Item : Stuff {
        public const int MinIcons = 1;
        public const int MaxIcons = 4;
        public List<Icon> icons = new List<Icon>(MinIcons);
        [Serializable]
        public class Icon
        {
            public Sprite icon;
            public string description;
            public bool localized;
        }
        public Group group = null;

        public enum PurchaseType {IAP, RewardedVideo, CoinCurrency}
        public PurchaseType purchaseType = PurchaseType.CoinCurrency;

        public string iap = "";
        public int cost = 1;

        public List<ItemPack> pack = new List<ItemPack>();

        [SerializeField]
        public UnityEvent onPurchase = new UnityEvent();
        
        public bool alwaysAvaliable = true;
        public Comparer avaliableWhen = new Comparer();

        public bool showInfoButton;
        public bool showTape;
        public string tapeDescription;
        
        public static Item New(Group group) {
            Item item = new Item();
            item.instanceID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            item.group = group;
            return item;
        }

        public override Stuff Clone() {
            Item result = (Item) base.Clone();
            result.pack = result.pack.Select(x => x.Clone()).ToList();
            return result;
        }
    }

    [Serializable]
    public class Group : Stuff
    {
        public bool IsItemGroup;
        public ItemID ItemGroup;
        public static Group New() {
            Group group = new Group();
            group.instanceID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            return group;
        }
    }

    [Serializable]
    public class IAP {
        public string id
        {
            get
            {
#if UNITY_EDITOR || UNITY_ANDROID
                return androidId;
#elif UNITY_IOS
                return iosId;
#endif
            }
        }
        public string androidId;
        public string iosId;
        public string sku;
    }

    [Serializable]
    public class ItemPack {
        public enum Type
        {
            Items, Moves
        }

        public Type type;
        public ItemID itemID;
        public int itemCount;
        public int movesCount;
        
        public ItemPack Clone() {
            return new ItemPack() {
                itemID = itemID,
                itemCount = itemCount,
                movesCount = movesCount
            };
        }

        public static void Purchase(Item item, List<ItemPack> packs, UnityEvent action, int cost) {
            if (cost >= 0 && CurrentUser.main[ItemID.coin] >= cost) {
                CurrentUser.main[ItemID.coin] -= cost;
                Purchase(packs, action);
            }
        }

        public static void Purchase(List<ItemPack> packs, UnityEvent action) {
            if (packs != null)
                foreach (ItemPack pack in packs)
                    switch (pack.type)
                    {
                        case Type.Items:
                            CurrentUser.main[pack.itemID] += pack.itemCount; break;
                        case Type.Moves:
                            for (int i = 0; i < pack.movesCount; i++)
                                SessionInfo.current.AddMove();
                            break;
                    }
    
            if (action != null)
                action.Invoke();
            UserUtils.WriteProfileOnDevice(CurrentUser.main);
            ItemCounter.RefreshAll();
            AudioAssistant.Shot("Buy");
        }
    }
    }

