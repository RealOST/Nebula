using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour {
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private AudioData[] hitSFX;
    [SerializeField] private float damage;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected Vector2 moveDirection;

    protected GameObject target;

    protected virtual void OnEnable() {
        StartCoroutine(MoveDirectly());
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent<Character>(out var character)) {
            character.TakeDamage(damage);

            // var contactPoint = collision.GetContact(0);
            // PoolManager.Release(hitVFX, contactPoint.point,Quaternion.LookRotation(contactPoint.normal));
            PoolManager.Release(hitVFX, collision.GetContact(0).point,
                Quaternion.LookRotation(collision.GetContact(0).normal));
            AudioManager.Instance.PlayRandomSFX(hitSFX);
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveDirectly() {
        while (gameObject.activeSelf) {
            Move();
            yield return null;
        }
    }

    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

    protected void SetTarget(GameObject target) => this.target = target;
}