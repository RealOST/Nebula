using System.Collections;
using UnityEngine;

public class MissileSystem : MonoBehaviour {
    [SerializeField] private int defaultAmount = 5;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private AudioData launchSFX = null;

    private bool isReady = true;

    private int amount;

    private void Awake() {
        amount = defaultAmount;
    }

    private void Start() {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void PickUp() {
        amount++;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 1) {
            MissileDisplay.UpdateCooldownImage(0f);
            isReady = true;
        }
    }

    public void Launch(Transform muzzleTransform) {
        if (amount == 0 || !isReady) return; // TODO: Add SFX && UI VFX here 

        // isReady = false;
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
            MissileDisplay.UpdateCooldownImage(1f);
        else
            StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine() {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f) {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }
}