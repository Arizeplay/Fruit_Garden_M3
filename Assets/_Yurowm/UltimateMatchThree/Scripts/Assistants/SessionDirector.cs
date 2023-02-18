using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Yurowm.GameCore;

[Serializable]
public class SessionIceCreamRewards
{
    [Serializable]
    public class DifficultyReward
    {
        public LevelDesign.Difficulty Difficulty;
        public int IceCreamCount;
    }

    public List<DifficultyReward> DifficultyRewards;

    public int GetIceCreamCount(LevelDesign.Difficulty difficulty)
    {
        var reward = DifficultyRewards.FirstOrDefault(x => x.Difficulty == difficulty);
        if (reward == null)
        {
            return 0;
        }
        
        return reward.IceCreamCount;
    }
}

public class SessionDirector : MonoBehaviourAssistant<SessionDirector>
{
    public List<Level> Levels = new List<Level>();
    public List<SessionEndReward> SessionEndRewards = new List<SessionEndReward>();
    public List<ReducingLevelComplexity> ReducingLevelComplexities = new List<ReducingLevelComplexity>();
    public SessionIceCreamRewards SessionIceCreamRewards;
    
    public static int SessionHours = 5;
    public static int SessionFreeLevels = 0;
    
    private static string _settingsRaw;
    private static DirectoryInfo _directory;
    
    private static Level _currentLevel;
    
    private static int _sessionPosition;
    private static int _sessionFails;
    
    public static DirectoryInfo directory 
    {
        get 
        {
            if (_directory == null || !_directory.Exists) _directory = null;
            if (_directory == null) {
                _directory = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources"));
                if (!_directory.Exists) _directory.Create();
                _directory = new DirectoryInfo(Path.Combine(_directory.FullName, "Settings"));
                if (!_directory.Exists) _directory.Create();
            }
            return _directory;
        }
    }
    
    private void Awake()
    {
        #if UNITY_EDITOR
        var files = directory.GetFiles();
        var sessionDirectorFile = files.First(x => x.Name == "SessionDirector.xml");
        _settingsRaw = System.IO.File.ReadAllText(sessionDirectorFile.FullName);
        #else
        _settingsRaw = Resources.LoadAll("Settings").Cast<TextAsset>().First(x => x.name == "SessionDirector").text;
        #endif

        StartCoroutine(LoadSettings());
        
        SessionAssistant.OnRunLevelDesign += OnLevelRun;
    }
    
    IEnumerator LoadSettings()
    {
        Deserialize(XElement.Parse(_settingsRaw));

        yield return 0;
        
        if (!CurrentUser.main.inventory.ContainsKey(ItemID.complexity))
        {
            CurrentUser.main.inventory.Add(ItemID.complexity, 0);
        }
        
        LoadSave();
    }
    
    private void Update()
    {
        DebugPanel.Log("Session Timer","Session", CurrentUser.main.sessionTask.GetTimer());
        DebugPanel.Log("User Complexities","Session", CurrentUser.main[ItemID.complexity]);
        DebugPanel.Log("Free Levels","Session", $"{SessionFreeLevels}");
        
        if (SessionInfo.current is { isPlaying: true })
        {
            DebugPanel.Log("Is Free Level","Session",$"{SessionInfo.current.design.number <= SessionFreeLevels}, current level::{SessionInfo.current.design.number}");
            DebugPanel.Log("Current Level Design Difficulty","Session", SessionInfo.current.design.difficulty);
        }
        
        if (_currentLevel != null)
        {
            DebugPanel.Log("Current Difficulty","Session", _currentLevel.Difficulty.ToString());
        }
        
        var nextIndex = _sessionPosition + 1;
        if (nextIndex >= Levels.Count)
        {
            nextIndex = 0;
        }
        
        DebugPanel.Log("Next Difficulty","Session",Levels[nextIndex].Difficulty.ToString());
        
        if (SessionInfo.current is { isPlaying: true })
        {
            DebugPanel.Log("Design Difficulty","Session", GetDifficulty(SessionInfo.current.design));
        }
    }

    public static LevelDesign.Difficulty GetDifficulty(LevelDesign levelDesign)
    {
        var difficulty = LevelDesign.Difficulty.Normal;

        if (levelDesign.number > SessionFreeLevels && 
            (!CurrentUser.main.sessions.ContainsKey(levelDesign.number) || CurrentUser.main.sessions.ContainsKey(levelDesign.number) && CurrentUser.main.sessions[levelDesign.number].bestScore > levelDesign.firstStarScore))
        {
            if (_currentLevel != null)
            {
                difficulty = _currentLevel.GetSetting().Key;
            }
        }

        if (CurrentUser.main.sessions.ContainsKey(levelDesign.number))
        {
            foreach (var reducingLevelComplexity in main.ReducingLevelComplexities)
            {
                difficulty = reducingLevelComplexity.Difficulty(CurrentUser.main.sessions[levelDesign.number].failedCount, difficulty);
            }
        }

        return difficulty;
    }
    
    private void OnLevelRun(LevelDesign levelDesign)
    {
        if (CurrentUser.main.sessionTask.IsAvailable() && levelDesign.number > SessionFreeLevels)
            CurrentUser.main.sessionTask.SetSession(SessionHours);
        
        if (levelDesign.number > SessionFreeLevels && 
            (!CurrentUser.main.sessions.ContainsKey(levelDesign.number) ||
                CurrentUser.main.sessions.ContainsKey(levelDesign.number) && 
                CurrentUser.main.sessions[levelDesign.number].bestScore < levelDesign.firstStarScore))
        {
            Project.onLevelComplete.AddListener(() => OnLevelComplete(levelDesign));
            Project.onLevelFailed.AddListener(() => OnLevelFailed(levelDesign));
        }
    }

    private void OnLevelComplete(LevelDesign levelDesign)
    {
        _currentLevel = Levels[_sessionPosition];
        _currentLevel.GetSetting().Value.Complete.Obtain();

        CurrentUser.main[ItemID.IceCream] += SessionIceCreamRewards.GetIceCreamCount(levelDesign.difficulty);
        
        _sessionPosition += 1;

        if (_sessionPosition >= Levels.Count)
        {
            SessionEnd();
            
            _sessionPosition = 0;
        }

        _currentLevel = Levels[_sessionPosition];

        PlayerPrefs.SetInt("SessionDirector_Position", _sessionPosition);
        PlayerPrefs.Save();
    }
    
    private void OnLevelFailed(LevelDesign levelDesign)
    {
        _currentLevel = Levels[_sessionPosition];
        _currentLevel.GetSetting().Value.Fail.Obtain();

        _sessionFails += 1;
        
        PlayerPrefs.SetInt("SessionDirector_Fails", _sessionFails);
        PlayerPrefs.Save();
    }

    private void SessionEnd()
    {
        foreach (var sessionEndReward in SessionEndRewards)
        {
            var reward = sessionEndReward.GetRewardOrNull(_sessionFails);
            if (reward != null)
            {
                reward.Obtain();
            }
        }
    }

    public void ResetSession()
    {
        _sessionPosition = 0;
        _sessionFails = 0;

        _currentLevel = Levels[_sessionPosition];

        PlayerPrefs.SetInt("SessionDirector_Position", _sessionPosition);
        PlayerPrefs.SetInt("SessionDirector_Fails", _sessionFails);
        PlayerPrefs.Save();
    }

    public void StartSession()
    {
        if (CurrentUser.main.sessionTask.IsAvailable())
        {
            CurrentUser.main.sessionTask.SetSession(SessionHours);
            
            ResetSession();
        }
        else
        {
            LoadSave();
        }
    }

    private void LoadSave()
    {
        _sessionPosition = PlayerPrefs.GetInt("SessionDirector_Position", 0);
        _sessionFails = PlayerPrefs.GetInt("SessionDirector_Fails", 0);

        _currentLevel = Levels[_sessionPosition];
    }
    
    public XElement Serialize()
    {
        XElement xml = new XElement("SessionDirector");
        
        xml.Add(new XAttribute("SessionHours", SessionHours));
        xml.Add(new XAttribute("SessionFreeLevels", SessionFreeLevels));
        
        var levelsXml = new XElement("levels");
        xml.Add(levelsXml);
        foreach (var c in Levels) levelsXml.Add(c.Serialize("level"));
        
        var sessionEndRewardsXml = new XElement("session_end_rewards");
        xml.Add(sessionEndRewardsXml);
        foreach (var r in SessionEndRewards) sessionEndRewardsXml.Add(r.Serialize("session_end_reward"));
        
        var reducingLevelComplexitiesXml = new XElement("reducing_level_complexities");
        xml.Add(reducingLevelComplexitiesXml);
        foreach (var r in ReducingLevelComplexities) reducingLevelComplexitiesXml.Add(r.Serialize("reducing_level_complexity"));
        
        return xml;
    }
    
    public void Deserialize(XElement xml)
    {
        var sessionHours = xml.Attribute("SessionHours");
        if (sessionHours != null)
        {
            SessionHours = int.Parse(sessionHours.Value);
        }
        
        var sessionFreeLevels = xml.Attribute("SessionFreeLevels");
        if (sessionFreeLevels != null)
        {
            SessionFreeLevels = int.Parse(sessionFreeLevels.Value);
        }
        
        Levels = new List<Level>();
        
        var levelsXml = xml.Element("levels");
        if (levelsXml != null)
        {
            foreach (var e in levelsXml.Elements())
                Levels.Add(Level.Deserialize(e));
        }

        if (Levels.Count < 1)
        {
            Levels.Add(new Level(Level.SessionDifficulty.Medium, new LevelDesignSettings()));
        }

        SessionEndRewards = new List<SessionEndReward>();
        
        var sessionEndRewardsXml = xml.Element("session_end_rewards");
        if (sessionEndRewardsXml != null)
        {
            foreach (var e in sessionEndRewardsXml.Elements())
                SessionEndRewards.Add(SessionEndReward.Deserialize(e));
        }
        
        ReducingLevelComplexities = new List<ReducingLevelComplexity>();
        
        var reducingLevelComplexitiesXml = xml.Element("reducing_level_complexities");
        if (reducingLevelComplexitiesXml != null)
        {
            foreach (var e in reducingLevelComplexitiesXml.Elements())
                ReducingLevelComplexities.Add(ReducingLevelComplexity.Deserialize(e));
        }
    }
    
    public enum Downgrade
    {
        Down, Easy
    }

    public class ReducingLevelComplexity
    {
        public int FailCount = 0;
        public Downgrade Downgrade;

        public LevelDesign.Difficulty Difficulty(int failCount, LevelDesign.Difficulty difficulty)
        {
            if (FailCount > failCount)
            {
                return difficulty;
            }
            
            switch (Downgrade)
            {
                case Downgrade.Down:
                    {
                        var newDifficulty = difficulty - 1;
                        if (newDifficulty < 0)
                        {
                            newDifficulty = 0;
                        }
            
                        return newDifficulty;
                    }
                case Downgrade.Easy:
                    return LevelDesign.Difficulty.Easy;
                default:
                    return LevelDesign.Difficulty.Easy;
            }
        }
        
        public XElement Serialize(string name)
        {
            XElement xml = new XElement(name);
        
            xml.Add(new XAttribute("fail_count", FailCount));
            xml.Add(new XAttribute("downgrade", (int)Downgrade));
        
            return xml;
        }
    
        public static ReducingLevelComplexity Deserialize(XElement xml)
        {
            var failCount = int.Parse(xml.Attribute("fail_count").Value);
            var downgrade = (Downgrade)int.Parse(xml.Attribute("downgrade").Value);

            return new ReducingLevelComplexity
            {
                FailCount = failCount,
                Downgrade = downgrade
            };
        }
    }

    public class SessionEndReward
    {
        public int FailCount = 0;
        public RewardContainer RewardContainer = new RewardContainer();

        public RewardContainer GetRewardOrNull(int failCount)
        {
            if (FailCount == failCount)
            {
                return RewardContainer;
            }

            return null;
        }
        
        public XElement Serialize(string name)
        {
            XElement xml = new XElement(name);
        
            xml.Add(new XAttribute("fail_count", FailCount));
            xml.Add(RewardContainer.Serialize("reward"));
        
            return xml;
        }
    
        public static SessionEndReward Deserialize(XElement xml)
        {
            var failCount = int.Parse(xml.Attribute("fail_count").Value);
            var reward = RewardContainer.Deserialize(xml.Element("reward"));

            return new SessionEndReward
            {
                FailCount = failCount,
                RewardContainer = reward
            };
        }
    }
    
    public class LevelDesignSetting
    {
        public enum Operation { Never, Always, EGreater, Else }
        
        public Operation operation;
        public int Complexities;

        public RewardContainer Complete = new RewardContainer();
        public RewardContainer Fail = new RewardContainer();

        public XElement Serialize(string name) 
        {
            XElement xml = new XElement(name);
            xml.Add(new XAttribute("operation", (int)operation));
            xml.Add(new XAttribute("complexities", Complexities));
            xml.Add(Complete.Serialize("win"));
            xml.Add(Fail.Serialize("fail"));

            return xml;
        }

        public static LevelDesignSetting Deserialize(XElement xml)
        {
            return new LevelDesignSetting
            {
                operation = (Operation)int.Parse(xml.Attribute("operation").Value),
                Complexities = int.Parse(xml.Attribute("complexities").Value),
                Complete = RewardContainer.Deserialize(xml.Element("win")),
                Fail = RewardContainer.Deserialize(xml.Element("fail")),
            };
        }

        public LevelDesignSetting Clone() 
        {
            return (LevelDesignSetting) MemberwiseClone();
        }
    }

    public class LevelDesignSettings : Dictionary<LevelDesign.Difficulty, LevelDesignSetting>
    {
        public void CreateDefaultKeys()
        {
            Clear();
            
            foreach (LevelDesign.Difficulty difficulty in Enum.GetValues(typeof(LevelDesign.Difficulty)))
            {
                Add(difficulty, new LevelDesignSetting());
            } 
        }

        public XElement Serialize(string name)
        {
            XElement complexities = new XElement(name);
            foreach (var key in Keys)
            {
                var complexityElement = new XElement("complexity");
                complexityElement.Add(new XAttribute("difficulty", (int) key));
                complexityElement.Add(this[key].Serialize("complexity_operation"));
                complexities.Add(complexityElement);
            }

            return complexities;
        }
        
        public static LevelDesignSettings Deserialize(XElement xml)
        {
            var complexities = new LevelDesignSettings();
            foreach (var element in xml.Elements())
            {
                var key = (LevelDesign.Difficulty)int.Parse(element.Attribute("difficulty").Value);
                var value = LevelDesignSetting.Deserialize(element.Element(("complexity_operation")));
                
                complexities.Add(key, value);
            }

            return complexities;
        }
    }
    
    [Serializable]
    public class Level 
    {
        public enum SessionDifficulty {Easy, Medium, Hard}
        public SessionDifficulty Difficulty;

        public LevelDesignSettings Settings;
        
        public Level(SessionDifficulty difficulty, LevelDesignSettings settings) 
        {
            Difficulty = difficulty;
            Settings = settings;
        }

        public KeyValuePair<LevelDesign.Difficulty, LevelDesignSetting> GetSetting()
        {
            var alwaysSetting = Settings.FirstOrDefault(s => s.Value.operation == LevelDesignSetting.Operation.Always);
            if (alwaysSetting.Value != null)
            {
                return alwaysSetting;
            }

            var complexity = CurrentUser.main[ItemID.complexity];
            var selected = new KeyValuePair<LevelDesign.Difficulty, LevelDesignSetting>(LevelDesign.Difficulty.Normal, null);
            foreach (var setting in Settings)
            {
                if (setting.Value.operation != LevelDesignSetting.Operation.EGreater) continue;
                if (complexity < setting.Value.Complexities) continue;
                    
                if (selected.Value != null)
                {
                    if (setting.Value.Complexities >= selected.Value.Complexities)
                    {
                        selected = setting;
                    }
                }
                else
                {
                    selected = setting;
                }
            }
            
            if (selected.Value == null)
            {
                var elseSetting = Settings.FirstOrDefault(s => s.Value.operation == LevelDesignSetting.Operation.Else);
                if (elseSetting.Value != null)
                {
                    return elseSetting;
                }
            }
            
            return selected;
        }

        public XElement Serialize(string name) 
        {
            XElement xml = new XElement(name);
            xml.Add(new XAttribute("difficulty", (int)Difficulty));
            
            Settings ??= new LevelDesignSettings();
            if (Settings.Keys.Count == 0)
            {
                Settings.CreateDefaultKeys();
            }
            
            xml.Add(Settings.Serialize("complexities"));
            
            return xml;
        }

        public static Level Deserialize(XElement xml)
        {
            var difficulty = (SessionDifficulty)int.Parse(xml.Attribute("difficulty").Value);

            LevelDesignSettings complexities = null;
            var complexitiesXml = xml.Element("complexities");
            complexities = complexitiesXml != null ? LevelDesignSettings.Deserialize(complexitiesXml) : new LevelDesignSettings();
            if (complexities.Keys.Count == 0)
            {
                complexities.CreateDefaultKeys();
            }
            
            return new Level(difficulty, complexities);
        }
    }
}