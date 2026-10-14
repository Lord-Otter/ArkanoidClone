using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Sidebar")]
    [SerializeField] private TMP_Text cumulativeTotalScoreText;
    [SerializeField] private TMP_Text currentLevelScoreText;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text levelTimerText;

    [Header("Hearts")]
    [SerializeField] private List<Image> heartImages = new List<Image>();
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTitleText;
    [SerializeField] private TMP_Text gameOverFinalScoreText;
    [SerializeField] private TMP_Text retryButtonText;

    [Header("Level Cleared Panel")]
    [SerializeField] private GameObject levelClearPanel;
    [SerializeField] private TMP_Text timeBonusText;
    [SerializeField] private TMP_Text heartBonusText;
    [SerializeField] private TMP_Text levelScoreText;
    [SerializeField] private TMP_Text totalLevelScoreText;
    [SerializeField] private TMP_Text userButtonPromptText;
    [SerializeField] private float lineRevealDelay = 0.5f;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip userClickSound;
    [SerializeField] private AudioClip lineRevealSound;
    [SerializeField] private AudioClip levelClearSound;

    private bool isAnimatingBreakdown = false;
    private bool isReadyForNextLevel = false;
    private bool isPaused = false;
    private Coroutine revealRoutine;

    private int cachedTimeBonus;
    private int cachedHeartBonusTotal;
    private int cachedLevelScore;
    private int latestTotalScore;
    private int cachedRemainingLives;

    private void OnEnable()
    {
        GameEvents.OnTotalScoreChanged += UpdateTotalScoreDisplay;
        GameEvents.OnLevelScoreChanged += UpdateLevelScoreDisplay;
        GameEvents.OnLevelNumberChanged += UpdateLevelNumberDisplay;
        GameEvents.OnHighScoreChanged += UpdateHighScoreDisplay;
        GameEvents.OnTimerUpdated += UpdateTimerDisplay;        
        GameEvents.OnLivesChanged += UpdateLivesDisplay;
        GameEvents.OnGameOver += ShowGameOverScreen;
        GameEvents.OnGameWon += ShowGameWonScreen;
        GameEvents.OnLevelCompleted += ShowLevelClearScreen;
        GameEvents.OnLevelStarted += HideLevelClearScreen;
        GameEvents.OnConfirmInput += HandleConfirmPressed;
    }

    private void OnDisable()
    {
        GameEvents.OnTotalScoreChanged -= UpdateTotalScoreDisplay;
        GameEvents.OnLevelScoreChanged -= UpdateLevelScoreDisplay;
        GameEvents.OnLevelNumberChanged -= UpdateLevelNumberDisplay;
        GameEvents.OnHighScoreChanged -= UpdateHighScoreDisplay;
        GameEvents.OnTimerUpdated -= UpdateTimerDisplay;        
        GameEvents.OnLivesChanged -= UpdateLivesDisplay;
        GameEvents.OnGameOver -= ShowGameOverScreen;
        GameEvents.OnGameWon -= ShowGameWonScreen;
        GameEvents.OnLevelCompleted -= ShowLevelClearScreen;
        GameEvents.OnLevelStarted -= HideLevelClearScreen;
        GameEvents.OnConfirmInput -= HandleConfirmPressed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapePressed();
        }
    }

    private void UpdateTotalScoreDisplay(int totalScore)
    {
        latestTotalScore = totalScore;
        if(cumulativeTotalScoreText != null)
        {
            cumulativeTotalScoreText.text = $"Score\n{totalScore}";
        }
    }
    
    private void UpdateLevelScoreDisplay(int levelScore)
    {
        if(currentLevelScoreText != null)
        {
            currentLevelScoreText.text = $"{levelScore}";
        }
    }

    private void UpdateHighScoreDisplay(int highScore)
    {
        if(highScoreText != null)
        {
            highScoreText.text = $"HighScore!\n{highScore}";
        }
    }

    private void UpdateLevelNumberDisplay(int levelNumber)
    {
        if(currentLevelText != null)
        {
            currentLevelText.text = $"LEVEL {levelNumber}";
        }
    }

    private void UpdateTimerDisplay(float timeInSeconds)
    {
        if(levelTimerText != null)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            levelTimerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateLivesDisplay(int currentLives)
    {
        cachedRemainingLives = currentLives;

        for(int i = 0; i < heartImages.Count; i++)
        {
            if(heartImages[i] != null)
            {
                if(i < currentLives)
                {
                    heartImages[i].sprite = fullHeartSprite;
                    heartImages[i].gameObject.SetActive(true);
                }
                else
                {
                    heartImages[i].sprite = emptyHeartSprite;
                    heartImages[i].gameObject.SetActive(true);
                }
            }
        }
    }

    private void ShowGameOverScreen()
    {
        if (isPaused)
        {
            ResumeGame();
        } 

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        } 

        if (levelClearPanel != null)
        {
            levelClearPanel.SetActive(false);
        }
    
        if (gameOverPanel != null)
        {
            if(gameOverTitleText != null)
            {
                gameOverTitleText.text = "GAME OVER";
            }
            if(gameOverFinalScoreText != null)
            {
                gameOverFinalScoreText.text = $"Final Score: {latestTotalScore}";
            }
            if(retryButtonText != null)
            {
                retryButtonText.text = "Retry";
            }

            gameOverPanel.SetActive(true);
        }            
    }

    private void ShowGameWonScreen()
    {
        if (levelClearPanel != null)
        {
            levelClearPanel.SetActive(false);
        }

        if(gameOverPanel != null)
        {
            if(gameOverTitleText != null)
            {
                gameOverTitleText.text = "Congratulations";
            }
            if(gameOverFinalScoreText != null)
            {
                gameOverFinalScoreText.text = $"Final Score: {latestTotalScore}";
            }
            if(retryButtonText != null)
            {
                retryButtonText.text = "Play Again";
            }

            gameOverPanel.SetActive(true);
        }
    }

    public void OnRetryButtonClicked()
    {
        Time.timeScale = 1f;
        PlayUserClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButtonClicked(string sceneName)
    {
        PlayUserClickSound();
        SceneManager.LoadScene(sceneName);
    }

    private void ShowLevelClearScreen(int timeBonus, int heartBonus, int levelScore)
    {
        cachedTimeBonus = timeBonus;
        cachedHeartBonusTotal = heartBonus;
        cachedLevelScore = levelScore;

        if(timeBonusText != null) timeBonusText.gameObject.SetActive(false);
        if(heartBonusText != null) heartBonusText.gameObject.SetActive(false);
        if(levelScoreText != null) levelScoreText.gameObject.SetActive(false);
        if(totalLevelScoreText != null) totalLevelScoreText.gameObject.SetActive(false);
        if(userButtonPromptText != null) userButtonPromptText.gameObject.SetActive(false);

        if(levelClearPanel != null)
        {
            levelClearPanel.SetActive(true);
        }

        isAnimatingBreakdown = true;
        isReadyForNextLevel = false;

        revealRoutine = StartCoroutine(RevealLinesRoutine());
    }

    private IEnumerator RevealLinesRoutine()
    {
        PlayLevelClearSound();

        yield return new WaitForSeconds(2f);

        if(timeBonusText != null)
        {
            timeBonusText.text = $"{cachedTimeBonus}";
            timeBonusText.gameObject.SetActive(true);
            PlayRevealSound();
        }

        yield return new WaitForSeconds(lineRevealDelay);

        if(heartBonusText != null)
        {
            int unitBonus = 500;
            LevelManager levelManager = FindAnyObjectByType<LevelManager>();
            if(levelManager != null)
            {
                unitBonus = levelManager.HeartBonusValue;
            }
            
            heartBonusText.text = $"{cachedRemainingLives} x {unitBonus}";
            heartBonusText.gameObject.SetActive(true);
            PlayRevealSound();
        }

        yield return new WaitForSeconds(lineRevealDelay);

        if(levelScoreText != null)
        {
            levelScoreText.text = $"{cachedLevelScore}";
            levelScoreText.gameObject.SetActive(true);
            PlayRevealSound();
        }

        yield return new WaitForSeconds(lineRevealDelay + 0.5f);

        if(totalLevelScoreText != null)
        {
            int totalLevelScore = cachedTimeBonus + cachedHeartBonusTotal + cachedLevelScore;
            totalLevelScoreText.text = $"{totalLevelScore}";
            totalLevelScoreText.gameObject.SetActive(true);
            PlayRevealSound();
        }

        isAnimatingBreakdown = false;
        isReadyForNextLevel = true;

        yield return new WaitForSeconds(4);

        if(userButtonPromptText != null)
        {
            userButtonPromptText.gameObject.SetActive(true);
        }
    }

    private void SkipBreakdownAnimation()
    {
        int unitBonus = 500;
        LevelManager lvlMgr = FindAnyObjectByType<LevelManager>();
        if (lvlMgr != null)
        {
            unitBonus = lvlMgr.HeartBonusValue;
        } 

        if(timeBonusText != null)
        {
            timeBonusText.text = $"{cachedTimeBonus}";
            timeBonusText.gameObject.SetActive(true);
        }
        if(heartBonusText != null)
        {
            heartBonusText.text = $"{cachedRemainingLives} x {unitBonus}";
            heartBonusText.gameObject.SetActive(true);
        }
        if(levelScoreText != null)
        {
            levelScoreText.text = $"{cachedLevelScore}";
            levelScoreText.gameObject.SetActive(true);
        }
        if(totalLevelScoreText != null)
        {
            int totalLevelScore = cachedTimeBonus + cachedHeartBonusTotal + cachedLevelScore;
            totalLevelScoreText.text = $"{totalLevelScore}";
            totalLevelScoreText.gameObject.SetActive(true);
        }
    }

    private void HandleConfirmPressed()
    {
        if(levelClearPanel != null && levelClearPanel.activeSelf)
        {
            PlayUserClickSound();

            if (isAnimatingBreakdown)
            {
                if(revealRoutine != null)
                {
                    StopCoroutine(revealRoutine);
                }
                SkipBreakdownAnimation();
                isAnimatingBreakdown = false;
                isReadyForNextLevel = true;
            }
            else if(isReadyForNextLevel)
            {
                GameEvents.TriggerNextLevelRequested();
            }
        }
    }

    private void PlayLevelClearSound()
    {
        if(lineRevealSound != null)
        {
            GameEvents.TriggerSoundEffect(levelClearSound);
        }
    }

    private void PlayRevealSound()
    {
        if(lineRevealSound != null)
        {
            GameEvents.TriggerSoundEffect(lineRevealSound);
        }
    }

    private void PlayUserClickSound()
    {
        if(userClickSound != null)
        {
            GameEvents.TriggerSoundEffect(userClickSound);
        }
    }

    private void HideLevelClearScreen()
    {
        if(levelClearPanel != null)
        {
            levelClearPanel.SetActive(false);
        }

        isAnimatingBreakdown = false;
        isReadyForNextLevel = false;
    }

    private void HandleEscapePressed()
    {
        if(gameOverPanel != null && gameOverPanel.activeSelf)
            return;

        if(levelClearPanel != null && levelClearPanel.activeSelf)
        {
            HandleConfirmPressed();
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf)
            return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if(pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            } 

            GameEvents.TriggerGamePaused();
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        PlayUserClickSound();
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        GameEvents.TriggerGameResumed();
    }
}