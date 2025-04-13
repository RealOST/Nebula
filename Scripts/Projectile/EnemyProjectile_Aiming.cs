using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile_Aiming : Projectile {
    private void Awake() {
        SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    protected override void OnEnable() {
        StartCoroutine(MoveDirectionCoroutine());
        base.OnEnable();
    }

    private IEnumerator MoveDirectionCoroutine() {
        yield return null;
        if (target.activeSelf)
            moveDirection = (target.transform.position - transform.position).normalized;
    }
}