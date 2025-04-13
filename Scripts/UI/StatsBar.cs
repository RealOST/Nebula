using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour {
    [SerializeField] private Image fillImageBack;
    [SerializeField] private Image fillImageFront;
    [SerializeField] private bool delayFill = true;
    [SerializeField] private float fillDelay = 0.5f;
    [SerializeField] private float fillSpeed = 0.1f;
    private float currentFillAmount;
    protected float targetFillAmount;
    private float previousFillAmount;
    private float t;

    private WaitForSeconds waitForDelayFill;

    private Coroutine bufferedFillingCoroutine;

    private void Awake() {
        if (TryGetComponent<Canvas>(out var canvas))
            canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue) {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue) {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCoroutine != null) StopCoroutine(bufferedFillingCoroutine);

        // if stats reduce 当状态值减少时
        if (currentFillAmount > targetFillAmount) {
            // fill image front fill amount = targetFillAmount 前面图片的填充值 = 目标填充值
            fillImageFront.fillAmount = targetFillAmount;
            // slowly reduce fill image back's fill amount 慢慢减少后面图片的填充值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        }
        // if stats increase 当状态值增加时
        else if (currentFillAmount < targetFillAmount) {
            // fill image back's fill amount = targetFillAmounr 后面图片的填充值 = 目标填充值
            fillImageBack.fillAmount = targetFillAmount;
            // slowly increase fill image front's fill amount 慢慢增加前面图片的填充值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image) {
        if (delayFill) yield return waitForDelayFill;

        previousFillAmount = currentFillAmount;
        t = 0f;
        while (t < 1f) {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}