using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSwiter : MonoBehaviour
{
    public Button button;
    public PanelAnimations panelAnimations_A;
    public PanelAnimations panelAnimations_B;

    private PanelAnimations _currentPanelAnimations;
    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        panelAnimations_A.SetVisible(true);
        panelAnimations_B.SetVisible(false);
        
        _currentPanelAnimations = panelAnimations_A;
    }

    private void OnClick()
    {
        if (panelAnimations_A.isPlaying || panelAnimations_B.isPlaying) 
            return;
        
        var isCurrentPanelIsA = _currentPanelAnimations == panelAnimations_A;
        if (isCurrentPanelIsA)
        {
            panelAnimations_A.SetVisible(false);
            panelAnimations_B.SetVisible(true);
            _currentPanelAnimations = panelAnimations_B;
        }
        else
        {
            panelAnimations_A.SetVisible(true);
            panelAnimations_B.SetVisible(false);
            _currentPanelAnimations = panelAnimations_A;
        }
    }
}
