using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Yurowm.GameCore;

public class AppMetricaAnalyticsEvents
{
    private int turns;
    private int turnsLost;
    private bool bonusActive;
    private int stars;
    
    public void RegisterEvents()
    {
        Project.onLevelStart.AddListener(OnLevelStart);
        Project.onLevelFailed.AddListener(OnLevelFailed);
        Project.onLevelClose.AddListener(OnLevelClosed);
        Project.onLevelEnd.AddListener(OnLevelEnd);
        Project.onLevelComplete.AddListener(OnLevelComplete);
        Project.onPlayingModeChanged.AddListener(OnPlayingModeChanged);
    }

    private void OnPlayingModeChanged(PlayingMode mode)
    {
        if (!bonusActive && mode == PlayingMode.Bonus)
        {
            bonusActive = true;
            turnsLost = turns - SessionInfo.current.GetMovesCount();
        }
    }
    
    private void OnLevelStart()
    {
        turns = SessionInfo.current.design.movesCount;
        stars = 0;
        bonusActive = false;
    }

    private void OnLevelClosed()
    {
        TryExecuteLevel();
        SendLevelInfo();
    }

    private void OnLevelFailed()
    {
        TryExecuteLevel();
        SendLevelInfo();
    }
    
    private void OnLevelEnd() { }

    private void OnLevelComplete()
    {
        stars = SessionInfo.current.GetStarCount();
        
        SendLevelInfo();

        var unlockedLevel = CurrentUser.main.level;
        
        IYandexAppMetrica a_m = AppMetrica.Instance;
        YandexAppMetricaUserProfile n_u = new YandexAppMetricaUserProfile();
        n_u.Apply(new YandexAppMetricaNumberAttribute($"unlocked_level").WithValue(unlockedLevel));
        a_m.ReportUserProfile(n_u);
    }

    private void SendLevelInfo()
    {
        var design = SessionInfo.current.design;
        var levelNumber = SessionInfo.current.level;
        var escapedCount = CurrentUser.main.sessions.GetAndAdd(design.number).escapedCount;
        var failedCount = CurrentUser.main.sessions.GetAndAdd(design.number).failedCount;
        var successedCount = CurrentUser.main.sessions.GetAndAdd(design.number).successedCount;

        var movesReportDictionary = MovesReport();
        var spentBoostersDictionary = SpentBoosters();
        
        var levelReport = new Dictionary<string, object>
        {
            {"turns", turns},
            {"turns_lost", turnsLost},
            {"stars", stars},
            {"escaped_count", escapedCount},
            {"failed_count", failedCount},
            {"successed_count", successedCount},
            {"move_seconds", movesReportDictionary},
            {"spent_boosters", spentBoostersDictionary}
        };

        var levelsReport = new Dictionary<string, object>();
        levelsReport.Add($"Level {levelNumber} [{SessionInfo.current.design.difficulty}]", levelReport);
        
        AppMetrica.Instance.ReportEvent($"End Level", levelsReport);
    }

    private Dictionary<string, object> MovesReport()
    {
        var movesReport = new Dictionary<string, object>();
        var moveTimer = SessionInfo.current.moveTimer.list;

        for (var index = 0; index < moveTimer.Count; index++)
        {
            var time = moveTimer[index];
            movesReport.Add($"move_{index + 1}", time);
        }

        return movesReport;
    }

    private void TryExecuteLevel()
    {
        var lastLevel = CurrentUser.main.level;
        var openLevelNum = SessionInfo.current.level;
        
        var key = $"try_execute_level_{openLevelNum}";

        if (lastLevel == openLevelNum)
        {
            var hasKey = PlayerPrefs.HasKey(key);

            var attemptNumber = hasKey ? PlayerPrefs.GetInt(key) + 1 : 1;
            
            PlayerPrefs.SetInt($"Level {openLevelNum}", attemptNumber);
            
            var report = new Dictionary<string, object>();
            report.Add($"Level {openLevelNum} [{SessionInfo.current.design.difficulty}]", attemptNumber);
        
            AppMetrica.Instance.ReportEvent($"try_execute_level", report);
        }
    }

    private Dictionary<string, object> SpentBoosters()
    {
        var boostersUsed = SessionInfo.current.boostersUsed;
        var report = new Dictionary<string, object>();

        foreach (var itemID in boostersUsed.usedItems)
        {
            var itemsLost = boostersUsed[itemID];
            var itemsCount = CurrentUser.main[itemID];
            report.Add($"{itemID}_lost", itemsLost);
            report.Add($"{itemID}", itemsCount);
        }

        return report;
        
        //AppMetrica.Instance.ReportEvent("spent_boosters", report);
    }
}
