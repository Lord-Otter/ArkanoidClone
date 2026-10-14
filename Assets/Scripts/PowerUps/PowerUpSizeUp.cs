using UnityEngine;

public class SizeUpPowerUp : PowerUpBase
{
    protected override void TriggerPowerUpEvent()
    {
        GameEvents.TriggerPaddleSizeUpCollected();
    }
}