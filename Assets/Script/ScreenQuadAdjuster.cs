using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenQuadAdjuster : MonoBehaviour {
    void Start() {
        Camera mainCam = Camera.main;
        float distance = mainCam.nearClipPlane + 0.1f;
        transform.position = mainCam.transform.position + mainCam.transform.forward * distance;
        transform.rotation = mainCam.transform.rotation;

        float height = 2 * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        float width = height * mainCam.aspect;
        transform.localScale = new Vector3(width, height, 1);
    }
}