using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootItem : MonoBehaviour {
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] protected AudioData defaultPickUpSFX;

    private int pickUpStateID = Animator.StringToHash("PickUp");

    protected AudioData pickUpSFX;

    private Animator animator;

    protected Player player;

    protected Text lootMessage;

    private void Awake() {
        animator = GetComponent<Animator>();

        player = FindObjectOfType<Player>();

        lootMessage = GetComponentInChildren<Text>(true);

        pickUpSFX = defaultPickUpSFX;
    }

    private void OnEnable() {
        StartCoroutine(MoveCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PickUp();
    }

    protected virtual void PickUp() {
        StopAllCoroutines();
        animator.Play(pickUpStateID);
        AudioManager.Instance.PlayRandomSFX(pickUpSFX);
    }

    private IEnumerator MoveCoroutine() {
        var speed = Random.Range(minSpeed, maxSpeed);

        var direction = Vector3.left;

        while (true) {
            if (player.isActiveAndEnabled) direction = (player.transform.position - transform.position).normalized;

            transform.Translate(direction * speed * Time.deltaTime);

            yield return null;
        }
    }
}