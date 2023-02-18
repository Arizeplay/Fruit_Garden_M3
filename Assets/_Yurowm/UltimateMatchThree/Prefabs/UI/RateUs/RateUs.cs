using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : MonoBehaviour
{
    [Serializable]
    public class StarButton
    {
        public PanelAnimations star;
        public Button button;
    }
    
    public StarButton[] stars = new StarButton[5];

    private int currentStar = 0;
    
    void Start()
    {
        for (var i = 0; i < stars.Length; i++)
        {
            var index = i;
            
            var starButton = stars[index];
            starButton.star.SetVisible(false, true);
            starButton.button.onClick.AddListener(() =>
            {
                currentStar = index + 1;
        
                Refresh();
            });
        }

        Refresh();
    }

    public void Rate()
    {
        if (currentStar > 3)
        {
            OpenURL();
        }
        
        PlayerPrefs.SetInt("RateUs", currentStar);
        AppMetrica.Instance.ReportEvent("Rate Us", currentStar.ToString());
    }

    [ContextMenu("OpenURL")]
    public void OpenURL()
    {
#if UNITY_ANDROID
        Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
#elif UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif
    }
    
    private void Refresh()
    {
        for (var index = 0; index < stars.Length; index++)
        {
            var starButton = stars[index];
            
            starButton.star.SetVisible(index < currentStar);
        }
    }
}
