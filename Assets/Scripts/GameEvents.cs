using UnityEngine;
using System;

public static class GameEvents
{
  public static event Action<float> OnHorizontalMove;
  public static event Action OnLaunchInput;
  public static event Action OnConfirmInput;

  public static event Action<AudioClip> OnSoundEffectRequested;

  public static event Action<int> OnBrickDestroyed;
  public static event Action OnAllBricksCleared;
  public static event Action<int, int, int> OnLevelCompleted;

  public static event Action OnBallSpawned;
  public static event Action OnBallDestroyed; 
  public static event Action OnBallLost;      
  public static event Action<int> OnLivesChanged;

  public static event Action<int> OnLevelScoreChanged;
  public static event Action<int> OnTotalScoreChanged;
  public static event Action<int> OnHighScoreChanged;
  public static event Action<int> OnLevelNumberChanged;
  public static event Action<float> OnTimerUpdated;

  public static event Action OnGameWon;
  public static event Action OnGameOver;
  public static event Action OnLevelStarted;
  public static event Action OnNextLevelRequested;

  public static event Action OnPaddleSizeUpCollected;
  public static event Action OnPaddleSizeDownCollected;
  public static event Action OnMultiBallRequested;
  public static event Action OnHealthPowerUpCollected;
  public static event Action<int> OnBonusPointsRequested;
  
  public static event Action OnGamePaused;
  public static event Action OnGameResumed;

  public static void TriggerHorizontalMove(float value) => OnHorizontalMove?.Invoke(value);
  public static void TriggerLaunchInput() => OnLaunchInput?.Invoke();
  public static void TriggerConfirmInput() => OnConfirmInput?.Invoke();

  public static void TriggerSoundEffect(AudioClip clip) => OnSoundEffectRequested?.Invoke(clip);

  public static void TriggerBrickDestroyed(int points) => OnBrickDestroyed?.Invoke(points);
  public static void TriggerAllBricksCleared() => OnAllBricksCleared?.Invoke();
  public static void TriggerLevelCompleted(int timeBonus, int heartBonus, int levelScore) => OnLevelCompleted?.Invoke(timeBonus, heartBonus, levelScore);

  public static void TriggerBallSpawned() => OnBallSpawned?.Invoke();
  public static void TriggerBallDestroyed() => OnBallDestroyed?.Invoke();
  public static void TriggerBallLost() => OnBallLost?.Invoke();
  
  public static void TriggerLivesChanged(int lives) => OnLivesChanged?.Invoke(lives);
  
  public static void TriggerLevelScoreChanged(int score) => OnLevelScoreChanged?.Invoke(score);
  public static void TriggerTotalScoreChanged(int score) => OnTotalScoreChanged?.Invoke(score);
  public static void TriggerHighScoreChanged(int highScore) => OnHighScoreChanged?.Invoke(highScore);
  public static void TriggerLevelNumberChanged(int levelNumber) => OnLevelNumberChanged?.Invoke(levelNumber);
  public static void TriggerTimerUpdated(float timeInSeconds) => OnTimerUpdated?.Invoke(timeInSeconds);

  public static void TriggerGameWon() => OnGameWon?.Invoke();
  public static void TriggerGameOver() => OnGameOver?.Invoke();
  public static void TriggerLevelStarted() => OnLevelStarted?.Invoke();
  public static void TriggerNextLevelRequested() => OnNextLevelRequested?.Invoke();

  public static void TriggerPaddleSizeUpCollected() => OnPaddleSizeUpCollected?.Invoke();
  public static void TriggerPaddleSizeDownCollected() => OnPaddleSizeDownCollected?.Invoke();
  public static void TriggerMultiBallRequested() => OnMultiBallRequested?.Invoke();
  public static void TriggerHealthPowerUpCollected() => OnHealthPowerUpCollected?.Invoke();
  public static void TriggerBonusPointsRequested(int points) => OnBonusPointsRequested?.Invoke(points);

  public static void TriggerGamePaused() => OnGamePaused?.Invoke();
  public static void TriggerGameResumed() => OnGameResumed?.Invoke();
}
