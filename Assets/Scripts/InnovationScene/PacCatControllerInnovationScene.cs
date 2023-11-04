using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PacCatControllerInnovationScene : MonoBehaviour
{
    //Environment elements
    private AudioManager audioManager;
    private UIManager uIManager;
    private GhostControllerInnovationScene ghostController;
    private bool isGameStarted = false;
    private bool ghostsScared = false;
    private Coroutine scareTimerCoroutine;
    private bool isGameOver = false;
    public int score = 0;
    public float countdownDuration = 4.0f;

    //Generated Map Info
    private LevelGeneratorInnovationScene levelGenerator;
    private int currentCol = 1;
    private int currentRow = 1;

    //Game Timer
    private float startTime;
    private float elapsedTime;

    //Life Indicator
    private int heartCount = 3;

    //PacCat Vars
    private Animator pacCatAnimator;
    private bool isPacCatDead = false;

    //PacCat Lerping
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Vector3 lastInputDirection = Vector3.zero;
    private Vector3 currentInputDirection = Vector3.zero;
    private float deltaTimeSpeed = 1.5f;
    private bool isLearping = false;
    private float stepLength = 0.32f;

    //PacCat audio sources
    [SerializeField]
    private AudioSource walkingAudioSource;
    [SerializeField]
    private AudioSource eatingAudioSource;
    [SerializeField]
    private AudioSource hitWallAudioSource;

    //Particle Systems
    [SerializeField]
    private ParticleSystem dustParticles;
    [SerializeField]
    private ParticleSystem wallCollisionParticles;
    [SerializeField]
    private ParticleSystem deadParticles;

    //Ghost Scared State Timer
    public float scareDuration = 10.0f;



    void Start()
    {
        LoadObjectsOnStart();
        startingPosition = transform.position;
    }

    void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            UpdateGameTimer();
            HandleInput();
            if (!isLearping && !isPacCatDead)
            {
                if (CheckMoveValidity(lastInputDirection))
                {
                    currentInputDirection = lastInputDirection;
                }
                else
                {
                    lastInputDirection = currentInputDirection;
                }
                SetPacCatMovingAnimation(currentInputDirection);
                MovePacCat(currentInputDirection);
            }
        }
    }

    private void LoadObjectsOnStart()
    {
        pacCatAnimator = gameObject.GetComponent<Animator>();

        GameObject generatedMapObject = GameObject.Find("GeneratedMap");
        if (generatedMapObject != null)
        {
            levelGenerator = generatedMapObject.GetComponent<LevelGeneratorInnovationScene>();
        }
        else
        {
            Debug.LogWarning("GeneratedMap object not found.");
        }

        GameObject managersObj = GameObject.Find("Managers");
        if (managersObj != null)
        {
            audioManager = managersObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.LogWarning("AudioManager object not found.");
        }

        GameObject uiManagerObj = GameObject.Find("Managers");
        if (uiManagerObj != null)
        {
            uIManager = uiManagerObj.GetComponent<UIManager>();
        }
        else
        {
            Debug.LogWarning("UIManager object not found.");
        }

        GameObject ghostControllerObject = GameObject.Find("GhostController");
        if (ghostControllerObject != null)
        {
            ghostController = ghostControllerObject.GetComponent<GhostControllerInnovationScene>();
        }
        else
        {
            Debug.LogWarning("Ghost Controller object not found.");
        }

        LoadGameTimer();
    }

    private void LoadGameTimer()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        //Update time and play counting sound
        audioManager.PlayCountDownAudio();
        uIManager.UpdateCountDownText("3");
        yield return new WaitForSeconds(1.0f);

        audioManager.PlayCountDownAudio();
        uIManager.UpdateCountDownText("2");
        yield return new WaitForSeconds(1.0f);

        audioManager.PlayCountDownAudio();
        uIManager.UpdateCountDownText("1");
        yield return new WaitForSeconds(1.0f);

        audioManager.PlayCountDownAudio();
        uIManager.UpdateCountDownText("GO!");
        yield return new WaitForSeconds(1.0f);

        //Hide Count Down
        uIManager.HideCountDown();
        //Update Game State
        isGameStarted = true;
        //Play BgMusic
        audioManager.ResumeNormalAudio();
        //Record Game Start
        startTime = Time.time;
        //Set game End
        uIManager.SetGameEnd(false);
    }

    float totalTime = 90f;
    float remainingTime = 90f;
    private void UpdateGameTimer()
    {

        elapsedTime = Time.time - startTime;
        remainingTime = totalTime - elapsedTime;
        if (remainingTime < 0)
        {
            GameOver();
        }
        else
        { 
            uIManager.UpdateGameTimer(remainingTime); 
        }

    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);
        string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        return formattedTime;
    }

    void HandleInput()
    {
        //Save the user input only when lastInput is different (including different directions and null)
        if (Input.GetKey(KeyCode.W) && lastInputDirection != Vector3.up)
        {
            lastInputDirection = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.A) && lastInputDirection != Vector3.left)
        {
            lastInputDirection = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S) && lastInputDirection != Vector3.down)
        {
            lastInputDirection = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.D) && lastInputDirection != Vector3.right)
        {
            lastInputDirection = Vector3.right;
        }
    }

    void MovePacCat(Vector3 direction)
    {
        if (CheckMoveValidity(direction))
        {
            targetPosition = startingPosition + direction * stepLength;
            StartCoroutine(LerpPacCat(targetPosition));
        }
    }

    private void SetPacCatMovingAnimation(Vector3 direction)
    {
        //Set a dummy value
        int directionValue = -1;

        //Direction value of animator parameters
        if (direction == Vector3.left)
        {
            directionValue = 0;
        }
        else if (direction == Vector3.up)
        {
            directionValue = 1;
        }
        else if (direction == Vector3.right)
        {
            directionValue = 2;
        }
        else
        {
            directionValue = 3;
        }

        //Check if direction has been provided
        if (directionValue != -1)
        {
            pacCatAnimator.SetInteger("Direction", directionValue);
        }
    }


    private IEnumerator LerpPacCat(Vector3 target)
    {
        isLearping = true;
        
        //Check if PacCat is eating pallets
        if (!eatingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
        
        //Play dust effects
        dustParticles.Play();
        
        //Lerping
        float lerpLength = stepLength;
        float distanceCoverted = 0;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            distanceCoverted += Time.deltaTime * deltaTimeSpeed;
            float lerpFraction = distanceCoverted / lerpLength;
            transform.position = Vector3.Lerp(startingPosition, target, lerpFraction);
            yield return null;
        }
        transform.position = target;
        startingPosition = target;
        if (levelGenerator.mapArray[currentRow,currentCol] == 0 || levelGenerator.mapArray[currentRow, currentCol] == 6)
        {
            levelGenerator.GenerateTerritory(currentRow, currentCol);
            score += 10;
            uIManager.UpdateScore(score);
        }
        //Update current index of map array
        if (currentInputDirection == Vector3.up)
        {
            currentRow--;
        }
        else if (currentInputDirection == Vector3.left)
        {
            currentCol--;
        }
        else if (currentInputDirection == Vector3.down)
        {
            currentRow++;
        }
        else if (currentInputDirection == Vector3.right)
        {
            currentCol++;
        }

        isLearping = false;

        //Check if PacCat is movable after lerping
        if (!CheckMoveValidity(currentInputDirection))
        {
            int[] targetColAndRow = GetTargetColAndRow(currentInputDirection);
            if (targetColAndRow[0] != -1 && targetColAndRow[0] != levelGenerator.newCol - 1)
            {
                wallCollisionParticles.transform.position = new Vector3(targetColAndRow[0] * 0.32f, -targetColAndRow[1] * 0.32f, 0);
                wallCollisionParticles.Play();
                hitWallAudioSource.Stop();
                hitWallAudioSource.Play();
            }
            else
            {
                Vector3 newPosition = new Vector3((levelGenerator.newCol - 2) * 0.32f - startingPosition.x, startingPosition.y, 0);
                transform.position = newPosition;
                startingPosition = newPosition;
                currentCol = levelGenerator.newCol - 2 - currentCol;
            }
        }
        walkingAudioSource.Stop();
        dustParticles.Stop();
    }

    private bool CheckMoveValidity(Vector3 direction)
    {
        int[] targetColAndRow = GetTargetColAndRow(direction);
        return CheckMovable(targetColAndRow[0], targetColAndRow[1]);
    }

    private bool CheckMovable(int targetCol, int targetRow)
    {
        if (targetCol < -1 || targetRow < -1)
        {
            return false;
        }
        else if (targetCol == -1 || targetCol == levelGenerator.newCol - 1)
        {
            return false;

        }
        else if (ghostController.IsInSpawnArea(new int[2] { targetRow, targetCol}))
        {
            return false;
        }
        else
        {
            int valueToCheck = levelGenerator.mapArray[targetRow, targetCol];
            if (valueToCheck == 5 || valueToCheck == 6 || valueToCheck == 0)
            {
                return true;
            }
            return false;
        }
    }

    private int[] GetTargetColAndRow(Vector3 direction)
    {
        int targetCol = -2;
        int targetRow = -2;

        if (direction == Vector3.up)
        {
            targetCol = currentCol;
            targetRow = currentRow - 1;
        }
        else if (direction == Vector3.left)
        {
            targetCol = currentCol - 1;
            targetRow = currentRow;
        }
        else if (direction == Vector3.down)
        {
            targetCol = currentCol;
            targetRow = currentRow + 1;
        }
        else if (direction == Vector3.right)
        {
            targetCol = currentCol + 1;
            targetRow = currentRow;
        }
        return new int[] { targetCol, targetRow };
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("BonusCan"))
        {
            Destroy(other.gameObject);
            score += 100;
            uIManager.UpdateScore(score);
        }
        else if (other.CompareTag("PowerPill"))
        {
            Destroy(other.gameObject);
            audioManager.PlayScaredAudio();
            if (scareTimerCoroutine != null)
            {
                StopCoroutine(scareTimerCoroutine);
            }
            scareTimerCoroutine = StartCoroutine(ScareTimer());
        }
        else if (other.CompareTag("Ghost"))
        {
            if (!ghostsScared)
            {
                pacCatAnimator.SetBool("isDead", true);
                isPacCatDead = true;
                heartCount--;
                uIManager.UpdateHeartIndicator(heartCount);
                deadParticles.Play();
                if (heartCount > 0)
                {
                    StartCoroutine(RespawnPacCatWithDelay());
                }
                else
                {
                    GameOver();
                }
            }
            else
            {
                Animator ghostAnimator = other.gameObject.GetComponent<Animator>();
                if (!ghostAnimator.GetBool("isDead"))
                {
                    score += 300;
                    uIManager.UpdateScore(score);
                    ghostAnimator.SetBool("isDead", true);
                    ghostController.SetGhostDeadState(other.gameObject,true);
                    audioManager.PlayGhostDeadAudio();
                    StartCoroutine(TransitionToWalkingStateAfterDelay(5.0f, ghostAnimator,other.gameObject));
                }
            }
        }
    }

        private void GameOver()
    {
        uIManager.DisplayGameOver();
        uIManager.SetGameEnd(true);
        isGameOver = true;
        SaveScoresAndTime();
        StartCoroutine(ReturnToStartSceneAfterDelay(3.0f));
    }

    private void SaveScoresAndTime()
    {
        int previousHighScore = PlayerPrefs.GetInt("Lv2HighScore", 0);
        float previousTime = PlayerPrefs.GetFloat("Lv2HighScoreLiveTime", float.MaxValue);
        if (score > previousHighScore || (score == previousHighScore && elapsedTime > previousTime))
        {
            PlayerPrefs.SetInt("Lv2HighScore", score);
            PlayerPrefs.SetFloat("Lv2HighScoreLiveTime", elapsedTime);
        }
    }

    private IEnumerator ReturnToStartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        uIManager.LoadStartScene();
    }

    private IEnumerator TransitionToWalkingStateAfterDelay(float delay, Animator ghostAnimator,GameObject deadGhost)
    {
        yield return new WaitForSeconds(delay);
        ghostAnimator.SetBool("isDead", false);
        ghostController.SetGhostDeadState(deadGhost,false);
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        bool ghostsAllNormal = true;
        foreach (GameObject ghost in ghosts)
        {
            if (ghost.GetComponent<Animator>().GetBool("isDead"))
            {
                ghostsAllNormal = false;
            }
        }
        if (ghostsAllNormal)
        {
            if (uIManager.GhostTimerActive())
            {
                audioManager.PlayScaredAudio();
            }
            else
            {
                audioManager.ResumeNormalAudio();
            }
        }
    }

    private IEnumerator RespawnPacCatWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        deadParticles.Stop();
        pacCatAnimator.SetBool("isDead", false);
        isPacCatDead = false;
        lastInputDirection = Vector3.zero;
        currentInputDirection = Vector3.zero;
        startingPosition = new Vector3(0.32f, -0.32f, 0);
        targetPosition = new Vector3(0.32f, -0.32f, 0);
        transform.position = new Vector3(0.32f, -0.32f, 0);
        currentCol = 1;
        currentRow = 1;
    }

    private IEnumerator ScareTimer()
    {
        uIManager.DisplayGhostTimer();
        ghostsScared = true;
        float remainingTime = scareDuration;
        SetGhostScaredState();
        while (remainingTime > 0)
        {
            if (remainingTime == 3)
            {
                SetGhostRecoveryState();
            }
            uIManager.UpdateGhostTimer(remainingTime);
            yield return new WaitForSeconds(1.0f);
            remainingTime--;
        }
        ghostsScared = false;
        SetGhostNormalState();
        uIManager.HideGhostTimer();
        audioManager.ResumeNormalAudio();
    }

    private void SetGhostRecoveryState()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            Animator ghostAnimator = ghost.GetComponent<Animator>();
            ghostAnimator.SetBool("isRecovering", true);
            ghostAnimator.SetBool("isScared", false);
        }
    }

    private void SetGhostNormalState()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            Animator ghostAnimator = ghost.GetComponent<Animator>();
            ghostAnimator.SetBool("isRecovering", false);
            ghostAnimator.SetBool("isScared", false);
        }
    }

    private void SetGhostScaredState()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            Animator ghostAnimator = ghost.GetComponent<Animator>();
            ghostAnimator.SetBool("isRecovering", false);
            ghostAnimator.SetBool("isScared", true);
        }
    }

    public int[] GetCurrentMapGridPosition()
    {
        return new int[] { currentRow, currentCol};
    }

    public bool CheckPacCatDead()
    {
        return isPacCatDead;
    }
}