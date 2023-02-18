using System.Linq;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using UnityEditor;
using UnityEngine;
using Yurowm.EditorCore;
using Yurowm.GameCore;
[BerryPanelGroup("Content")]
[BerryPanelTab("Ad Random Booster")]
public class AdRandomEditor : MetaEditor<AdRandomRewarder> {
    private AdRandomRewarder.Reward current = null;

    private ISingleUseBooster[] _singleUseBoosters;
    public override AdRandomRewarder FindTarget() {
        return AdRandomRewarder.main;
    }

    public override bool Initialize() {
        if (metaTarget == null)
            return false;

        _singleUseBoosters = Content.GetPrefabList<ISingleUseBooster>().ToArray();
        
        return true;
    }

    public override void OnGUI() {
        float sum = metaTarget.Rewards.Sum(x => x.weight);

        foreach (AdRandomRewarder.Reward reward in metaTarget.Rewards) {
            using (new GUIHelper.Horizontal(GUILayout.Width(300))) {
                using (new GUIHelper.BackgroundColor(Color.red))
                    if (GUILayout.Button("X", EditorStyles.miniButtonLeft, GUILayout.Width(20))) {
                        metaTarget.Rewards.Remove(reward);
                        GUI.FocusControl("");
                        break;
                    }

                if (GUILayout.Button(string.Format("{0} ({1:F2}%)", reward.booster ? reward.booster.itemID : ItemID.coin, 100f * reward.weight / sum, GUILayout.Width(20)), EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true))) {
                    current = current == reward ? null : reward;
                    GUI.FocusControl("");
                }
            }
            if (reward == current) 
                using (new GUIHelper.Vertical(Styles.area, GUILayout.Width(300)))
                {
                    reward.selected = EditorGUILayout.Popup("Booster", reward.selected, _singleUseBoosters.Select(x => x.itemID.ToString()).ToArray());
                    reward.count = Mathf.Max(1, EditorGUILayout.IntField("Count", reward.count));
                    reward.weight = Mathf.Max(0f, EditorGUILayout.FloatField("Weight", reward.weight));

                    if (reward.selected < _singleUseBoosters.Length)
                    {
                        reward.booster = _singleUseBoosters[reward.selected];
                        var icon = ItemIcons.main.GetIconOrNull(reward.booster.itemID);
                        reward.icon = (Sprite) EditorGUILayout.ObjectField("Icon", icon ? icon : reward.icon, typeof (Sprite), false);
                    }
                }
        }

        using (new GUIHelper.BackgroundColor(Color.green))
            if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(40))) {
                AdRandomRewarder.Reward newReward = null;
                if (current != null) {
                    newReward = new AdRandomRewarder.Reward(current.booster, current.count, current.weight);
                    newReward.icon = current.icon;
                } else 
                    newReward = new AdRandomRewarder.Reward(Content.GetPrefab<ISingleUseBooster>(), 1, 1f);
                current = newReward;
                metaTarget.Rewards.Add(newReward);
                GUI.FocusControl("");
            }
    }
}