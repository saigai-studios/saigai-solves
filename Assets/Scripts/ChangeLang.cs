using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;

    public void ChangeLocale(int id) {
        if (translating == true) {
            return;
        }
        StartCoroutine(SetLocale(id));
    }
    IEnumerator SetLocale(int _id) {
        translating = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_id];
        translating = false;
    }
}
