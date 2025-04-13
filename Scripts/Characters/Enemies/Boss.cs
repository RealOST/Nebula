using DG.Tweening;
using UnityEngine;

public class Boss : Enemy {
    private BossHealthBar healthBar;

    private Canvas healthBarCanvas;

    protected override void Awake() {
        base.Awake();
        healthBar = FindObjectOfType<BossHealthBar>();
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        healthBar.Initialize(health, maxHealth);
        healthBarCanvas.enabled = true;
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent<Player>(out var player)) player.Die();
    }

    public override void Die() {
        healthBarCanvas.enabled = false;
        Camera.main.DOShakePosition(1f, 0.2f, 6, 90, true);
        base.Die();
    }

    public override void TakeDamage(float damage) {
        base.TakeDamage(damage);
        healthBar.UpdateStats(health, maxHealth);
    }

    protected override void SetHealth() {
        maxHealth += EnemyManager.Instance.WaveNumber * healthFactor;
    }
}