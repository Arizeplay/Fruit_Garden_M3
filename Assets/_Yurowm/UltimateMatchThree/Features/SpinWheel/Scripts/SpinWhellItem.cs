using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yurowm.GameCore;
using TMPro;

public class SpinWhellItem : MonoBehaviour {

    [HideInInspector]
    public int index = 0;
    public TextMeshProUGUI counter;
    public Image icon;

    public Color oddColor;
    public Color evenColor;

    new Animation animation;

    SpinWheelAssistant.Reward reward = null;

    [ContentSelector]
    public CollectionEffect collectionEffect;

    void Awake() {
        animation = GetComponent<Animation>();
    }

    public void SetInfo (SpinWheelAssistant.Reward reward) {
        this.reward = reward;
        counter.text = reward.count.ToString();
        icon.sprite = reward.icon;
        counter.color = index % 2 == 0 ? evenColor : oddColor;

        transform.rotation = Quaternion.Euler(0, 0, 360f * index / 12);
    }

    public void Rewarded(Transform target) {
        StopAllCoroutines();
        StartCoroutine(Rewarding(target));
    }

    IEnumerator Rewarding(Transform target) {
        animation.Play();

        yield return new WaitForSeconds(0.3f);

        if (reward != null && collectionEffect) {
            int count = reward.count;
            if (count > 100)
                count = 10;
            else if (count > 5)
                count = 5;

            for (int i = 0; i < count; i++) {
                CollectionEffect effect = Content.Emit(collectionEffect);
                effect.transform.Reset();
                effect.transform.position = icon.transform.position;
                effect.SetIcon(reward.icon);
                effect.SetOrder(count - i);
                effect.SetTarget(target);
                effect.Play();

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
