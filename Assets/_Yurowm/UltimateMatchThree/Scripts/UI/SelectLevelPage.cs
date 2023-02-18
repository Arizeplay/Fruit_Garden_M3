using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevelPage : MonoBehaviour
{
    public GameObject normalPage;
    public GameObject hardPage;
    private void OnEnable()
    {
        if (SessionDirector.GetDifficulty(LevelDesign.selected) >= LevelDesign.Difficulty.Hard)
        {
            normalPage.SetActive(false);
            hardPage.SetActive(true);
        }
        else
        {
            normalPage.SetActive(true);
            hardPage.SetActive(false);
        }
    }
}
