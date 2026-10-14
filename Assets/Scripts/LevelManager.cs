using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PowerUpConfig
{
    public GameObject powerUpPrefab;
    public float ratio;
}

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private List<GameObject> levelPrefabs = new List<GameObject>();
    [SerializeField] private float levelTransitionDelay = 2f;

    [Header("Bonus Points Settings")]
    [SerializeField] private int heartBonusValue = 1000;
    [SerializeField] private int timeBonusValue = 2500;
    [SerializeField] private float parTimeSeconds = 120f;

    [Header("PowerUp Distribution")]
    [SerializeField] private List<PowerUpConfig> powerUpConfigs = new List<PowerUpConfig>();

    [Header("Audio Clips")]
    [SerializeField] private AudioClip levelStartSound;

    public int HeartBonusValue => heartBonusValue;

    private int currentLevelIndex = 0;
    private GameObject activeLevelInstance;
    private int remainingBricks;

    private float levelTimer = 0f;
    private bool isTiming = false;
    private int levelScoreAccrued = 0;
    private bool levelCompletedHandled = false;

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    void Update()
    {
        if (isTiming)
        {
            levelTimer += Time.deltaTime;
            GameEvents.TriggerTimerUpdated(levelTimer);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnBrickDestroyed += HandleBrickDestroyed;
        GameEvents.OnNextLevelRequested += HandleNextLevelRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnBrickDestroyed -= HandleBrickDestroyed;
        GameEvents.OnNextLevelRequested -= HandleNextLevelRequested;
    }

    private void LoadLevel(int levelIndex)
    {
        if(activeLevelInstance != null)
        {
            Destroy(activeLevelInstance);
        }

        if(levelIndex >= levelPrefabs.Count)
        {
            GameCleared();
            return;
        }

        if(levelPrefabs[levelIndex] != null)
        {
            activeLevelInstance = Instantiate(levelPrefabs[levelIndex], Vector3.zero, Quaternion.identity);

            Brick[] allBricks = activeLevelInstance.GetComponentsInChildren<Brick>();
            remainingBricks = 0;

            foreach(Brick brick in allBricks)
            {
                if (!brick.IsIndestructable)
                {
                    remainingBricks++;
                }
            }

            DistributePowerUps(allBricks);

            levelTimer = 0f;
            levelScoreAccrued = 0;
            isTiming = true;
            levelCompletedHandled = false;

            GameEvents.TriggerSoundEffect(levelStartSound);
            GameEvents.TriggerLevelNumberChanged(currentLevelIndex + 1);
            GameEvents.TriggerLevelStarted();
        }
    }

    private void GameCleared()
    {
        GameEvents.TriggerAllBricksCleared();
        GameEvents.TriggerGameWon();
    }

    private void DistributePowerUps(Brick[] allBricks)
    {
        List<Brick> eligibleBricks = new List<Brick>();
        foreach(Brick brick in allBricks) // Note: using allBricks array
        {
            // filtering handled below
        }

        foreach(Brick brick in allBricks)
        {
            if(!brick.IsIndestructable)
            {
                eligibleBricks.Add(brick);
            }
        }

        ShuffleList(eligibleBricks);

        int brickIndex = 0;
        foreach(PowerUpConfig config in powerUpConfigs)
        {
            if(config.powerUpPrefab == null)
                continue;

            // Calculate exact spawn count dynamically based on the ratio and total eligible bricks
            int spawnCount = Mathf.RoundToInt(eligibleBricks.Count * config.ratio);

            for(int i = 0; i < spawnCount; i++)
            {
                if(brickIndex >= eligibleBricks.Count)
                    break;

                eligibleBricks[brickIndex].AssignPowerUp(config.powerUpPrefab);
                brickIndex++;
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[random];
            list[random] = temp;
        }
    }

    private void HandleBrickDestroyed(int points)
    {
        levelScoreAccrued += points;
        remainingBricks--;

        if(remainingBricks <= 0 && isTiming && !levelCompletedHandled)
        {
            isTiming = false;
            levelCompletedHandled = true;

            int timeBonus = 0;

            if(levelTimer <= parTimeSeconds)
            {
                timeBonus = timeBonusValue;
            }

            int remainingLives = 1;
            LifeManager lifeManager = FindAnyObjectByType<LifeManager>();

            if(lifeManager != null)
            {
                remainingLives = Mathf.Max(1, lifeManager.CurrentLives);
            }

            int heartBonus = remainingLives * heartBonusValue;

            GameEvents.TriggerAllBricksCleared();
            GameEvents.TriggerLevelCompleted(timeBonus, heartBonus, levelScoreAccrued);
        }
    }

    private void HandleNextLevelRequested()
    {
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);
    }
}
