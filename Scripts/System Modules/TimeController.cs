using System.Collections;
using UnityEngine;

public class TimeController : Singleton<TimeController> {
    [SerializeField] [Range(0f, 1f)] private float bulletTimeScale = 0.1f;


    private float defaultFixedDeltaTime;
    private float timeScaleBeforePause;
    private float t;

    protected override void Awake() {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Pause() {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Unpause() {
        Time.timeScale = timeScaleBeforePause;
    }

    public void BulletTime(float duration) {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    public void BulletTime(float inDuration, float outDuration) {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    public void BulletTime(float inDuration, float keepingDuration, float outDuration) {
        StartCoroutine(SlowInKeepAndOutCoroutine(inDuration, keepingDuration, outDuration));
    }

    public void SlowIn(float duration) {
        StartCoroutine(SlowInCoroutine(duration));
    }

    public void SlowOut(float duration) {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    private IEnumerator SlowInKeepAndOutCoroutine(float inDuration, float keepingDuration, float outDuration) {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepingDuration);

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    private IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration) {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    private IEnumerator SlowInCoroutine(float duration) {
        t = 0f;

        while (t < 1f) {
            if (GameManager.GameState != GameState.Paused) {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }

    private IEnumerator SlowOutCoroutine(float duration) {
        t = 0f;

        while (t < 1f) {
            if (GameManager.GameState != GameState.Paused) {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }
}