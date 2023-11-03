using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    //Start Scene Components
    private Text highScoreText;
    private Text highScoreTimeText;

    //First Level Components
    private Text scoreText;
    private Text countdownText;
    private Text gameTimerText;
    private Text ghostTimerText;
    private Text ghostTimerLabelText;
    private Text gameOverText;
    private string[] heartNames = { "Life_1", "Life_2", "Life_3" };
    private bool isGameStarted = false;
    private bool isGameEnd = false;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
        isGameEnd = true;
        isGameStarted = false;
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
        isGameEnd = false;
    }

    public void LoadSecondScene()
    {
        SceneManager.LoadScene(2);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            LoadStartSceneComponents();
            UpdateStartSceneComponents();
        }

        if (scene.buildIndex == 1)
        {
            LoadFirstLevelComponents();

        }
    }



    private void LoadStartSceneComponents()
    {
        GameObject highScoreObj = GameObject.Find("HighScore");
        highScoreText = highScoreObj.GetComponent<Text>();

        GameObject highScoreTimeObj = GameObject.Find("HighScoreTime");
        highScoreTimeText = highScoreTimeObj.GetComponent<Text>();
    }

    private void UpdateStartSceneComponents()
    {
        int previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScore(previousHighScore);

        float previousTime = PlayerPrefs.GetFloat("HighScoreTime", 0.00f);
        UpdateHighScoreTime(previousTime);
    }

    public void UpdateHighScoreTime(float timeInSeconds)
    {
        highScoreTimeText.text = FormatTime(timeInSeconds);
    }

    public void UpdateHighScore(int score)
    {
        highScoreText.text = score.ToString();
    }

    private void LoadFirstLevelComponents()
    {

        GameObject exitButton = GameObject.FindGameObjectWithTag("ExitButton");
        if (exitButton != null)
        {
            Button quitButtonComponent = exitButton.GetComponent<Button>();
            if (quitButtonComponent != null)
            {
                quitButtonComponent.onClick.AddListener(LoadStartScene);
            }
        }

        GameObject scoreValueObj = GameObject.Find("ScoreValue");
        scoreText = scoreValueObj.GetComponent<Text>();

        GameObject countDownLabelObj = GameObject.Find("CountDownLabel");
        countdownText = countDownLabelObj.GetComponent<Text>();

        GameObject gameTimerObj = GameObject.Find("GameTimeValue");
        gameTimerText = gameTimerObj.GetComponent<Text>();

        GameObject ghostTimerLabelObj = GameObject.Find("GhostTimeValue");
        ghostTimerText = ghostTimerLabelObj.GetComponent<Text>();

        GameObject ghostTimerValueObj = GameObject.Find("GhostTimerLabel");
        ghostTimerLabelText = ghostTimerValueObj.GetComponent<Text>();

        GameObject gameOverTextLabelObj = GameObject.Find("GameOverTextLabel");
        gameOverText = gameOverTextLabelObj.GetComponent<Text>();

    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateCountDownText(string text)
    {
        countdownText.text = text;
    }

    public void HideCountDown()
    {
        countdownText.enabled = false;
        isGameStarted = true;
    }

    public bool CheckGameStarted()
    {
        return isGameStarted;
    }

    public void DisplayGameOver()
    {
        gameOverText.enabled = true;
    }

    public void UpdateGameTimer(float timeInSeconds)
    {
        gameTimerText.text = FormatTime(timeInSeconds);
    }

    public void UpdateHeartIndicator(int heartRemaining)
    {
        Image heartLost = GameObject.Find(heartNames[heartRemaining]).GetComponent<Image>();
        if (heartLost != null)
        {
            heartLost.enabled = false;
        }
        else
        {
            Debug.LogError("UI Image heartLost not found in the scene.");
        }
    }

    public void HideGhostTimer()
    {
        ghostTimerLabelText.enabled = false;
        ghostTimerText.enabled = false;

    }

    public void DisplayGhostTimer()
    {
        ghostTimerLabelText.enabled = true;
        ghostTimerText.enabled = true;

    }

    public bool GhostTimerActive()
    {
        return ghostTimerText.enabled;
    }

    public void UpdateGhostTimer(float remainingTime)
    {
        ghostTimerText.text = remainingTime.ToString("F1");
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void SetGameEnd(bool newVal)
    {
        isGameEnd = newVal;
    }
    
    public bool CheckGameEnd()
    {
        return isGameEnd;
    }
}
