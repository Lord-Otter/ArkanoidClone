using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int TotalScore { get; private set; }
    public int LevelScore { get; private set; }
    public int HighScore { get; private set; }

    [Header("Multiplier Settings")]
    [SerializeField] private float multiplierTimeWindow = 0.5f;
    [SerializeField] private float multiplierSize = 1.5f;
    private float lastBrickDestroyedTime;

    private const string HighScoreKey = "HighScore";

    void Awake()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    void Start()
    {
        GameEvents.TriggerTotalScoreChanged(TotalScore);
        GameEvents.TriggerLevelScoreChanged(LevelScore);
        GameEvents.TriggerHighScoreChanged(HighScore);
    }

    private void OnEnable()
    {
        GameEvents.OnBrickDestroyed += AddBrickScore;
        GameEvents.OnLevelCompleted += FinalizeLevelScore;
        GameEvents.OnGameOver += HandleGameOverScore;
        GameEvents.OnLevelStarted += ResetLevelScore;
        GameEvents.OnBonusPointsRequested += AddBonusScore;
    }

    private void OnDisable()
    {
        GameEvents.OnBrickDestroyed -= AddBrickScore;
        GameEvents.OnLevelCompleted -= FinalizeLevelScore;
        GameEvents.OnGameOver -= HandleGameOverScore;
        GameEvents.OnLevelStarted -= ResetLevelScore;
        GameEvents.OnBonusPointsRequested -= AddBonusScore;
    }

    private void AddBrickScore(int points)
    {
        float currentTime = Time.time;
        int finalPoints = points;

        if(currentTime - lastBrickDestroyedTime <= multiplierTimeWindow)
        {
            finalPoints = Mathf.RoundToInt((points * multiplierSize) / 5f) * 5;
        }

        lastBrickDestroyedTime = currentTime;

        LevelScore += finalPoints;
        GameEvents.TriggerLevelScoreChanged(LevelScore);
    }

    private void ResetLevelScore()
    {
        LevelScore = 0;
        GameEvents.TriggerLevelScoreChanged(LevelScore);
    }

    private void FinalizeLevelScore(int timeBonus, int heartBonus, int levelScore)
    {
        int totalLevelGain = levelScore + timeBonus + heartBonus;
        TotalScore += totalLevelGain;
        LevelScore = 0;

        GameEvents.TriggerTotalScoreChanged(TotalScore);
        GameEvents.TriggerLevelScoreChanged(LevelScore);
        CheckAndSaveHighScore();
    }

    private void HandleGameOverScore()
    {
        TotalScore += LevelScore;
        LevelScore = 0;

        GameEvents.TriggerTotalScoreChanged(TotalScore);
        GameEvents.TriggerLevelScoreChanged(LevelScore);
        CheckAndSaveHighScore();
    }

    private void CheckAndSaveHighScore()
    {
        if(TotalScore > HighScore)
        {
            HighScore = TotalScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            GameEvents.TriggerHighScoreChanged(HighScore);
        }
    }

    private void AddBonusScore(int points)
    {
        LevelScore += points;
        GameEvents.TriggerLevelScoreChanged(LevelScore);
    }
}
