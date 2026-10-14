using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ball"))
        {
            GameEvents.TriggerBallDestroyed();
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("PowerUp") || other.GetComponent<PowerUpBase>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}