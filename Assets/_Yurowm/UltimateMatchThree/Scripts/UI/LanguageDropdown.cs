using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        var languages = new List<string>();
        foreach (var language in LocalizationAssistant.main.languages)
        {
            languages.Add(language.ToString());
        }

        var currentLanguage = LocalizationAssistant.main.current_language;
        _dropdown.ClearOptions();
        _dropdown.AddOptions(languages);
        _dropdown.value = _dropdown.options.FindIndex(x => x.text == currentLanguage.ToString());
        _dropdown.onValueChanged.AddListener(Select);
    }
    
    void Select (int direction) {
        Debug.Log(LocalizationAssistant.main.languages[direction]);
        LocalizationAssistant.main.LearnLanguage(LocalizationAssistant.main.languages[direction]);
        ItemCounter.RefreshAll();
    }
}
