using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yurowm.GameCore;

[RequireComponent (typeof (ContentAnimator))]
public class TutorialPopup : ILiveContent, IAnimated {

    ContentAnimator animator;

    public RectTransform IconHolder;
    public TMP_Text Header;
    public TMP_Text Label;
    public Button ClickHandler;
    [NonSerialized]
    public bool clicked = false;

    Transform arrowConnector = null;

    public override void Initialize() {
        animator = GetComponent<ContentAnimator>();
        ClickHandler.gameObject.SetActive(false);
        ClickHandler.onClick.AddListener(OnClick);
    }

    public void Say(string text, string iconName)
    {
        Header.text = iconName;
        Label.text = text;
        
        var sprite = Content.GetPrefab<TutorialSprite>(x => x.Name == iconName);
        Instantiate(sprite, IconHolder);
        
        clicked = false;
        ClickHandler.gameObject.SetActive(true);
    }

    void OnClick() {
        clicked = true;
        ClickHandler.gameObject.SetActive(false);
    }

    public IEnumerator Show() {
        gameObject.SetActive(true);
        yield return animator.PlayAndWait("Show");
    }

    public IEnumerator Hide() {
        yield return animator.PlayAndWait("Hide");
        gameObject.SetActive(false);
    }

    public IEnumerator GetAnimationNames() {
        yield return "Show";
        yield return "Hide";
    }
}
