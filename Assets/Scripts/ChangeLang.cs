using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;
    private int id = 0;
    public void ChangeLocale() {
        if (translating == true) {
            return;
        }
        StartCoroutine(SetLocale());
    }
    IEnumerator SetLocale() {
        translating = true;
        yield return LocalizationSettings.InitializationOperation;
        if (id == 0) {
            id = 1;
        } else {
            id = 0;
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        translating = false;
    }
}
