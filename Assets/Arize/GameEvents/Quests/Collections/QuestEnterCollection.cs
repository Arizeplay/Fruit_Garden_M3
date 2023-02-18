using Ninsar.GameEvents.Quests.Collections.Requirements;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Collections
{
    [CreateAssetMenu(menuName = "Create Quest/Quest Enter Collection", fileName = "QuestEnterCollection", order = 0)]
    public class QuestEnterCollection : QuestCollection<QuestEnterCount, QuestUI> { }
}