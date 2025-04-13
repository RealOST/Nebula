using UnityEngine;

public class EnemyBeam : MonoBehaviour {
    [SerializeField] private float damage = 50f;
    [SerializeField] private GameObject hitVFX;

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent<Player>(out var player)) {
            player.TakeDamage(damage);
            PoolManager.Release(hitVFX, collision.GetContact(0).point,
                Quaternion.LookRotation(collision.GetContact(0).normal));
        }
    }
}