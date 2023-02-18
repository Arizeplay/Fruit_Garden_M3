using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Yurowm.GameCore;

public class LevelMapAssistant : MonoBehaviourAssistant<LevelMapAssistant> 
{
    public float mapSize = 1200f;
    public MapLocation[] locationPrefabs;

    private List<MapLocationInfo> _locationsInfo;
    List<MapLocationInfo> locationsInfo
    {
        get
        {
            if (_locationsInfo.IsNullOrEmpty())
            {
                _locationsInfo = new List<MapLocationInfo>();
                
                var order = 0;
                var levelCounter = 0;
                foreach (var prefab in locationPrefabs) 
                {
                    var info = new MapLocationInfo
                    {
                        prefab = prefab,
                        order = order++,
                        firstLevel = levelCounter + 1,
                        levelCount = prefab.GetLevelCount()
                    };
                    
                    levelCounter += info.levelCount;
                    info.lastLevel = levelCounter;
                    
                    _locationsInfo.Add(info);
                }
            }

            return _locationsInfo;
        }
    }

    internal int GetLocationCount()
    {
        return locationsInfo.Count;
    }

    internal MapLocationInfo GetLocationInfo(int order)
    {
        return locationsInfo.Find(x => x.order == order);
    }

    internal MapLocation CreateNewLocation(int order) 
    {
        MapLocationInfo info = GetLocationInfo(order);
        if (info == null)
            return null;
        MapLocation result = Instantiate(info.prefab).GetComponent<MapLocation>();
        result.order = order;
        result.name = info.prefab.name;
        return result;
    }

    internal MapLocation CreateNewLocationByLevelNumber(int level)
    {
        MapLocationInfo info = locationsInfo.Find(x => x.firstLevel <= level && x.lastLevel >= level);
        
        if (info == null)
            info = level <= 0 ? locationsInfo.GetMin(x => x.firstLevel) : locationsInfo.GetMax(x => x.lastLevel);

        if (info == null)
            info = locationsInfo.First();
        
        if (info == null)
            return null;
        
        MapLocation result = Instantiate(info.prefab).GetComponent<MapLocation>();
        result.order = info.order;
        result.name = info.prefab.name;
        return result;
    }
}

public class MapLocationInfo 
{
    public MapLocation prefab;
    public int levelCount;
    public int firstLevel;
    public int lastLevel;
    public int order;
}
