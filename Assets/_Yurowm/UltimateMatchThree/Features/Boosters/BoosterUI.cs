using UnityEngine;
using System.Collections;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using TMPro;
using UnityEngine.UI;

public class BoosterUI : MonoBehaviour
{
    public bool IsShown { get; private set; }
    
    private static ItemID _itemID;
    private static bool _show;
    private static bool _withExitButton;

    public TMP_Text Description;
    public Image Icon;
    public Button cancel;
    public GameObject Panel;
    public Animation anim;
    
	private void Awake () 
    {
        cancel.onClick.AddListener(Cancel);
    }

    private void OnEnable()
    {
        if (_show)
        {
            Show(_itemID, _withExitButton);
        }
        else
        {
            Panel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Cancel() 
    {
        BoosterAssistant.main.Cancel();
    }

    public void Show(ItemID itemID, bool withExitButton = true)
    {
        Panel.SetActive(true);

        _show = true;
        _itemID = itemID;
        _withExitButton = withExitButton;
        
        IsShown = false;
        
        StartCoroutine(ShowIE(itemID, withExitButton));
    }

    public void Hide()
    {
        _show = false;

        StartCoroutine(HideIE());
    }
    
	public IEnumerator ShowIE (ItemID itemID, bool withExitButton = true) 
    {
        cancel.gameObject.SetActive(withExitButton);

        Icon.sprite = ItemIcons.main.GetIconOrNull(itemID);
        Description.text = LocalizationAssistant.main[$"booster/item/{itemID}/description"];
        
        yield return StartCoroutine(Play("ScoreBonusShow"));

        IsShown = true;
    }

    public IEnumerator HideIE() 
    {
        yield return StartCoroutine(Play("ScoreBonusHide"));
        
        IsShown = false;
        
        Panel.SetActive(false);
    }

    IEnumerator Play(string clip) {
        anim.Play(clip);
        while (clip != "") {
            anim[clip].time += Time.unscaledDeltaTime;
            anim.enabled = true;
            anim.Sample();
            anim.enabled = false;
            if (anim[clip].time >= anim[clip].length)
                clip = "";
            yield return 0;
        }
    }
}
