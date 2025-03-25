using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Credit to SasquatchBStudios
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySoundClip(AudioClip audioClip, Transform transform, float volume, float pitch)
    {
        AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
