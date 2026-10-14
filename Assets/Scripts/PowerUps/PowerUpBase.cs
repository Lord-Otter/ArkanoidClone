using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private AudioClip pickupSound;

    protected virtual void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(pickupSound != null)
            {
                GameEvents.TriggerSoundEffect(pickupSound);
            }

            TriggerPowerUpEvent();

            Destroy(gameObject);
        }
        else if (other.CompareTag("Death"))
        {
            Destroy(gameObject);
        }
    }

    protected abstract void TriggerPowerUpEvent();
}
