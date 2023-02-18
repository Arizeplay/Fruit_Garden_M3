using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum MaskLibraryComparison {Page, Field, PlayingMode, Difficulty};
public enum SimpleComparisonOperator {Equal, NotEqual};
public enum Operators {Less, LEqual, Equal, Greater, GEqual};
public enum FieldComparisonSource {FieldOpened, FieldClosed};
public enum PageComparisonSource {CurrentPage, Page};
public enum PlayingModeComparisonSource {CurrentPlayMode, PlayMode};

public class MaskLibrary : MonoBehaviour
{
    public Operators operators;
    public MaskLibraryComparison type;
    public FieldComparisonSource fieldMustBe;
    
    [FormerlySerializedAs("pageMustBe")] public SimpleComparisonOperator simpleMustBe;

    public LevelDesign.Difficulty difficulty;
    
    public PageValue PageA;
    public PageValue PageB;
    
    public PlayModeValue PlayModeA;
    public PlayModeValue PlayModeB;


    public bool allChild = true;
    public List<GameObject> targets = new List<GameObject>();
    
    public ComparisonAction action = ComparisonAction.Deactivate;

    void Start () {
		Refresh ();
        ItemCounter.refresh += Refresh;
        UIAssistant.onShowPage += OnShowPage;
	}
    
    private void OnShowPage(UIAssistant.Page obj)
    {
        Refresh ();
    }

    void OnEnable () {
		Refresh ();
	}

	public void Refresh () {
		bool result = false;

        switch (type)
        {
            case MaskLibraryComparison.Page:
                string pageA = GetPageValue(PageA);
                string pageB = GetPageValue(PageB);
                switch (simpleMustBe) {
                    case SimpleComparisonOperator.Equal: result = pageA == pageB; break;
                    case SimpleComparisonOperator.NotEqual: result = pageA != pageB; break;
                }
                break;
            case MaskLibraryComparison.Field:
                switch (fieldMustBe) {
                    case FieldComparisonSource.FieldOpened: result = ! (bool) FieldAssistant.main.sceneFolder; break;
                    case FieldComparisonSource.FieldClosed: result = (bool) FieldAssistant.main.sceneFolder; break;
                }
                break;
            case MaskLibraryComparison.PlayingMode:
            {                
                PlayingMode playingModeA = GetPlayModeValue(PlayModeA);
                PlayingMode playingModeB = GetPlayModeValue(PlayModeB);
                switch (simpleMustBe)
                {
                    case SimpleComparisonOperator.Equal: result = playingModeA == playingModeB; break;
                    case SimpleComparisonOperator.NotEqual: result = playingModeA != playingModeB; break;
                }

                break;
            }
            case MaskLibraryComparison.Difficulty:
                var selectedDifficulty = SessionDirector.GetDifficulty(LevelDesign.selected);
                switch (operators)
                {
                    case Operators.Less:
                        result = selectedDifficulty < difficulty;
                        break;
                    case Operators.LEqual:
                        result = selectedDifficulty <= difficulty;
                        break;
                    case Operators.Equal:
                        result = selectedDifficulty == difficulty;
                        break;
                    case Operators.Greater:
                        result = selectedDifficulty > difficulty;
                        break;
                    case Operators.GEqual:
                        result = selectedDifficulty >= difficulty;
                        break;
                }
                
                break;
        }
        
        AllTargets(result);
	}

    PlayingMode GetPlayModeValue(PlayModeValue v)
    {
        if (SessionInfo.current == null || SessionInfo.current.rule == null) return PlayingMode.NA;
        
        switch (v.source) {
            case PlayingModeComparisonSource.CurrentPlayMode: return SessionInfo.current.rule.GetMode();
            case PlayingModeComparisonSource.PlayMode: return v.playMode;
        }
        return PlayingMode.NA;
    }
    
    string GetPageValue(PageValue v) {

        switch (v.source) {
            case PageComparisonSource.CurrentPage: return UIAssistant.main.GetCurrentPage().name;
            case PageComparisonSource.Page: return v.page;
        }
        return null;
    }

    void AllTargets (bool v) {
        if (allChild)
            foreach (Transform t in transform)
                Action(t.gameObject, v);

        foreach (GameObject t in targets)
            Action(t, v);
    }

    void Action(GameObject go, bool v) {
        if (action == ComparisonAction.Deactivate) {
            go.SetActive(v);
            return;
        }
        if (action == ComparisonAction.LockButton) {
            go.GetComponent<Button>().interactable = v;
            return;
        }
    }

    [Serializable]
    public class PageValue {
        public PageComparisonSource source = PageComparisonSource.Page;
        public string page = "";
    }
    
    [Serializable]
    public class PlayModeValue {
        public PlayingModeComparisonSource source = PlayingModeComparisonSource.PlayMode;
        public PlayingMode playMode;
    }
}