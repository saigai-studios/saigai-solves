using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;

    public void SelectLocale(int id) {
        // dev warning
        if (id < 0 || id >= LocalizationSettings.AvailableLocales.Locales.Count) {
            Debug.Log("Invalid Locales ID!");
            return;
        }
        // if a coroutine is currently in progress
        if (translating == true) {
            return;
        }

        StartCoroutine(ChangeLocale(id));
    }
    IEnumerator ChangeLocale(int _id) {
        translating = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_id];
        translating = false;
    }
}
