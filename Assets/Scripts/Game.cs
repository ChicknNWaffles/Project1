using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour {
    public enum WinType { 
        Time,
        Waves,
        Kills,
        Endless
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
    public bool hasWon = false;
    private bool gameOver = false;
    public float curTime = 0.0f;
    private bool paused = false;
    public int kills = 0;
    public int nextLevel; // the scene build number for the next level
    public TextMeshProUGUI objectiveStatementObj;
    public string objectiveStatement;
    private char[] msgArray;
    private int msgLetterIndex = 0;
    private bool msgUp = false;
    public float msgTimer = 5.0f;
    private int indexToReplace = 0;


    // Start is called before the first frame update
    void Start() {

        Instance = this;
        Input = new ShooterControls();
        Input.Enable();
        SceneManager.activeSceneChanged += disableInput;
        msgArray = objectiveStatement.ToCharArray();
        Time.timeScale = 1f;
        gameOverScreen.SetActive(false);

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
        if(winType == WinType.Waves && Instance.GetComponent<CombatDirector>().waveNumber >= winCond + 1) {
            hasWon = true;
        }
        if(winType == WinType.Kills && kills >= winCond)
        {
            hasWon = true;
        }

        if (hasWon)
        {
            print("You Won!");
            SceneManager.LoadScene(nextLevel);
        }

        if (msgLetterIndex < msgArray.Length)
        {
            appendObjectiveText(msgArray[msgLetterIndex].ToString());
            msgLetterIndex++;
        }
        else
        {
            msgUp = true;
        }
        if (msgUp)
        {
            msgTimer -= Time.deltaTime;
        }
        if (msgTimer < 0)
        {
            if (indexToReplace < objectiveStatement.Length)
            {
                partEraseObjectiveText(indexToReplace);
                indexToReplace++;
            }
            else
            {
                clearObjectiveText();
                msgTimer = 1f;
            }
        }


    }

    void setObjectiveText(string msg)
    {
        objectiveStatementObj.text = msg;
    }

    void appendObjectiveText(string msg)
    {
        setObjectiveText(objectiveStatementObj.text + msg);
    }

    // replaces the first character in the objective notification text with a space
    // used for slow erasing
    void partEraseObjectiveText(int i)
    {
        //takes the current text, converts it to a list of characters, leaving off the first few characters,
        // then converts it back to a string and prepends a number of spaces equal to the number of characters
        // removed.
        setObjectiveText(eraseStep(i, objectiveStatement));
    }

    string eraseStep(int i, string msg)
    {
        if (i > 0)
        {
            var msgWithoutFirst = string.Concat(msg.ToCharArray(1, msg.Length - 1));
            return " " + eraseStep(i - 1, msgWithoutFirst);
        }
        else
        {
            return msg;
        }
    }

    void clearObjectiveText()
    {
        objectiveStatementObj.text = "";
        Destroy(objectiveStatementObj.transform.GetChild(0).gameObject);
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

    // game over bounces in
    public void GameOver() {

        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        RectTransform gameOverTransform = gameOverScreen.GetComponent<RectTransform>();
        gameOverTransform.anchoredPosition = new Vector3(0, 1000, 0);
        LeanTween.moveY(gameOverTransform, 0, 1f).setEaseOutBounce().setIgnoreTimeScale(true); 

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
