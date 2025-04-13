using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader> {
    [SerializeField] private Image transitionImage;
    [SerializeField] private float fadeTime = 3.5f;

    private Color color;

    private const string GAMEPLAY = "Gameplay";
    private const string MAIN_MENU = "MainMenu";
    private const string SCORING = "Scoring";

    private IEnumerator LoadCoroutine(string sceneName) {
        // Load new scene in background
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        // Set this scene inactive
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        // Fade out
        while (color.a < 1f) {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        // Activate the new scene
        loadingOperation.allowSceneActivation = true;

        // Fade in
        while (color.a > 0f) {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
    }

    public void LoadGameplayScene() {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }

    public void LoadMainMenuScene() {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(MAIN_MENU));
    }

    public void LoadScoringScene() {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(SCORING));
    }
}