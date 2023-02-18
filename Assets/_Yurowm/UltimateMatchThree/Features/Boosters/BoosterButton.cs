using UnityEngine;
using System.Collections;
using System.Linq;
using Ninsar.Store;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BoosterButton : MonoBehaviour
{
    Button button;
    public Text counter;
    public GameObject nullCounter;
    public Image icon;
    public GameObject outline;
    public GameObject lockeOutline;
    public Text title;
    public Text description;

    IBooster prefab;

    public void SetPrefab(IBooster prefab) {
        this.prefab = prefab;
        Refresh();
    }

    void Awake () {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        ItemCounter.refresh += Refresh;
        BoosterInfo.refresh += Refresh;
        Refresh();
	}

    void OnEnable() {
        Refresh();
    }

    void OnDisable() {
        ItemCounter.refresh -= Refresh;
        BoosterInfo.refresh -= Refresh;
    }

    void Refresh() {
        if (prefab != null) {
            if (ProfileAssistant.main && CurrentUser.main != null) {
                if (nullCounter) nullCounter.SetActive(CurrentUser.main[prefab.itemID] <= 0);
                counter.gameObject.SetActive(!nullCounter || CurrentUser.main[prefab.itemID] > 0);
                counter.text = CurrentUser.main[prefab.itemID].ToString();
            }

            if (outline)
                outline.SetActive(BoosterInfo.toRelease.Any(x => x.booster.itemID == prefab.itemID));
            if (lockeOutline)
            {
                lockeOutline.SetActive(StartLevelButtonWithAd.BoostersSelected.Contains(prefab));
            }
            if (title)
                title.text = prefab.localized ? LocalizationAssistant.main[string.Format(IBooster.titleLocalizationKey, prefab.itemID)] : prefab.title;
            if (description)
                description.text = prefab.localized ? LocalizationAssistant.main[string.Format(IBooster.descriptionLocalizationKey, prefab.itemID)] : prefab.description;
        }
    }

	void OnClick ()
    {
        if (StartLevelButtonWithAd.BoostersSelected.Contains(prefab))
        {
            return;
        }
        
        if (BoosterInfo.toRelease.Any(x => x.booster.itemID == prefab.itemID))
        {
            var remove = BoosterInfo.toRelease.FirstOrDefault(x => x.booster.itemID == prefab.itemID);
            if (remove != null && BoosterInfo.toRelease.Contains(remove))
            {
                BoosterInfo.toRelease.Remove(remove);
                Debug.Log("remove");
            }
            
            Refresh();
            
            return;
        }

        if (FieldAssistant.main.sceneFolder != null)
        {
            if (!SessionInfo.current.settings.sBoostersEnable && prefab is ISingleUseBooster) return;
            if (!SessionInfo.current.settings.mBoostersEnable && prefab is IMultipleUseBooster) return;
            
            BerryAnalytics.Event("Booster Button Pressed", "ItemID:" + prefab.itemID);
            if (CurrentUser.main[prefab.itemID] > 0) {
                SessionInfo.current.mBooster = prefab;
                SessionInfo.current.boosterSelected = true;
            }
            else
            {
                StoreItem.selected = prefab.itemID;
                UIAssistant.main.ShowPage("StoreItem");
            }
        }
        else
        {
            if (CurrentUser.main[prefab.itemID] > 0) {
                BoosterInfo.toRelease.Add( new BoosterInfo(prefab, true));
                Debug.Log(prefab.name + " selected");
            }
            else
            {
                StoreItem.selected = prefab.itemID;
                UIAssistant.main.ShowPage("StoreItem");
            }
        }

        Refresh();
    }
}
