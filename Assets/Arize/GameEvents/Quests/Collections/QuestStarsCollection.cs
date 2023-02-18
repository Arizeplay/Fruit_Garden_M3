using Ninsar.GameEvents.Quests.Collections.Requirements;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Collections
{
    [CreateAssetMenu(menuName = "Create Quest/Quest Stars Collection", fileName = "QuestStarsCollection", order = 0)]
    public class QuestStarsCollection : QuestCollection<QuestRequireStars, QuestUI>
    {
        // public override void Init()
        // {
        //     base.Init();
        //
        //     var task = SetTotalStars();
        // }
        //
        // private async Task SetTotalStars()
        // {
        //     while (!LevelAssistant.main.DesignsLoaded)
        //     {
        //         await Task.Delay(500);
        //     }
        //     
        //     currentTargetCount = CurrentUser.main.GetTotalStars();
        // }
    }
}