using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Saigai.Studios;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;
    private Text[] gameTexts;
    private Font myFont, enFont, jpFont;

    void Start() {
        gameTexts = FindObjectsOfType<Text>();
        enFont = Resources.Load<Font>("Fonts/behance-64ecbed2a3a6e/Fafo Sans/Fafo Sans/Fafo Sans Regular.tff");
        jpFont = Resources.Load<Font>("Fonts/behance-64ecbed2a3a6e/Fafo Sans/Fafo Nihongo/Fafo Nihongo.tff");
    }
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
            myFont = enFont;
        } else if (_id == 1) {
            myFont = jpFont;
        }
		foreach (Text t in gameTexts)
			t.font = myFont;
        translating = false;
    }
}
