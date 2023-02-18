using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using Yurowm.EditorCore;

[BerryPanelGroup("Content")]
[BerryPanelTab("Session Director")]
public class SessionDirectorEditor : MetaEditor<SessionDirector>
{

    private SessionDirectorFile _sessionDirectorFile;
    private SessionDirector.Level _editLevel;
    private KeyValuePair<LevelDesign.Difficulty, SessionDirector.LevelDesignSetting> _editSetting;
    
    public override bool Initialize()
    {
        if (!SessionDirector.main) {
            Debug.LogError("SessionDirector is missing");
            return false;
        }
        
        _sessionDirectorFile = new SessionDirectorFile(metaTarget);
        _sessionDirectorFile.Load();

        _editLevel = SessionDirector.main.Levels.First();
        
        return true;
    }
    
    public override SessionDirector FindTarget()
    {
        return SessionDirector.main;
    }
    
    public override void OnGUI()
    {
        using (new GUIHelper.Horizontal())
        {
            using (new GUIHelper.Vertical(Styles.area, GUILayout.Width(100f)))
            {
                EditorGUILayout.LabelField("Session", Styles.centeredLabel);
                EditorGUILayout.Space();
                
                DrawDifficultySequence();
                
                using (new GUIHelper.Vertical(GUILayout.Height(150f)))
                {
                    using (new GUIHelper.Horizontal())
                    {
                        using (new GUIHelper.Vertical(Styles.area))
                        {
                            EditorGUILayout.LabelField("Level Settings", Styles.centeredLabel);
                            EditorGUILayout.Space();

                            if (_editLevel != null)
                            {
                                DrawLevel(_editLevel);
                            }
                        }
                    }
                }
                
                using (new GUIHelper.Vertical(Styles.area, GUILayout.Height(150f)))
                {
                    EditorGUILayout.LabelField("Reducing the level of complexity", Styles.centeredLabel);

                    DrawReducingLevelComplexities();
                }
                
                using (new GUIHelper.Vertical(Styles.area))
                {
                    using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
                    {
                        EditorGUILayout.LabelField("Params", Styles.centeredLabel);
                        EditorGUILayout.Space();
                        SessionDirector.SessionHours = EditorGUILayout.IntField("SessionHours", SessionDirector.SessionHours);
                        SessionDirector.SessionFreeLevels = EditorGUILayout.IntField("SessionFreeLevels", SessionDirector.SessionFreeLevels);
                    }
                }
                
                GUILayout.FlexibleSpace();
            }
            
            using (new GUIHelper.Vertical())
            {
                using (new GUIHelper.Vertical(Styles.area, GUILayout.Height(150f)))
                {
                    EditorGUILayout.Space();
                    
                    DrawGraphic();
                }
                
                using (new GUIHelper.Vertical(Styles.area, GUILayout.Height(200f)))
                {
                    if (SessionDirector.main.SessionEndRewards != null)
                    {
                        EditorGUILayout.LabelField("End Rewards", Styles.centeredLabel);
                        EditorGUILayout.Space();

                        DrawEndRewards();
                    }
                    else
                    {
                        EditorGUILayout.LabelField("End Rewards", Styles.centeredLabel);
                    }
                }
                
                using (new GUIHelper.Vertical(Styles.area,  GUILayout.Height(150f)))
                {
                    if (_editSetting.Value != null)
                    {
                        EditorGUILayout.LabelField("Rewards " + "'" + _editSetting.Key + "'", Styles.centeredLabel);
                        EditorGUILayout.Space();
                                
                        DrawRewards(_editSetting.Value);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Rewards", Styles.centeredLabel);
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }

        if (_sessionDirectorFile is { dirty: true })
        {
            _sessionDirectorFile.Save();
        }
    }

    private void DrawFreeLevels()
    {
        
    }

    private void DrawReducingLevelComplexities()
    {
        using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
        {
            EditorGUILayout.LabelField("Fail Count", GUILayout.Width(80f));

            foreach (var item in SessionDirector.main.ReducingLevelComplexities)
            {
                using (new GUIHelper.Horizontal())
                {
                    item.FailCount = EditorGUILayout.IntField(item.FailCount, GUILayout.Width(80f));
                    item.Downgrade = (SessionDirector.Downgrade)EditorGUILayout.EnumPopup(item.Downgrade);

                    using (new GUIHelper.Color(Color.red))
                    {
                        if (GUILayout.Button("X", GUILayout.Width(30f)))
                        {
                            SessionDirector.main.ReducingLevelComplexities.Remove(item);
                        
                            break;
                        }
                    }
                }
            }

            GUILayout.FlexibleSpace();

            using (new GUIHelper.Horizontal())
            {
                using (new GUIHelper.Color(new Color(0.6f, 1f, 0.6f)))
                    if (GUILayout.Button("Add", GUILayout.Width(60f)))
                    {
                        var newEndReward = new SessionDirector.ReducingLevelComplexity();
                        if (SessionDirector.main.ReducingLevelComplexities.Count > 0)
                        {
                            newEndReward.FailCount = SessionDirector.main.ReducingLevelComplexities.Last().FailCount + 1;
                        }
                        SessionDirector.main.ReducingLevelComplexities.Add(newEndReward);

                        _sessionDirectorFile.dirty = true;
                    }

                using (new GUIHelper.Lock(SessionDirector.main.ReducingLevelComplexities.Count < 1))
                using (new GUIHelper.Color(new Color(1f, 0.6f, 0.6f)))
                    if (GUILayout.Button("Remove", GUILayout.Width(60f)))
                    {
                        SessionDirector.main.ReducingLevelComplexities.RemoveAt(SessionDirector.main.ReducingLevelComplexities.Count - 1);

                        _sessionDirectorFile.dirty = true;
                    }
            }
        }
    }

    private SessionDirector.SessionEndReward _editEndReward = null;

    private void DrawEndRewards()
    {
        using (new GUIHelper.Horizontal())
        {
            using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
            using (new GUIHelper.Vertical(Styles.area, GUILayout.Width(100f)))
            {
                EditorGUILayout.LabelField("Fails", GUILayout.Width(50f));

                foreach (var item in SessionDirector.main.SessionEndRewards)
                {
                    using (new GUIHelper.Horizontal())
                    {
                        item.FailCount = EditorGUILayout.IntField(item.FailCount, GUILayout.Width(30f));

                        if (GUILayout.Button("Edit Reward"))
                        {
                            item.RewardContainer ??= new RewardContainer();
                            
                            _editEndReward = item;
                        }
                    }
                }
        
                GUILayout.FlexibleSpace();
                
                using (new GUIHelper.Horizontal())
                {
                    using (new GUIHelper.Color(new Color(0.6f, 1f, 0.6f)))
                        if (GUILayout.Button("Add", GUILayout.Width(60f)))
                        {
                            var newEndReward = new SessionDirector.SessionEndReward();
                            if (SessionDirector.main.SessionEndRewards.Count > 0)
                            {
                                newEndReward.FailCount = SessionDirector.main.SessionEndRewards.Last().FailCount + 1;
                            }
                            SessionDirector.main.SessionEndRewards.Add(newEndReward);
                    
                            _sessionDirectorFile.dirty = true;
                        }
            
                    using (new GUIHelper.Lock(SessionDirector.main.SessionEndRewards.Count < 1))
                    using (new GUIHelper.Color(new Color(1f, 0.6f, 0.6f)))
                        if (GUILayout.Button("Remove", GUILayout.Width(60f)))
                        {
                            SessionDirector.main.SessionEndRewards.RemoveAt(SessionDirector.main.SessionEndRewards.Count - 1);

                            _sessionDirectorFile.dirty = true;
                        }
                }
            }

            using (new GUIHelper.Vertical(Styles.area))
            {
                if (_editEndReward != null)
                {
                    EditorGUILayout.LabelField("Reward " + "'" + _editEndReward.FailCount + "'");

                    DrawReward(_editEndReward.RewardContainer);
                }
                else
                {
                    EditorGUILayout.LabelField("Reward");
                }
                
                
                GUILayout.FlexibleSpace();
            }
        }
    }

    private void DrawGraphic()
    {
        var defaultColor = Handles.color;
        var rect = GUILayoutUtility.GetLastRect();
        var width = rect.width - 30;
        var startPos = new Vector3(rect.x, rect.y) + new Vector3(10, 25);
        
        Handles.DrawLine(startPos + new Vector3(0, -10), startPos + new Vector3(-5, 5) + new Vector3(0, -10));
        Handles.DrawLine(startPos + new Vector3(0, -10), startPos + new Vector3(5, 5) + new Vector3(0, -10));
        Handles.DrawLine(startPos + new Vector3(-5, 5) + new Vector3(0, -10), startPos + new Vector3(5, 5) + new Vector3(0, -10));
        
        Handles.DrawLine(startPos + new Vector3(0, -10), startPos + new Vector3(0, 110));
        Handles.DrawLine(startPos + new Vector3(0, 110), startPos + new Vector3(width + 10, 110));

        var list = metaTarget.Levels;
        var max = Enum.GetValues(typeof(SessionDirector.Level.SessionDifficulty)).Cast<int>().Max();

        startPos += new Vector3(10, 0);

        Vector3 first = startPos;
        Vector3 last = Vector3.zero;
        
        for (int i = 0; i < list.Count; i++)
        {
            last = new Vector3((float)i / list.Count * width, 100 - (float) list[i].Difficulty / max * 100);

            if (i > 0)
            {
                first = new Vector3((float)(i - 1)  / list.Count * width, 100 - (float) list[i - 1].Difficulty / max * 100);
                Handles.color = Color.gray;
                Handles.DrawDottedLine(startPos + first, startPos + last, 1f);
            }
            
            Handles.color = Color.yellow;

            var buttonSize = _editLevel == list[i] ? new Vector3(20, 20) : new Vector3(15, 15);
            var buttonRect = new Rect(startPos + last - buttonSize / 2, buttonSize);

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("DotFrameDotted"), GUIStyle.none))
            {
                _editLevel = list[i];
            }
                
            Handles.Label(startPos + last + new Vector3(10, -16), list[i].Difficulty.ToString());
        }
        
        Handles.color = defaultColor;
    }
    
    private void DrawRewards(SessionDirector.LevelDesignSetting setting)
    {
        using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
        using (new GUIHelper.Horizontal())
        {
            using (new GUIHelper.Vertical(Styles.area, GUILayout.Width(150f)))
            {
                EditorGUILayout.LabelField("Complete");
                EditorGUILayout.Space();
                DrawReward(setting.Complete);
                
                GUILayout.FlexibleSpace();
            }

            using (new GUIHelper.Vertical(Styles.area, GUILayout.Width(150f)))
            {
                EditorGUILayout.LabelField("Fail");
                EditorGUILayout.Space();
                DrawReward(setting.Fail);
                
                GUILayout.FlexibleSpace();
            }
        }
    }

    private void DrawReward(RewardContainer rewardContainer)
    {
        using (new GUIHelper.Horizontal())
        {
            using (new GUIHelper.Color(new Color(0.6f, 1f, 0.6f)))
                if (GUILayout.Button("Add", GUILayout.Width(60f)))
                {
                    rewardContainer.Items.Add(new RewardItem());
                    
                    _sessionDirectorFile.dirty = true;
                }
            
            using (new GUIHelper.Lock(rewardContainer.Items.Count < 1))
            using (new GUIHelper.Color(new Color(1f, 0.6f, 0.6f)))
                if (GUILayout.Button("Remove", GUILayout.Width(60f)))
                {
                    rewardContainer.Items.RemoveAt(rewardContainer.Items.Count - 1);

                    _sessionDirectorFile.dirty = true;
                }
        }
        
        EditorGUILayout.Space();

        foreach (var item in rewardContainer.Items)
        {
            using (new GUIHelper.Horizontal())
            {
                EditorGUILayout.LabelField("Item", GUILayout.Width(50f));
                item.ItemID = (ItemID)EditorGUILayout.EnumPopup(item.ItemID);
                item.Count = EditorGUILayout.IntField(item.Count);

                using (new GUIHelper.Color(Color.red))
                {
                    if (GUILayout.Button("X"))
                    {
                        rewardContainer.Items.Remove(item);
                        
                        break;
                    }
                }
            }
        }
    }

    private void DrawLevel(SessionDirector.Level level)
    {
        level.Settings ??= new SessionDirector.LevelDesignSettings();
        if (level.Settings.Count == 0)
        {
            level.Settings.CreateDefaultKeys();
        }
        
        level.Difficulty = (SessionDirector.Level.SessionDifficulty)EditorGUILayout.EnumPopup(level.Difficulty);
                    
        EditorGUILayout.Space();
        
        foreach (var complexity in level.Settings)
        {
            using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
            {
                using (new GUIHelper.Horizontal())
                {
                    EditorGUILayout.LabelField(complexity.Key.ToString(), GUILayout.Width(45f));
                    
                    using (new GUIHelper.Color(complexity.Value.operation == SessionDirector.LevelDesignSetting.Operation.Always ? Color.yellow : Color.white))
                        complexity.Value.operation = (SessionDirector.LevelDesignSetting.Operation)EditorGUILayout.EnumPopup(complexity.Value.operation);

                    if (complexity.Value.operation == SessionDirector.LevelDesignSetting.Operation.Always)
                    {
                        foreach (var c in level.Settings)
                        {
                            if (c.Key != complexity.Key)
                            {
                                c.Value.operation = SessionDirector.LevelDesignSetting.Operation.Never;
                            }
                        }
                    }
                    
                    if (complexity.Value.operation == SessionDirector.LevelDesignSetting.Operation.Else)
                    {
                        foreach (var c in level.Settings)
                        {
                            if (c.Key != complexity.Key && c.Value.operation == SessionDirector.LevelDesignSetting.Operation.Else)
                            {
                                c.Value.operation = SessionDirector.LevelDesignSetting.Operation.Never;
                            }
                        }
                    }
                    
                    if (complexity.Value.operation == SessionDirector.LevelDesignSetting.Operation.EGreater)
                    {
                        complexity.Value.Complexities = EditorGUILayout.IntField(complexity.Value.Complexities, GUILayout.Width(30f));
                    }

                    using (new GUIHelper.Lock(complexity.Value.operation == SessionDirector.LevelDesignSetting.Operation.Never))
                        if (GUILayout.Button($"Rewards"))
                            _editSetting = complexity;
                }
            }
        }
    }
    
    private void DrawDifficultySequence()
    {
        var list = metaTarget.Levels;

        if (list.Count < 2)
        {
            for (int i = 0; i < 5; i++)
            {
                var c = new SessionDirector.LevelDesignSettings();
                c.CreateDefaultKeys();
                list.Add(new SessionDirector.Level(SessionDirector.Level.SessionDifficulty.Medium, c));
            }
            
            _sessionDirectorFile.dirty = true;
        }

        for (int i = 0; i < list.Count; i++)
        {
            using (new GUIHelper.Horizontal())
            {
                using (new GUIHelper.Change(() => _sessionDirectorFile.dirty = true))
                {
                    using (_editLevel == list[i] ? new GUIHelper.Color(new Color(1f, 0.9f, .9f) * 2) : i % 2 == 0 ? new GUIHelper.Color(new Color(0.8f, 0.8f, .8f)) : null)
                    {
                        EditorGUILayout.LabelField($"{i}: ", GUILayout.Width(20f));
                        list[i].Difficulty = (SessionDirector.Level.SessionDifficulty)EditorGUILayout.EnumPopup(list[i].Difficulty, GUILayout.Width(100f));
                        if (GUILayout.Button("Edit"))
                        {
                            _editLevel = list[i];
                        }
                    }
                }
            }
        }
        
        GUILayout.Space(10);

        using (new GUIHelper.Horizontal())
        {
            using (new GUIHelper.Lock(list.Count > 10))
            using (new GUIHelper.Color(new Color(0.6f, 1f, 0.6f)))
                if (GUILayout.Button("Add"))
                {
                    var c = new SessionDirector.LevelDesignSettings();
                    c.CreateDefaultKeys();
                    metaTarget.Levels.Add(new SessionDirector.Level(SessionDirector.Level.SessionDifficulty.Medium, c));
                    
                    _sessionDirectorFile.dirty = true;
                }
            
            using (new GUIHelper.Lock(list.Count < 5))
                using (new GUIHelper.Color(new Color(1f, 0.6f, 0.6f)))
                    if (GUILayout.Button("Remove"))
                    {
                        metaTarget.Levels.RemoveAt(metaTarget.Levels.Count - 1);

                        _sessionDirectorFile.dirty = true;
                    }
        }
    }
    
    public class SessionDirectorFile {
        public const string fileName = "SessionDirector.xml";

        public readonly SessionDirector SessionDirector;
        public readonly FileInfo file;

        public SessionDirectorFile(SessionDirector sessionDirector)
        {
            SessionDirector = sessionDirector;
            
            file = new FileInfo(Path.Combine(SessionDirector.directory.FullName, fileName));
        }

        public XElement Xml 
        {
            get 
            {
                if (xml == null) 
                    Load();
                
                return xml;
            }
        }

        XElement xml = null;
        internal bool dirty;

        public void Save()
        {
            var save = false;
            if (xml == null || dirty) 
            {
                xml = SessionDirector.Serialize();
                save = true;
            }

            if (save) 
            {
                if (file.Directory is { Exists: false })
                    file.Directory.Create();
                
                File.WriteAllText(file.FullName, xml.ToString());
            }
            
            dirty = false;
        }

        public void Load() 
        {
            if (!file.Exists) 
                return;
            
            xml = XElement.Parse(File.ReadAllText(file.FullName));

            try
            {
                SessionDirector.Deserialize(xml);
            } 
            catch (Exception e) 
            {
                Debug.LogException(e);
                Debug.Log(file.FullName);
            }
        }

        public override bool Equals(object obj) {
            return obj is SessionDirectorFile directorFile && file.Equals(directorFile.file);
        }

        public override int GetHashCode() {
            return file.GetHashCode();
        }
    }
}
