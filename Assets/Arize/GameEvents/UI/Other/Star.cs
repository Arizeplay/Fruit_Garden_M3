using System;
using Ninsar.GameEvents.Quests;
using UnityEngine;
using UnityEngine.UI;
namespace Ninsar.GameEvents.UI.Other
{
    public class Star : MonoBehaviour
    {
        public Sprite EmptyStar;
        public Sprite GrayStar;
        public Sprite GreenStar;
        public Sprite BlueStar;
        public Sprite PurpleStar;
        public Sprite OrangeStar;
        
        public Image Icon;

        public Vector3 NormalScale = Vector3.one;
        public Vector3 HighlightScale = Vector3.one * 1.1f;

        public void SetIcon(RarityType rarityType)
        {
            switch (rarityType)
            {
                case RarityType.Gray:
                    Icon.sprite = GrayStar;
                    break;
                case RarityType.Green:
                    Icon.sprite = GreenStar;
                    break;
                case RarityType.Blue:
                    Icon.sprite = BlueStar;
                    break;
                case RarityType.Purple:
                    Icon.sprite = PurpleStar;
                    break;
                case RarityType.Orange:
                    Icon.sprite = OrangeStar;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rarityType), rarityType, null);
            }
        }

        public void SetEmpty()
        {
            Icon.sprite = EmptyStar;
        }

        public void SetHighlight(bool value)
        {
            Icon.transform.localScale = value ? HighlightScale : NormalScale;
        }
    }
}