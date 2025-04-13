using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour {
    [Header("==== CANVAS ====")]
    [SerializeField] private Canvas mainMenuCanvas;

    [Header("==== BUTTONS ====")]
    [SerializeField] private Button buttonStart;

    [SerializeField] private Button buttonOptions;
    [SerializeField] private Button buttonQuit;

    private void OnEnable() {
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonStart.gameObject.name, OnButtonStartClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonOptions.gameObject.name, OnButtonOptionsClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonQuit.gameObject.name, OnButtonQuitClicked);
    }

    private void OnDisable() {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Start() {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(buttonStart);
    }

    private void OnButtonStartClicked() {
        mainMenuCanvas.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    private void OnButtonOptionsClicked() {
        UIInput.Instance.SelectUI(buttonOptions);
    }

    private void OnButtonQuitClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}