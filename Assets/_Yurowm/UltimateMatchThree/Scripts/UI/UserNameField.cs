using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserNameField : MonoBehaviour
{
    public TMP_InputField inputField;

    public const string PlayerNameKey = "PlayerName";
    void Start()
    {
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            inputField.text = PlayerPrefs.GetString(PlayerNameKey);
        }
        
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string value)
    {
        PlayerPrefs.SetString(PlayerNameKey, value);
    }
}
