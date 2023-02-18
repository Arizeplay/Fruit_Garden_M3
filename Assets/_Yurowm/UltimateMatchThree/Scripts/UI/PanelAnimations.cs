using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;

// An important element of UI. It combines elements of the interface closest to the destination.
public class PanelAnimations : MonoBehaviour {

    public static int uiAnimation = 0;

    [FormerlySerializedAs("gameObject")]
    public GameObject DisableObject;
    public GameObject GameObject
    {
        get
        {
            if (DisableObject)
            {
                return DisableObject;
            }

            return gameObject;
        }
    }
    [FormerlySerializedAs("disableObject")]
    public bool DoDisableObject = true;
    public bool waitForComplete;
    bool _isPlaying = false;
    public bool isPlaying {
        get {
            return _isPlaying;
        }
        set {
            if (_isPlaying != value) {
                _isPlaying = value;
                uiAnimation += _isPlaying ? 1 : -1;
            }
        }
    }

	public string hide; // Name of showing animation
	public string show; // Name of hiding animation

	private string currentClip = "";

    Animation anim;
    void Awake() {
        anim = GetComponent<Animation>();
    }

    public void SetVisible(bool visible, bool immediate = false) {
        if (GameObject.activeSelf == visible && DoDisableObject)
            return;
        currentClip = "";
        if (!visible) {
            if (hide != "")
                currentClip = hide;
            else {
                if (DoDisableObject)
                    GameObject.SetActive(false);
                return;
            }
        }
        if (visible) {
            if (DoDisableObject)
                GameObject.SetActive(true);
            if (show != "")
                currentClip = show;
            else
                return;
        }
        if (currentClip == "")
            return;
        if (immediate) {
            anim[currentClip].enabled = true;
            anim[currentClip].time = anim[currentClip].length;
            anim.Sample();
            anim[currentClip].enabled = false;
            if (DoDisableObject)
                GameObject.SetActive(visible);
        } else 
            Play(currentClip);
    }

    void Play(string clip) {
        StartCoroutine(PlayClipRoutine(clip));
    }

    public void PlayClip(string clip) {
        if (!isPlaying)
            Play(clip);
    }

    IEnumerator PlayClipRoutine(string clip) {

        if (waitForComplete)
        {
            yield return new WaitWhile(() => isPlaying);
        }
        
        isPlaying = true;
        
        anim.Play(clip);
        anim[clip].time = 0;

        while (anim[clip].time < anim[clip].length) {

            anim[clip].enabled = true;
            anim[clip].time += Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
            anim.Sample();
            anim[clip].enabled = false;

            yield return 0;
        }

        anim.Stop();

        isPlaying = false;
        if (clip == hide && DoDisableObject)
            GameObject.SetActive(false);
    }
}
	
