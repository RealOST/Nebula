using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : PersistentSingleton<AudioManager> {
    [SerializeField] private AudioSource sfxPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<AudioSource> sfxPool = new();

    private const float MIN_PITCH = 0.9f;
    private const float MAX_PITCH = 1.1f;

    protected override void Awake() {
        base.Awake();
        for (var i = 0; i < poolSize; i++) {
            var sfx = Instantiate(sfxPrefab, transform);
            sfx.playOnAwake = false;
            sfxPool.Enqueue(sfx);
        }
    }

    private AudioSource GetAvailableAudioSource() {
        var source = sfxPool.Dequeue();
        sfxPool.Enqueue(source);
        return source;
    }

    public void PlaySFX(AudioData audioData) {
        var source = GetAvailableAudioSource();
        source.pitch = 1f;
        source.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlayRandomSFX(AudioData audioData) {
        var source = GetAvailableAudioSource();
        source.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        source.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlayRandomSFX(AudioData[] audioData) {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }
}

[System.Serializable]
public class AudioData {
    public AudioClip audioClip;
    public float volume;
}