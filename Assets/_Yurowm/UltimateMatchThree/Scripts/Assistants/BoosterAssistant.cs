using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Yurowm.GameCore;
using UnityEngine;
using Object = UnityEngine.Object;

public enum BoosterLogic
{
    Base,
    GameStart
}

public class BoosterAssistant : MonoBehaviourAssistant<BoosterAssistant>, ILocalized {

    internal PlayingMode? boosterMode = null;

    IBooster currentLogic;

    private List<IBooster> _singleUseBoosters;

    public List<IBooster> SingleUseBoosters => new List<IBooster>(_singleUseBoosters);

    private void Start()
    {
        if (PlayerPrefs.HasKey("START_BOOSTERS"))
        {
            var json = PlayerPrefs.GetString("START_BOOSTERS");
            var list = JsonConvert.DeserializeObject<List<string>>(json);
            
            _singleUseBoosters = Content.GetPrefabList<IBooster>();
            _singleUseBoosters.RemoveAll(x => x is IMultipleUseBooster);
            _singleUseBoosters.RemoveAll(x => !list.Contains(x.name));
        }
        else
        {
            GenerateStartBoosters();
        }
        
        SessionAssistant.OnLevelEnd += OnLevelEnd;
    }
    
    private void OnLevelEnd()
    {
        GenerateStartBoosters();
    }

    public void GenerateStartBoosters()
    {
        _singleUseBoosters = Content.GetPrefabList<IBooster>();
        _singleUseBoosters.RemoveAll(x => x is IMultipleUseBooster);

        var list = new List<IBooster>();
        for (var i = 0; i < 3; i++)
        {
            list.Add(_singleUseBoosters.Where(x => !list.Contains(x)).GetRandom());
        }
        
        _singleUseBoosters = list;

        var boosterNames = _singleUseBoosters.Select(booster => booster.name).ToList();

        var names = new List<string>(boosterNames);
        var json = JsonConvert.SerializeObject(names);
        
        PlayerPrefs.SetString("START_BOOSTERS", json);
        PlayerPrefs.Save();
    }
    
    internal void Cancel() {
        if (currentLogic != null && currentLogic is IMultipleUseBooster)
            (currentLogic as IMultipleUseBooster).Cancel();            
    }

    public IEnumerator Run (IBooster prefab, BoosterLogic logic = BoosterLogic.Base) {
        if (CurrentUser.main[prefab.itemID] < 1) {
            UIAssistant.main.ShowPage("Store");
            yield break;
        }

        BerryAnalytics.Event("Booster Started", "ItemID:" + prefab.itemID);

        currentLogic = Content.Emit(prefab);
        currentLogic.transform.SetParent(FieldAssistant.main.sceneFolder);
        currentLogic.transform.Reset();
        currentLogic.Initialize();

        //BoosterUI.main.SetMessage(currentLogic.FirstMessage());

        var booster = Object.FindObjectOfType<BoosterUI>();
        
        if (booster && prefab.ShowButton)
        {
            booster.Show(prefab.itemID, prefab.ShowExitButton);

            yield return new WaitUntil(() => booster.IsShown);
        }

        switch (logic)
        {
            case BoosterLogic.Base: yield return StartCoroutine(currentLogic.Logic()); break;
            case BoosterLogic.GameStart:
                {
                    GenerateStartBoosters();
                    
                    yield return StartCoroutine(currentLogic.LogicOnGameStart()); break;
                }
        }

        boosterMode = null;
        if (currentLogic is ISingleUseBooster || !(currentLogic as IMultipleUseBooster).IsCanceled()) {
            BerryAnalytics.Event("Booster Used", "ItemID:" + prefab.itemID);
            SessionInfo.current.boostersUsed[prefab.itemID]++;
            CurrentUser.main[prefab.itemID]--;
            UserUtils.WriteProfileOnDevice(CurrentUser.main);
            ItemCounter.RefreshAll();
        }

        if (booster && prefab.ShowButton)
        {
            booster.Hide();
            
            yield return new WaitUntil(() => !booster.IsShown);
        }

        currentLogic = null;
    }

    public IEnumerator RequriedLocalizationKeys() {
        foreach (ILocalized localized in Content.GetPrefabList<IBooster>().Cast<ILocalized>())
            yield return localized.RequriedLocalizationKeys();
    }
}

public abstract class IBooster : ILiveContent, ISounded, IAnimated {
    [HideInInspector]
    public bool localized = false;

    public virtual bool ShowButton => true;
    public virtual bool ShowExitButton => true;

    public const string editLocalizationPattern = "booster/item/{0}/";
    public const string titleLocalizationKey = "booster/item/{0}/title";
    public const string descriptionLocalizationKey = "booster/item/{0}/description";
    public const string firstMessageKeyFormat = "booster/item/{0}/1message";

    internal ContentAnimator animator;
    internal ContentSound sound;

    public override void Initialize() {
        base.Initialize();

        animator = GetComponent<ContentAnimator>();
        sound = GetComponent<ContentSound>();
    }

    public ItemID itemID;
    [HideInInspector]
    public Sprite icon;
    [HideInInspector]
    public string title = "";
    [HideInInspector]
    public string description = "";

    public abstract IEnumerator Logic();
    public abstract IEnumerator LogicOnGameStart();
    public abstract string FirstMessage();

    public string FirstMessageLocalizedKey() {
        return string.Format(firstMessageKeyFormat, itemID);
    }
    
    public virtual IEnumerator RequriedLocalizationKeys() {
        yield return FirstMessageLocalizedKey();
        if (localized) {
            yield return string.Format(titleLocalizationKey, itemID);
            yield return string.Format(descriptionLocalizationKey, itemID);
        }
    }

    public virtual IEnumerator GetAnimationNames() {
        yield break;
    }

    public virtual IEnumerator GetSoundNames() {
        yield break;
    }
}

public abstract class ISingleUseBooster : IBooster {}

public abstract class IMultipleUseBooster : IBooster {
    bool canceled = false;
    public override void Initialize() {
        canceled = false;
    }

    public void Cancel() {
        canceled = true;
    }

    public bool IsCanceled() {
        return canceled;
    }
}