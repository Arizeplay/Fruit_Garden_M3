using UnityEngine;
using System.Collections;
using Yurowm.GameCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Linq;
using _Yurowm.UltimateMatchThree.Features.Multiplyer;

[Flags]
public enum ReactionType {
    Move = 1 << 0,
    Match = 1 << 1
}
public enum PlayingMode {
        NA = 0,
        Wait = 1,
        Matching = 2,
        Gravity = 3,
        Reaction = 4,
        TargetChecking = 5,
        Shuffle = 6,
        ResourceChecking = 7,
        Bonus = 8,
        YouWin = 9,
        YouLose = 10,
        Booster = 11,
        Almost = 12
    }

public abstract class LevelRule : ILiveContent {
    internal int matchDate = 0;
    int startMatchingDate = 0;
    bool targetIsReached = false;

    public List<ScoreBonus> scoreBonus = new List<ScoreBonus>();

    internal ContentSound sound;

    [ContentSelector]
    public SlotRenderer slotRenderer;

    public string soundTrack = "None";

    PlayingMode mode;

    public override void Initialize() {
        base.Initialize();
        sound = GetComponent<ContentSound>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas)
            canvas.worldCamera = GameCamera.camera;
    }

    public PlayingMode GetMode() {
        if (mode == PlayingMode.Booster && BoosterAssistant.main.boosterMode.HasValue)
            return BoosterAssistant.main.boosterMode.Value;
        return mode;
    }

    public void SetMode(PlayingMode newMode) {
        if (GetMode() == PlayingMode.Wait) {
            mode = newMode;
            Project.onPlayingModeChanged.Invoke(mode);
        } else
            Debug.LogError("You can change playing mode only when current mode is Wait");
    }

    void Update() {
        DebugPanel.Log("Session", "Mode", GetMode());
    }

    List<Reaction> reactions = new List<Reaction>();
    List<Reactor> reactors = new List<Reactor>();
    int eachMoveReaction = -1;

    internal IEnumerator _Session() {
        reactions = Reaction.GetReactions();

        UIAssistant.main.SetPanelVisible("Loading", true);
        UIAssistant.main.FreezPanel("Loading", true);
        while (CPanel.uiAnimation > 0) yield return 0;

        yield return StartCoroutine(FieldAssistant.main.CreateField());

        if (!IsThereAnyMoves()) {
            Debug.Log("There is no any moves");
            Shuffle(true);
        }
        Project.onLevelCreate.Invoke();

        UIAssistant.main.ShowPage("Field");
        GameCamera.main.SetPosition(-5, 5);

        while (CPanel.uiAnimation > 0) yield return 0;
        

        UIAssistant.main.FreezPanel("Loading", false);
        UIAssistant.main.SetPanelVisible("Loading", false);
        while (CPanel.uiAnimation > 0) yield return 0;

        mode = PlayingMode.Gravity;
        
        if (BoosterInfo.toRelease.Count > 0)
        {
            SessionInfo.current.startBooster = BoosterInfo.toRelease.Last().booster;
            SessionInfo.current.boosterSelected = BoosterInfo.toRelease.Last().boosterSelected;
        }
        
        yield return 0;

        SessionInfo.current.DeepIndexUpdate(true);

        Project.onLevelStart.Invoke();

        yield return 0;
        GameCamera.main.ShowField();
        while (GameCamera.main.animate)
            yield return 0;

        SessionInfo.current.isPlaying = true;
        SessionInfo.current.Save(true);

        IEnumerator session = SessionPipeline(mode);
        while (session != null && session.MoveNext()) {
            yield return session.Current;
            if (session.Current is PlayingMode) {
                PlayingMode modeResult = (PlayingMode) session.Current;

                if (modeResult != PlayingMode.NA) {
                    mode = modeResult;
                    Project.onPlayingModeChanged.Invoke(mode);
                    session = SessionPipeline(mode);
                }
            }
            else continue;
        }

        SessionInfo.current.isPlaying = false;
        FieldAssistant.main.RemoveField();

        mode = PlayingMode.NA;
    }

    IEnumerator SessionPipeline(PlayingMode playingMode) {
        IEnumerator logic = PreSessionModes(playingMode);
        while (logic.MoveNext()) yield return logic.Current;
        logic = SessionModes(playingMode);
        while (logic.MoveNext()) yield return logic.Current;
        logic = PostSessionModes(playingMode);
        while (logic.MoveNext()) yield return logic.Current;
    }

    IEnumerator PreSessionModes(PlayingMode playingMode) {
        switch (playingMode) {
            case PlayingMode.Wait: {
                    Project.onStartWaitingNextMove.Invoke();
                    SessionInfo.current.moveTimer.oldMoveTime = Time.time;
                    eachMoveReaction = SessionInfo.current.GetMovesCount() - 1;
                    reactors = Reactor.GenerateRectionList(this);
                } break;
            //case PlayingMode.Gravity: {
            //        Slot.all.Values.ForEach(x => x.GravityReaction());
            //    } break;
            case PlayingMode.Matching: {
                    if (SessionInfo.current.IsCompleteTarget())
                        yield return PlayingMode.TargetChecking;
                    startMatchingDate = matchDate;
                } break;
            case PlayingMode.Reaction: {
                    ReactionType type = ReactionType.Match;
                    if (eachMoveReaction == SessionInfo.current.GetMovesCount())
                        type = type | ReactionType.Move;
                    reactors.RemoveAll(x => x.reaction != null && (type & x.reaction.GetReactionType()) == 0);

                    if (reactors.Count > 0) {
                        ReactionResult result = ReactionResult.Complete;
                        Reactor reactor = reactors.First();
                        while (reactor.logic.MoveNext()) {
                            object current = reactor.logic.Current;

                            if (current is ReactionResult)
                                result = (ReactionResult) current;

                            yield return current;
                        }
                        switch (result) {
                            case ReactionResult.Complete: {
                                    reactors.RemoveAt(0);
                                    yield return PlayingMode.Reaction;
                                } break;
                            case ReactionResult.Gravity: reactors.RemoveAt(0); break;
                            case ReactionResult.GravityAndRepeate: reactor.Reset(); break;
                        }
                        yield return PlayingMode.Gravity;
                    }

                    yield return PlayingMode.Shuffle;
                } break;
            case PlayingMode.TargetChecking: {
                    if (SessionInfo.current.IsFaildedTarget())
                        yield return PlayingMode.YouLose;
                    if (!SessionInfo.current.IsCompleteTarget()) {
                        if (SessionInfo.current.DeepIndexUpdate(false))
                            yield return PlayingMode.Matching;
                        yield return PlayingMode.ResourceChecking;
                    }
                    Project.onAllTargetsIsReached.Invoke();
                    SessionInfo.current.Save(true);
                } break;
            case PlayingMode.ResourceChecking: {
                    if (SessionInfo.current.GetMovesCount() > 0)
                        yield return PlayingMode.Wait;
                    yield return PlayingMode.Almost;
                } break;
            case PlayingMode.Almost: {
                    UIAssistant.main.ShowPage("Almost");
                    yield return new WaitWithDelay(() => CPanel.uiAnimation == 0, 0.1f);
                    yield return new WaitWithDelay(() => UIAssistant.main.GetCurrentPage().name == "Field", 0.1f);
                    if (SessionInfo.current.GetMovesCount() <= 0)
                        yield return PlayingMode.YouLose;
                    yield return PlayingMode.Wait;
                } break;
            case PlayingMode.Shuffle: {
                    if (!IsThereAnyMoves()) {
                        if (!Shuffle() && !SessionInfo.current.IsCompleteTarget())
                        {
                            yield return PlayingMode.YouLose;
                        }
                            
                        yield return new WaitForSeconds(.1f);
                    }
                    SessionInfo.current.Save();
                    yield return PlayingMode.TargetChecking;
                } break;
            case PlayingMode.Bonus: {
                    SessionInfo.current.Save();
                    if (!bonus)
                    {
                        bonus = Content.Emit(SessionInfo.current.design.bonus) as CompleteBonus;
                        if (!bonus.IsComplete())
                            UIAssistant.main.ShowPage("SkipBonus");
                    }

                    if (bonus && !bonus.IsComplete()) {
                        yield return StartCoroutine(bonus.Logic());
                        yield return PlayingMode.Gravity;
                    } 

                    yield return PlayingMode.YouWin;
                } break;
            case PlayingMode.YouWin: {
                yield return StartCoroutine(YouWin());
            } break;
            case PlayingMode.YouLose: {
                yield return StartCoroutine(YouLose());
            } break;
            case PlayingMode.Booster: {
                    Debug.Log(SessionInfo.current.startBooster);
                    Debug.Log(SessionInfo.current.mBooster);
                    if (SessionInfo.current.startBooster)
                    {
                        yield return StartCoroutine(BoosterAssistant.main.Run(SessionInfo.current.startBooster, BoosterLogic.GameStart));
                        
                        var remove = BoosterInfo.toRelease.FirstOrDefault(x => x.booster.itemID == SessionInfo.current.startBooster.itemID);
                        if (remove != null && BoosterInfo.toRelease.Contains(remove))
                        {
                            BoosterInfo.toRelease.Remove(remove);
                        }

                        SessionInfo.current.startBooster = BoosterInfo.toRelease.Count > 0 ? BoosterInfo.toRelease.Last().booster : null;
                        
                        yield return PlayingMode.Gravity;
                    }
                    else if (SessionInfo.current.mBooster) {
                        yield return StartCoroutine(BoosterAssistant.main.Run(SessionInfo.current.mBooster));
                        
                        var remove = BoosterInfo.toRelease.FirstOrDefault(x => x.booster.itemID == SessionInfo.current.mBooster.itemID);
                        if (remove != null && BoosterInfo.toRelease.Contains(remove))
                        {
                            BoosterInfo.toRelease.Remove(remove);
                        }
                        
                        SessionInfo.current.mBooster = BoosterInfo.toRelease.Count > 0 ? BoosterInfo.toRelease.Last().booster : null;
                        
                        yield return PlayingMode.Gravity;
                    }
                    
                    SessionInfo.current.startBooster = null;
                    SessionInfo.current.mBooster = null;
                   
                    
                    yield return PlayingMode.Matching;
                } break;
        }
        yield break;
    }

    IEnumerator PostSessionModes(PlayingMode playingMode) {
        switch (playingMode) {
            case PlayingMode.Wait: {
                    while (GetMode() == PlayingMode.Wait)
                    {
                        if ((SessionInfo.current.mBooster != null || SessionInfo.current.startBooster != null) && ILevelExtension.currentExtensions.All(x => x.GetType() != typeof(Dialogue)))
                        {
                            break;
                        }
                        
                        yield return 0;
                    }
                    
                    if (ILevelExtension.currentExtensions.Any(x => x.GetType() == typeof(Dialogue)))
                    {
                        yield return GetMode();
                    }

                    if (SessionInfo.current.mBooster != null || SessionInfo.current.startBooster != null)
                        yield return PlayingMode.Booster;
                    else yield return GetMode();
                } break;
            case PlayingMode.Matching: {
                    bool isBlocked = false;
                    float delay = 0f;
                    while (delay <= .1f) {
                        if (ISlotContent.gravityLockers.Count == 0)
                            delay += Time.deltaTime;
                        else {
                            delay = 0f;
                            isBlocked = true;
                            ISlotContent.gravityLockers.RemoveAll(c => c == null);
                        }
                             
                        yield return 0;
                    }

                    yield return isBlocked || startMatchingDate != matchDate ? PlayingMode.Gravity : PlayingMode.Reaction;
                } break;
            case PlayingMode.Gravity: {
                    yield return 0;
                    Func<bool> isFalling = () => !Contains<SlotGenerator>(x => !x.slot.chip && !x.slot.block)
                        && !Contains<IChip>(x => x.falling);

                    yield return new WaitWithDelay(isFalling, 0.1f);

                    yield return PlayingMode.Matching;
                } break;
            case PlayingMode.TargetChecking: {
                    if (!targetIsReached && SessionInfo.current.IsCompleteTarget()) {
                        targetIsReached = true;
                        yield return new WaitForSeconds(0.1f);
                    }

                    yield return PlayingMode.Bonus;
                } break;
        }
        yield break;
    }

    internal virtual IEnumerator SessionModes(PlayingMode playingMode) {
        yield break;
    }
    internal abstract void Control(bool isBegan, bool isPress, bool IsOverUI, Vector2 point);

    internal abstract bool IsThereAnyMoves();
    internal abstract bool IsThereAnySolutions();

    public bool Shuffle(bool immediate = false) {    
        List<IChip> chips = Slot.allActive.Values.Where(x => x.GetCurrentContent() is IChip && x.color.IsPhysicalColor() && x.chip is IShuffled)
            .Select(x => x.chip).ToList();
        List<Slot> slots = chips.Select(x => x.slot).ToList();

        for (int i = 0; i < 100; i++) {
            chips.ForEach(x => x.ParentRemove());
            chips = chips.Unsort().ToList();
            for (int s = 0; s < slots.Count; s++)
                slots[s].chip = chips[s];
            if (!IsThereAnySolutions() && IsThereAnyMoves()) {
                if (immediate) chips.ForEach(x => x.transform.localPosition = Vector3.zero);
                return true;
            }
        }

        return false;
    }

    CompleteBonus bonus = null;

    public class Reactor {
        public Reaction reaction;

        Func<IEnumerator> provider;

        IEnumerator _logic;
        public IEnumerator logic {
            get {
                return _logic;
            }
        }

        public Reactor(Func<IEnumerator> provider, Reaction reaction = null) {
            this.reaction = reaction;
            this.provider = provider;
            _logic = provider.Invoke();
        }

        public void Reset() {
            _logic = provider.Invoke();
        }

        public static List<Reactor> GenerateRectionList(LevelRule rules) {
            List<Reactor> result = new List<Reactor>();

            result.AddRange(rules.reactions.Select(x => new Reactor(x.React, x)).ToList());
            
            result.AddRange(SessionInfo.current.GetReactions().Select(x => new Reactor(x.GetReactorLogic())).ToList());

            return result;
        }
    }

    internal void Run() {
        StartCoroutine(_Session());
        StartCoroutine(ControlAssistant.main.ControlRoutine(this));
    }

    public enum GenerationType { AllUnkonwn, EvenSlots, OddSlots, EvenFinal }
    public abstract ItemColor[] ColorGeneration(ItemColor[] colors, Dictionary<Side, ItemColor> nears, GenerationType type);

    public void Complete() {
        if (mode == PlayingMode.Wait)
            mode = PlayingMode.YouWin;
    }

    public void Fail() {
        if (mode == PlayingMode.Wait)
            mode = PlayingMode.YouLose;
    }

    private IEnumerator YouWin()
    {
        yield return new WaitWithDelay(() => CPanel.uiAnimation == 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        // UIAssistant.main.ShowPage("TargetIsReached");
        // AudioAssistant.Shot("TargetIsReached");
        // yield return new WaitWithDelay(() => CPanel.uiAnimation == 0, 0.1f);
        UIAssistant.main.ShowPage("Field");
        
        CurrentUser.main.sessionCount++;
        CurrentUser.main.UpdateLevelStatistic(SessionInfo.current.level, s => {
            if (s.bestScore < SessionInfo.current.GetScore())
                s.bestScore = SessionInfo.current.GetScore();
            s.totalCount ++;
            s.successedCount ++;
        });
        UserUtils.WriteProfileOnDevice(CurrentUser.main);
        BerryAnalytics.Event("Level Session Complete", SessionInfo.current.SessionEventKeys());

        Project.onLevelComplete.Invoke();
        Project.onLevelEnd.Invoke();

        yield return StartCoroutine(GameCamera.main.HideFieldRoutine());

        
        Project.ClearDelegates();
        SessionInfo.RemoveSavedSession();

        AudioAssistant.Shot("YouWin");
        PlayerPrefs.SetInt("FirstPass", 1);
                    
        yield return 0;
        
        while (CPanel.uiAnimation != 0) yield return 0;
        
        yield return 0;

        UIAssistant.main.ShowPage("YouWin");
        Debug.Log("you won!");
        
        if (!PlayerPrefs.HasKey("RateUs") && CurrentUser.main.level == 3)
        {
            yield return new WaitWithDelay(() => CPanel.uiAnimation == 0, 0.1f);
            UIAssistant.main.ShowPage("RateUs");
        }
    }

    private IEnumerator YouLose()
    {
        CurrentUser.main.sessionCount++;
        CurrentUser.main.UpdateLevelStatistic(SessionInfo.current.level, s => {
            s.totalCount++;
            s.failedCount++;
        });
        UserUtils.WriteProfileOnDevice(CurrentUser.main);
        BerryAnalytics.Event("Level Session Failed", SessionInfo.current.SessionEventKeys());

        Project.onLevelFailed.Invoke();
        Project.onLevelEnd.Invoke();

        yield return StartCoroutine(GameCamera.main.HideFieldRoutine());

        Project.ClearDelegates();
        SessionInfo.RemoveSavedSession();

        CurrentUser.main.lifeSystem.BurnLife();

        AudioAssistant.Shot("YouLose");

        yield return 0;

        while (CPanel.uiAnimation != 0) yield return 0;

        yield return 0;
                    
        //Advertising.instance.ShowAds(AdType.Regular,null, true);
        UIAssistant.main.ShowPage("YouLose");
    }

    private IEnumerator CompleteCoroutine()
    {
        yield return StartCoroutine(YouWin());
        
        SessionInfo.current.isPlaying = false;
        FieldAssistant.main.RemoveField();

        mode = PlayingMode.NA;
    }
    
    public bool KillBonus()
    {
        if (bonus)
        {
            bonus.Kill();
            StopAllCoroutines();
            StartCoroutine(CompleteCoroutine());
            
            return true;
        }

        return false;
    }

    public virtual void Serialize(XElement xml) {}
    public virtual void Deserialize(XElement xml) {}
}