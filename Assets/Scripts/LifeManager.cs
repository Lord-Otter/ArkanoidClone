using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private int startingLives = 4;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform paddleTransform;

    public int CurrentLives { get; private set; }
    private int activeBallCount = 0;

    void Start()
    {
        CurrentLives = startingLives;
        GameEvents.TriggerLivesChanged(CurrentLives);
    }

    private void OnEnable()
    {
        GameEvents.OnBallSpawned += HandleBallSpawned;
        GameEvents.OnBallDestroyed += HandleBallDestroyed;
        GameEvents.OnLevelStarted += HandleLevelStarted;
        GameEvents.OnHealthPowerUpCollected += HandleHealthPowerUpCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnBallSpawned -= HandleBallSpawned;
        GameEvents.OnBallDestroyed -= HandleBallDestroyed;
        GameEvents.OnLevelStarted -= HandleLevelStarted;
        GameEvents.OnHealthPowerUpCollected -= HandleHealthPowerUpCollected;
    }

    private void HandleBallSpawned()
    {
        activeBallCount++;
    }

    private void HandleBallDestroyed()
    {
        activeBallCount--;

        if(activeBallCount <= 0)
        {
            activeBallCount = 0;
            GameEvents.TriggerBallLost();
            DeductLife();
        }
    }

    private void HandleLevelStarted()
    {
        if(paddleTransform == null)
        {
            Paddle paddle = FindFirstObjectByType<Paddle>();
            
            if(paddle != null)
            {
                paddleTransform = paddle.transform;
            }
        }

        CurrentLives = startingLives;
        GameEvents.TriggerLivesChanged(CurrentLives);
        
        activeBallCount = 0;

        if(activeBallCount <= 0 && paddleTransform != null)
        {
            RespawnBall();
        }
    }

    private void DeductLife()
    {
        CurrentLives--;
        GameEvents.TriggerLivesChanged(CurrentLives);

        if(CurrentLives > 0)
        {
            RespawnBall();
        }
        else
        {
            GameEvents.TriggerGameOver();
        }
    }

    private void RespawnBall()
    {
        if(ballPrefab != null && paddleTransform != null)
        {
            Instantiate(ballPrefab, paddleTransform.position, Quaternion.identity);
        }
    }

    private void HandleHealthPowerUpCollected()
    {
        if(CurrentLives < startingLives)
        {
            CurrentLives++;
            GameEvents.TriggerLivesChanged(CurrentLives);
        }
        else
        {
            GameEvents.TriggerBonusPointsRequested(500);
        }
    }
}
