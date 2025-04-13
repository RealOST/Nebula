using UnityEngine;

public class Enemy : Character {
    [SerializeField] private int scorePoint = 100;
    [SerializeField] private int deathEnergyBonus = 3;
    [SerializeField] protected int healthFactor;

    private LootSpawner lootSpawner;

    protected virtual void Awake() {
        lootSpawner = GetComponent<LootSpawner>();
    }

    protected override void OnEnable() {
        SetHealth();
        base.OnEnable();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent<Player>(out var player)) {
            player.Die();
            Die();
        }
    }

    public override void Die() {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);
        base.Die();
    }

    protected virtual void SetHealth() {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber / healthFactor);
    }
}