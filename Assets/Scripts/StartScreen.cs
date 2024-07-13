using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Saigai.Studios;

public class StartScreen : MonoBehaviour
{
    public void Start() {
        // Load the data if possible
        if (Interop.data_load() == true) {
            Debug.Log("Loaded data successfully.");
            // Update all settings
            SelectLocale(Interop.data_get_language());
            // Update the status of the completed minigames
            MinigameWin.doneMinigame1 = Interop.data_has_earthquake_card(0);
            MinigameWin.doneMinigame2 = Interop.data_has_earthquake_card(1);
            MinigameWin.doneMinigame3 = Interop.data_has_earthquake_card(2);
        } else {
            Debug.Log("Failed to load data.");
        }
    }

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
        translating = false;
    }
}
