using UnityEngine;

public class MultiBallPowerUp : PowerUpBase
{
    protected override void TriggerPowerUpEvent()
    {
        GameEvents.TriggerMultiBallRequested();
    }
}