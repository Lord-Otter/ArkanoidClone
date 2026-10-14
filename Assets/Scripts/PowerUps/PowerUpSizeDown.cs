using UnityEngine;

public class SizeDownPowerUp : PowerUpBase
{
    protected override void TriggerPowerUpEvent()
    {
        GameEvents.TriggerPaddleSizeDownCollected();
    }
}