using Ninsar.GameEvents.Quests.Collections.Requirements;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Collections
{
    [CreateAssetMenu(menuName = "Create Quest/Quest Chip Collection", fileName = "QuestChipCollection", order = 0)]
    public class QuestContentCollection : QuestCollection<QuestRequireContent, QuestUI> { }
}