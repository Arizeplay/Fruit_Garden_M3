using Ninsar.GameEvents.Quests.Collections.Requirements;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Collections
{
    [CreateAssetMenu(menuName = "Create Quest/Quest Item Collection", fileName = "QuestItemCollection", order = 0)]
    public class QuestItemCollection : QuestCollection<QuestRequireItem, QuestUI> { }
}