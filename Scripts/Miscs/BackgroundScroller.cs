using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {
    [SerializeField]private Vector2 scrollerVelocity;
    private Material _material;

    private void Awake() {
        _material = GetComponent<Renderer>().material;
    }

    private void Update() {
        _material.mainTextureOffset += scrollerVelocity * Time.deltaTime;
    }
}
