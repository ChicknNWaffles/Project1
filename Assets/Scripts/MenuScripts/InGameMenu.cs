using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

    public void ReturnMainMenu()
    {

        Game.Instance.togglePause();
        SceneManager.LoadScene(0);

    }

    public void Unpause()
    {

        Game.Instance.togglePause();

    }
}
