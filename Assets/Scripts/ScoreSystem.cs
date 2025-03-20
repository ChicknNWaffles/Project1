using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{        
    public static ScoreSystem Instance;
    public int maxScore = 999999999;
    public int maxMult = 32;
    private int currentScore;
    private int currentMult;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MultText;

    public float fontSizeMultiplier = 1.5f;
    public Vector3 OriginalMultPosition;
    public float OriginalMultSize;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }


    void Start() {
        currentScore = 0;
        currentMult = 1;
        OriginalMultPosition = MultText.rectTransform.position;
        OriginalMultSize = MultText.fontSize;
        UpdateScoreBar();
        UpdateMultBar();
    }

    // Use this to increase the multiplier in other scripts
    public void MultIncrease(){
        int newMult = currentMult * 2;
        currentMult = Mathf.Min(newMult, maxMult);
        UpdateMultBar();
    }

    // Resets the mult, when the player gets hit
    public void MultReset() {
        currentMult = 1;
        UpdateMultBar();
    }
    // Use this to increase the score in other scripts
    public void ScoreIncrease(int score) {
        int newScore = score * currentMult;
        currentScore += newScore;
        UpdateScoreBar();
    }

    void UpdateScoreBar() {
        if (currentScore >= maxScore){
            ScoreText.text = "Score: naninf";
        }
        else {
            ScoreText.text = "Score: " + currentScore.ToString();
        }
    }
    
    void UpdateMultBar() {
        MultText.text = "x" + currentMult.ToString();
        MultText.fontSize = OriginalMultSize + ((float)currentMult - 1) * fontSizeMultiplier;
        StartCoroutine(ShakeMult());
    }

    private IEnumerator ShakeMult() {
        float shakeAmount = 1.4f * (float)currentMult;
        float shakeDuration = 0.2f;

        for (float t = 0; t < shakeDuration; t += Time.deltaTime) {
            MultText.transform.position = OriginalMultPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }
        MultText.transform.position = OriginalMultPosition;
    }
}
