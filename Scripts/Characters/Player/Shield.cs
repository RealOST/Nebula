using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    public GameObject shield;

    public void showShield() {
        shield.SetActive(true);
    }
}