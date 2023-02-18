using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceRotationMask : MonoBehaviour
{
    public List<GameObject> portraits;
    public List<GameObject> landscapes;

    [Range(0.01f, 1f)]
    [SerializeField]
    private float timeStep = 0.01f;
    private void OnEnable()
    {
        StartCoroutine(Refresh());
    }

    private IEnumerator Refresh()
    {
        while (true)
        {
            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                portraits.ForEach(x => x.SetActive(true));
                landscapes.ForEach(x => x.SetActive(false));
            } else 
            {
                portraits.ForEach(x => x.SetActive(false));
                landscapes.ForEach(x => x.SetActive(true));
            }

            yield return new WaitForSeconds(timeStep);
        }

        yield return null;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }
    }

#endif

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
