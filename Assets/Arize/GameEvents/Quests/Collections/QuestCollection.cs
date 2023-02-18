using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Ninsar.GameEvents.Quests.Collections.Requirements;
using Ninsar.GameEvents.Quests.Rewards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ninsar.GameEvents.Quests.Collections
{
    public abstract class QuestCollection<TReq, TComponent> : Quest where TReq : QuestRequirement where TComponent : QuestComponent
    {
        [SerializeField]
        private string _saveName;

        [SerializeField]
        private TReq _requirement;

        [SerializeField]
        protected List<Rarity> _rarities;
        
        [SerializeField]
        private List<QuestLevel> _questLevels;

        [SerializeField]
        private TComponent _questUIComponent;

        [SerializeField]
        private QuestPinger _questPinger;
        
        [SerializeField]
        private bool _autoComplete;
        
        private int _totalTargets;
        private int _lastTotalTargets;
        private int _currentLevelIndex;

        public override void ResetStats()
        {
            var firstRandom = Random.Range(0f, 1f);
            _totalTargets = Random.Range(firstRandom > 0.5f ? 100 : 0, firstRandom > 0.5f ? 1500 : 0);
            _lastTotalTargets = 0;
            _currentLevelIndex = 0;

            Update();
        }

        public override GameObject Instantiate(Transform parent) => _questUIComponent.Instantiate(this, parent);

        public override void Init()
        {
            _requirement.RegisterEvents();
            _requirement.onItemAdd += OnItemAdd;
            _requirement.onItemCountChanged += OnItemCountChanged;
        }
        
        private void OnItemAdd(int count)
        {
            _totalTargets += count;
            
            Update();
        }
        
        private void OnItemCountChanged(int count)
        {
            _totalTargets = count;
            
            Update();
        }

        public override void Update()
        {
            _questUIComponent.Update();

            Save();
            
            if (IsReady() && _autoComplete)
            {
                CompleteIfReady();
            }
            else if (IsReady())
            {
                QuestsReady();
            }
        }

        public override void CompleteIfReady()
        {
            if (!IsReady()) return;
            
            var dictionary = new Dictionary<Reward.Item, int>();

            if (CurrentLevel.Rewards != null)
            {
                foreach (var reward in CurrentLevel.Rewards)
                {
                    var (item, count) = reward.Apply();
                    if (item == null) continue;

                    if (dictionary.ContainsKey(item))
                    {
                        dictionary[item] += count;
                    }
                    else
                    {
                        dictionary.Add(item, count);
                    }
                }
            }

            if (_currentLevelIndex < _questLevels.Count)
            {
                _lastTotalTargets += CurrentLevel.TargetItems;
                _currentLevelIndex++;
            }

            if (dictionary.Count > 0) GettedRewardItems(dictionary);
                
            QuestsComplete();

            Update();
        }
        
        public override bool IsReady() => !IsComplete() && CurrentTargets >= CurrentLevel.TargetItems;

        public override bool IsPinging() => IsReady() || (_questPinger && _questPinger.IsPinging());
        
        public override bool IsComplete() => _currentLevelIndex >= _questLevels.Count;

        public override int CurrentTargets => (_totalTargets - _lastTotalTargets) > CurrentLevel.TargetItems ? CurrentLevel.TargetItems : (_totalTargets - _lastTotalTargets) ;
        
        public override QuestLevel CurrentLevel => _currentLevelIndex < _questLevels.Count ? _questLevels[_currentLevelIndex] : _questLevels.Last();

        public override float Progress
        {
            get
            {
                if (_questLevels.Count == 0) return 0;
                
                return (float) _currentLevelIndex / _questLevels.Count;
            }
        }

        public override RarityType GetRarity() => Rarity.GetRarity(_rarities, _currentLevelIndex);
        
        public override int GetStars(out int maxCount) => Rarity.GetPosition(_rarities, _currentLevelIndex, out maxCount);
        
        public class QuestData
        {
            public int CurrentLevel;
            public int TotalTargets;
            public int LastTotalTargets;
        }
        
        public override void Load()
        {
            try
            {
                using var reader = new StreamReader(Path.Combine(Application.persistentDataPath, $"{_saveName}.json"));
            
                var json = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<QuestData>(json);
                if (data == null) return;
                
                _currentLevelIndex = data.CurrentLevel;
                _totalTargets = data.TotalTargets;
                _lastTotalTargets = data.LastTotalTargets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public override void Save()
        {
            var data = new QuestData
            {
                CurrentLevel = _currentLevelIndex,
                TotalTargets = _totalTargets,
                LastTotalTargets = _lastTotalTargets,
            };

            using var writer = new StreamWriter(Path.Combine(Application.persistentDataPath, $"{_saveName}.json"));
            var json = JsonConvert.SerializeObject(data);

            writer.Write(json);
        }
    }
}