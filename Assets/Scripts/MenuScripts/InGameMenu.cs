using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

    public GameObject game;

    public void ReturnMainMenu()
    {

        game.GetComponent<Game>().togglePause();
        SceneManager.LoadScene(0);

    }

    public void Unpause()
    {

        game.GetComponent<Game>().togglePause();

    }
}
