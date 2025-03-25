using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
    public enum WinType { 
        Time,
        Waves,
        Kills
    }

    public static ShooterControls Input { get; private set; }
    public static Game Instance { get; private set; }
    public GameObject playerObj;
    public GameObject gameOverScreen;
    public GameObject pauseMenu;
    public SpriteRenderer player;
    public SpriteRenderer mainBullet;
    public Camera camera;
    public WinType winType;
    public int winCond;
    private bool hasWon = false;
    private bool gameOver = false;
    private float curTime = 0.0f;
    private bool paused = false;
    public int kills = 0;

    // Start is called before the first frame update
    void Start() {

        Instance = this;
        Input = new ShooterControls();
        Input.Enable();
        SceneManager.activeSceneChanged += disableInput;

    }

    // Update is called once per frame
    void Update(){

        if (Input.Standard.Pause.triggered) {
            OnPause();
        }

        if (gameOver) {

            GameOver();

        }

        if (!paused) {
            curTime += Time.deltaTime;
        }

        if(winType == WinType.Time && curTime >= winCond) {
            hasWon = true;
        }
        if(winType == WinType.Waves && Instance.GetComponent<CombatDirector>().waveNumber >= winCond) {
            hasWon = true;
        }
        if(winType == WinType.Kills && kills >= winCond)
        {
            hasWon = true;
        }

        if (hasWon)
        {
            print("You Won!");
        }

    }

    private void disableInput() {

        Input.Standard.Disable();

    }

    private void disableInput(Scene curr, Scene next) {

        Input.Standard.Disable();

    }

    private void enableInput() {

        Input.Standard.Enable();

    }

    public void GameOver()
    {

        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);

    }

    private void OnPause() {

        togglePause();

    }

    public bool togglePause() {
        if (Time.timeScale == 0f) {

            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            paused = false;
            return (false);

        }
        else {

            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
            return (true);

        }
    }

    private bool IsOffScreenVertically(SpriteRenderer sprite, Camera camera)
    {

        var bounds = sprite.bounds;
        var top = camera.WorldToViewportPoint(bounds.max);
        var bottom = camera.WorldToViewportPoint(bounds.min);
        return top.y < 0 || bottom.y > 1;

    }
}
