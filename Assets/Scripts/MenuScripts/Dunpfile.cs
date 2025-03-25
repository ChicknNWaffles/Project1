/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMenu : MonoBehaviour
{
    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audio.Play();
        StartCoroutine(WaitForAudioAndLoadScene());
    }

    private IEnumerator WaitForAudioAndLoadScene()
    {
        while (audio.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        audio.Play();
        StartCoroutine(WaitForAudioAndLoadScene());
        Application.Quit();
    }
}
*/