using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    [Header("Brick Attributes")]
    [SerializeField] private int pointsValue = 100;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private bool isIndestructable = false;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroySound;

    [Header("Brick Health Sprites")]
    [SerializeField] private List<Sprite> healthSprites = new List<Sprite>();

    [Header("Animation Settings")]
    [SerializeField] private string hitTriggerName = "Hit";

    private int currentHealth;
    private int currentSpriteIndex = 0;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameObject assignedPowerUp;

    public bool IsIndestructable => isIndestructable;
    public GameObject AssignedPowerUp => assignedPowerUp;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        UpdateVisuals();
    }

    public void AssignPowerUp(GameObject powerUpPrefab)
    {
        if(isIndestructable || powerUpPrefab == null)
            return;

        assignedPowerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity, transform);
        assignedPowerUp.SetActive(false);
    }

    private void TriggerHitEffect()
    {
        if(animator == null)
            return;

        animator.SetTrigger(hitTriggerName);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("Ball"))
            return;

        TriggerHitEffect();

        if(hitSound != null)
        {
            GameEvents.TriggerSoundEffect(hitSound);
        }

        if (!isIndestructable)
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            if(healthSprites == null || healthSprites.Count <= 1 || currentSpriteIndex >= healthSprites.Count - 1)
            {
                DestroyBrick();
            }
            else
            {
                currentSpriteIndex++;
                currentHealth = maxHealth;
                UpdateVisuals();
            }
        }
    }

    private void UpdateVisuals()
    {
        if(spriteRenderer == null || healthSprites == null || healthSprites.Count <= 1)
            return;

        if(currentSpriteIndex < healthSprites.Count && healthSprites[currentSpriteIndex] != null)
        {
            spriteRenderer.sprite = healthSprites[currentSpriteIndex];
        }
    }

    private void DestroyBrick()
    {
        if(destroySound != null)
        {
            GameEvents.TriggerSoundEffect(destroySound);
        }

        if(assignedPowerUp != null)
        {
            assignedPowerUp.transform.SetParent(null);
            assignedPowerUp.SetActive(true);
        }

        GameEvents.TriggerBrickDestroyed(pointsValue);
        Destroy(gameObject);
    }
}
