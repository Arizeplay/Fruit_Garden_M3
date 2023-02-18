using System;
using UnityEngine;

public class LevelUnlocker : MonoBehaviour
{
    private bool unlock;

    public void Press()
    {
        if (unlock)
        {
            LockAllLevels();
        }
        else
        {
            UnlockAllLevels();
        }
        
        unlock = !unlock;
    }
    
    public void LockAllLevels()
    {
        foreach (LevelDesign level in LevelAssistant.main.designs)
            CurrentUser.main.UpdateLevelStatistic(level.number, x => {
                if (x.complete) x.bestScore = 0;
            });
        UserUtils.WriteProfileOnDevice(CurrentUser.main);
        LevelMapDisplayer.RefreshAll();
    }
    
    public void UnlockAllLevels()
    {
        foreach (LevelDesign level in LevelAssistant.main.designs)
            CurrentUser.main.UpdateLevelStatistic(level.number, x => {
                if (!x.complete) x.bestScore = 1;
            });
        UserUtils.WriteProfileOnDevice(CurrentUser.main);
        LevelMapDisplayer.RefreshAll();
    }
}
