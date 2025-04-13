using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController {
    [SerializeField] private float continuousFireDuration = 1.5f;

    [Header("---- Player Detection ----")]
    [SerializeField] private Transform playerDetectionTransform;

    [SerializeField] private Vector3 playerDetectionSize;
    [SerializeField] private LayerMask playerLayer;

    [Header("---- Beam ----")]
    [SerializeField] private float beamCooldownTime = 12f;

    [SerializeField] private AudioData beamChargingSFX;
    [SerializeField] private AudioData beamLaunchSFX;

    private bool isBeamReady;

    private int launchBeamID = Animator.StringToHash("launchBeam");

    private WaitForSeconds waitForContinuousFireInterval => new(minFireInterval);
    private WaitForSeconds waitForFireInterval => new(maxFireInterval);
    private WaitForSeconds waitBeamCooldownTime => new(beamCooldownTime);

    private List<GameObject> magazine;

    private AudioData launchSFX;

    private Animator animator;

    private Transform playerTransform;

    protected override void Awake() {
        base.Awake();

        animator = GetComponent<Animator>();

        magazine = new List<GameObject>(projectiles.Length);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void OnEnable() {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

    private void ActivateBeamWeapon() {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    private void AE_LaunchBeam() {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    private void AE_StopBeam() {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    private void LoadProjectiles() {
        magazine.Clear();

        if (Physics2D.OverlapBox(playerDetectionTransform.position, playerDetectionSize, 0f, playerLayer)) {
            magazine.Add(projectiles[0]);
            launchSFX = projectileLaunchSFX[0];
        }
        else {
            if (Random.value < 0.5f) {
                magazine.Add(projectiles[1]);
                launchSFX = projectileLaunchSFX[1];
            }
            else {
                for (var i = 2; i < projectiles.Length; i++) magazine.Add(projectiles[i]);

                launchSFX = projectileLaunchSFX[2];
            }
        }
    }

    protected override IEnumerator RandomlyFireCoroutine() {
        while (isActiveAndEnabled) {
            if (GameManager.GameState == GameState.GameOver) yield break;

            if (isBeamReady) {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));

                yield break;
            }

            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    private IEnumerator ContinuousFireCoroutine() {
        LoadProjectiles();
        muzzleVFX.Play();

        var continuousFireTimer = 0f;

        while (continuousFireTimer < continuousFireDuration) {
            foreach (var projectile in magazine) PoolManager.Release(projectile, muzzle.position);

            continuousFireTimer += minFireInterval;
            AudioManager.Instance.PlayRandomSFX(launchSFX);

            yield return waitForContinuousFireInterval;
        }

        muzzleVFX.Stop();
    }

    private IEnumerator BeamCooldownCoroutine() {
        yield return waitBeamCooldownTime;

        isBeamReady = true;
    }

    private IEnumerator ChasingPlayerCoroutine() {
        while (isActiveAndEnabled) {
            targetPosition.x = Viewport.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;

            yield return null;
        }
    }
}