using UnityEngine;

public class GameOverScreen : MonoBehaviour {
    [SerializeField] private PlayerInput input;

    [SerializeField] private Canvas HUDCanvas;

    [SerializeField] private AudioData confirmGameOverSound;

    private int exitStateID = Animator.StringToHash("GameOverScreenExit");

    private Canvas canvas;

    private Animator animator;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        animator = GetComponent<Animator>();

        canvas.enabled = false;
        animator.enabled = false;
    }

    private void OnEnable() {
        GameManager.onGameOver += OnGameOver;

        input.onConfirmGameOver += OnConfirmGameOver;
    }

    private void OnDisable() {
        GameManager.onGameOver -= OnGameOver;

        input.onConfirmGameOver -= OnConfirmGameOver;
    }

    private void OnGameOver() {
        HUDCanvas.enabled = false;
        canvas.enabled = true;
        animator.enabled = true;
        input.DisableAllInputs();
    }

    private void OnConfirmGameOver() {
        AudioManager.Instance.PlaySFX(confirmGameOverSound);
        input.EnableGameOverScreenInput();
        animator.Play(exitStateID);
        SceneLoader.Instance.LoadScoringScene();
    }

    // Animation Event
    private void EnableGameOverScreenInput() {
        input.EnableGameOverScreenInput();
    }
}