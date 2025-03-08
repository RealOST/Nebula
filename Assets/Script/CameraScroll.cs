using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour {
    public float baseScrollSpeed = 2.0f; // 基础右移速度
    private Vector3 moveDirection = Vector3.right;

    void LateUpdate() {
        transform.Translate(moveDirection * baseScrollSpeed * Time.deltaTime);
    }
}
