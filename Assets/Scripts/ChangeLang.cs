using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Saigai.Studios;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;

public class ChangeLang : MonoBehaviour {

    private bool translating = false;
    /* old change font method */
    // private Text[] gameTexts;
    // private Font myFont, enFont, jpFont;

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
        /* old change font method */
        // if (_id == 0) {
        //     myFont = enFont;
        // } else if (_id == 1) {
        //     myFont = jpFont;
        // } else {
        //     Debug.Log("No Font Loaded!");
        // }
		// foreach (Text t in gameTexts)
		// 	t.font = myFont;
        // translating = false;
    }
    
    /* old change font method */
    // public void ChangeLoadedFont(string loadMe) {
    //     if (loadMe == "Assets/Resources/Fonts/behance-64ecbed2a3a6e/Fafo Sans/Used Fonts/LoadedFont.otf") {

    //     } else if (File.Exists(loadMe)) {
    //         File.Move(loadMe, "Assets/Resources/Fonts/behance-64ecbed2a3a6e/Fafo Sans/Used Fonts/LoadedFont.otf");
    //     } else {
    //         Debug.Log("Field does not exist");
    //     }
    // }
}
