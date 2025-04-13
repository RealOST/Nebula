using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {
    [SerializeField] private float speed = 360f;
    [SerializeField] private Vector3 angle;

    private void OnEnable() {
        StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine() {
        while (true) {
            transform.Rotate(angle * speed * Time.deltaTime);

            yield return null;
        }
    }
}