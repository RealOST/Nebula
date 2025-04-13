using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerEnergy : Singleton<PlayerEnergy> {
    [SerializeField] private EnergyBar energyBar;
    [SerializeField] private float overdriveInterval = 0.1f;

    private bool available = true;

    public const int MAX = 100;
    public const int PERCENT = 1;
    private int energy;

    private WaitForSeconds waitforOverdriveInterval => new(overdriveInterval);

    private void OnEnable() {
        PlayerOverdrive.on += PlayerOverDriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void OnDisable() {
        PlayerOverdrive.on -= PlayerOverDriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    private void Start() {
        energyBar.Initialize(energy, MAX);
        Obtain(MAX);
    }

    public void Obtain(int value) {
        if (energy == MAX || !available || !gameObject.activeSelf) return;
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }

    public void Use(int value) {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        if (energy == 0 && !available) PlayerOverdrive.off.Invoke();
    }

    public bool IsEnough(int value) => energy >= value;

    private void PlayerOverDriveOn() {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    private void PlayerOverdriveOff() {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    private IEnumerator KeepUsingCoroutine() {
        while (gameObject.activeSelf && energy > 0) {
            // every 0.1 seconds 
            yield return waitforOverdriveInterval;

            // use 1% of max energy, every 1 second use 10% of max energy 
            // means that overdrive last for 10 seconds
            Use(PERCENT);
        }
    }
}