using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour {
    [Header("==== PlAYER INPUT ====")]
    [SerializeField] private PlayerInput playerInput;

    [Header("==== AUDIO DATA ====")]
    [SerializeField] private AudioData pauseSFX;

    [SerializeField] private AudioData unpauseSFX;

    [Header("==== CANVAS ====")]
    [SerializeField] private Canvas hUDcanvas;

    [SerializeField] private Canvas menusCanvas;

    [Header("==== CANVAS ====")]
    [SerializeField] private Button resumeButtion;

    [SerializeField] private Button optionsButtion;
    [SerializeField] private Button mainMenuButtion;

    private int buttonPressParameterID = Animator.StringToHash("Pressed");

    private void OnEnable() {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButtion.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButtion.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButtion.gameObject.name, OnMainMenuButtonClick);
    }

    private void OnDisable() {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Pause() {
        hUDcanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        UIInput.Instance.SelectUI(resumeButtion);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    private void Unpause() {
        resumeButtion.Select();
        resumeButtion.animator.SetTrigger(buttonPressParameterID);
        AudioManager.Instance.PlaySFX(unpauseSFX);
    }

    private void OnResumeButtonClick() {
        hUDcanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.Unpause();
        playerInput.EnableGameplayInput();
    }

    private void OnOptionsButtonClick() {
        UIInput.Instance.SelectUI(optionsButtion);
        playerInput.EnablePauseMenuInput();
    }

    private void OnMainMenuButtonClick() {
        Time.timeScale = 1f;
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }
}