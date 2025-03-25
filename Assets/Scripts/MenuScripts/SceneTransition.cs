using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public Image fadeImage;
    public float fadeDuration = 1.0f;

     private void Start() {
        if (fadeImage != null) {
            Color color = fadeImage.color;
            color.a = 0; 
            fadeImage.color = color;
        }
    }
    public void GoToMenu() {
        Debug.Log("GoToMenu() was called!");
        StartCoroutine(FadeLoadScene("MainMenu"));
    }

    public void RestartGame() {
        Debug.Log("RestartGame() was called!");
        StartCoroutine(FadeLoadScene(SceneManager.GetActiveScene().name));
    }

    // fades scene to black or whatever color is set before loading it
    public IEnumerator FadeLoadScene(string sceneName) {
        Time.timeScale = 1f;
        float elapsedTime = 0;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

    }
}
