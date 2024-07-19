using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Saigai.Studios;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;

    // NOTE: 0 = English, 1 = Japanese
    public void SelectLocale(int id) {
        // out of bounds
        if (id < 0 || id >= LocalizationSettings.AvailableLocales.Locales.Count) {
            Debug.Log("Invalid Locales ID!");
            Debug.Log(id);
            return;
        }

        // save the language
        if (Interop.data_set_language(id) == true) {
            Interop.data_save();
            Debug.Log("Saved language settings successfully.");
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
        if (_id == 0) {
            
        } else {

        }
        translating = false;
    }
}
