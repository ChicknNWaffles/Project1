using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMenu : MonoBehaviour {
    public SceneTransition sceneTransition;
    public void Play() {
        if (sceneTransition != null) {
            sceneTransition.StartCoroutine(sceneTransition.FadeLoadScene("LVL1 - Fleet Fights"));
        } else {
            SceneManager.LoadScene("LVL1 - Fleet Fights");
        }
    }

    public void Controls() {
        if (sceneTransition != null) {
            sceneTransition.StartCoroutine(sceneTransition.FadeLoadScene("Controls"));
        } else {
            SceneManager.LoadScene("Controls");
        }
    }

    public void Endless() {
        if (sceneTransition != null) {
            sceneTransition.StartCoroutine(sceneTransition.FadeLoadScene("LVL0 - ENDLESS"));
        } else {
            SceneManager.LoadScene("LVL0 - ENDLESS");
        }
    }
    public void Quit() {
        Application.Quit();
    }

}
