using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOverdrive : MonoBehaviour {
    public static UnityAction on = delegate { };
    public static UnityAction off = delegate { };

    [SerializeField] private GameObject triggerVFX;
    [SerializeField] private GameObject engineVFXNormal;
    [SerializeField] private GameObject engineVFXOverdrive;
    [SerializeField] private AudioData onSFX;
    [SerializeField] private AudioData offSFX;

    private void Awake() {
        on += On;
        off += Off;
    }

    private void OnDestroy() {
        on -= On;
        off -= Off;
    }

    private void On() {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverdrive.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    private void Off() {
        engineVFXNormal.SetActive(true);
        engineVFXOverdrive.SetActive(false);
        AudioManager.Instance.PlayRandomSFX(offSFX);
    }
}