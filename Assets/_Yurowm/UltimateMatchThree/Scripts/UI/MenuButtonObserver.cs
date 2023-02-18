using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonObserver : MonoBehaviour
{
    public PanelAnimations panelAnimations;
    public string targetPage;
    public string tag;

    private bool _currentState;
    
    private void Awake()
    {
        UIAssistant.onShowPage += OnShowPage;

        _currentState = false;
        
        var currentPage = UIAssistant.main.GetCurrentPage();
        if (currentPage != null)
        {
            _currentState = currentPage.name == targetPage;
            
            panelAnimations.SetVisible(_currentState);
        }
    }

    private void OnDestroy()
    {
        UIAssistant.onShowPage -= OnShowPage;
    }
    
    private void OnShowPage(UIAssistant.Page page)
    {
        if (page.HasTag(tag))
        {
            var state = page.name == targetPage;
            
            if (_currentState == state)
            {
                return;
            }
            
            _currentState = state;
            
            panelAnimations.SetVisible(_currentState);
        }
    }
}
