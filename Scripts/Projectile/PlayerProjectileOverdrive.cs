using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile {
    [SerializeField] private ProjectileguidanceSystem guidanceSystem;

    protected override void OnEnable() {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        if (target == null) base.OnEnable();
        else
            StartCoroutine(guidanceSystem.HomingCoroutine(target));
    }
}