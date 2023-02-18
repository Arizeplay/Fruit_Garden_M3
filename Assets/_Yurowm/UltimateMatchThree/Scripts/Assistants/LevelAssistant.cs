using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
using System.Xml.Linq;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
#endif

public class LevelAssistant : MonoBehaviourAssistant<LevelAssistant>
{
    #if UNITY_EDITOR
    [HideInInspector]
    public TreeViewState levelListState = new ();
    [HideInInspector]
    public bool levelListShown = true;
    [HideInInspector]
    public float[] splitterH = new float[2] { 200, 300 };
    [HideInInspector]
    public List<TreeFolder> folders = new ();
    [HideInInspector]
    public Vector2 levelListScroll = Vector2.zero;
    [HideInInspector]
    public Vector2 parametersScroll = Vector2.zero;
    #endif

    public List<LevelDesign> designs = new ();

    public static string[] levelsRaw;

    public bool DesignsLoaded { get; private set; }
    
    void Awake() 
    {
        DesignsLoaded = false;
        
        #if UNITY_EDITOR
        string assetsPath = System.IO.Path.Combine(Application.dataPath, "Resources");
        assetsPath = System.IO.Path.Combine(assetsPath, "Levels");
        levelsRaw = new System.IO.DirectoryInfo(assetsPath)
            .GetFiles()
            .Where(f => f.Extension == ".xml")
            .Select(f => System.IO.File.ReadAllText(f.FullName))
            .ToArray();
        #else
        levelsRaw = Resources.LoadAll("Levels").Cast<TextAsset>().Select(a => a.text).ToArray();
        #endif
        
        StartCoroutine(LoadLevels());
    }

    IEnumerator LoadLevels()
    {
        yield return new WaitForEndOfFrame();

        var designs = new List<LevelDesign>();
        var access = new DelayedAccess(1f / 20);
        for (var index = 0; index < levelsRaw.Length; index++)
        {
            var raw = levelsRaw[index];
            var design = LevelDesign.DeserializeHalf(XElement.Parse(raw));
            design.rawDesignIndex = index;
            designs.Add(design);
            if (access.GetAccess()) yield return 0;
        }
        designs.Sort((a, b) => a.number.CompareTo(b.number));
        this.designs = designs;
        UpdateNumbers();
        
        DesignsLoaded = true;

        #if UNITY_EDITOR
        if (PlayerPrefs.GetInt("TestLevel") != 0) 
        {
            TestLevel(PlayerPrefs.GetInt("TestLevel"), (LevelDesign.Difficulty)PlayerPrefs.GetInt("TestLevelDifficulty"));
            SessionInfo.RemoveSavedSession();
            yield break;
        }
        #endif

        SessionInfo savedSession = SessionInfo.Load();
        if (savedSession != null)
            SessionAssistant.main.StartSession(savedSession);
        else
            UIAssistant.main.ShowPage("LevelList");
    }

    public static LevelDesign LoadLevel(int rawDesignIndex)
    {
        var raw = levelsRaw[rawDesignIndex];
        
        return LevelDesign.DeserializeFull(XElement.Parse(raw));
    }

    public static void SelectDesign(int levelNumber) 
    {
        if (CPanel.uiAnimation > 0)
            return;

        Project.randomSeed = UnityEngine.Random.Range(9, 999);

        LevelDesign.selected = LoadLevel(main.GetDesign(levelNumber).rawDesignIndex);
        if (LevelDesign.selected != null)
            UIAssistant.main.ShowPage(CurrentUser.main.lifeSystem.HasLife() ? "LevelSelectedPopup" : "NotEnoughLifes");
        else
            UIAssistant.main.ShowPage("MoreLevels");
    }

    public LevelDesign GetDesign(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > designs.Count)
            return null;
        return designs[levelNumber - 1];
    }

    public void UpdateNumbers() 
    {
        for (int i = 0; i < designs.Count; i++) designs[i].number = i + 1;
    }

    public static void TestLevel(int number, LevelDesign.Difficulty difficulty) 
    {
        LevelDesign.selected = LoadLevel(main.GetDesign(number).rawDesignIndex);
        SessionAssistant.main.StartSession(LevelDesign.selected, difficulty);
        PlayerPrefs.DeleteKey("TestLevel");
    }
}