using UnityEngine;

public class HealthPowerUp : PowerUpBase
{
    protected override void TriggerPowerUpEvent()
    {
        GameEvents.TriggerHealthPowerUpCollected();
    }
}
