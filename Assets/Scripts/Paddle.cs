using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 12f;

    [Header("Resizing")]
    [SerializeField] private List<Sprite> paddleSprites = new List<Sprite>();
    [SerializeField] private int startingSizeIndex = 2;
    private int currentSizeIndex;

    private bool isInputEnabled = true;
    private float currentInput = 0f;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D trigger;
    private BoxCollider2D collider;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<CapsuleCollider2D>();
        collider = GetComponent<BoxCollider2D>();

        if(paddleSprites.Count > 0)
        {
            currentSizeIndex = Mathf.Clamp(startingSizeIndex, 0, paddleSprites.Count - 1);
        }        
    }

    void Start()
    {
        ApplySize(currentSizeIndex);
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += DisableInput;
        GameEvents.OnAllBricksCleared += DisableInput; 
        GameEvents.OnLevelStarted += EnableInput;
        GameEvents.OnHorizontalMove += HandleHorizontalMove;
        GameEvents.OnPaddleSizeUpCollected += IncreaseSize;
        GameEvents.OnPaddleSizeDownCollected += DecreaseSize;
        GameEvents.OnLevelStarted += NewLevelSizeReset;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= DisableInput;
        GameEvents.OnAllBricksCleared -= DisableInput;
        GameEvents.OnLevelStarted -= EnableInput;
        GameEvents.OnHorizontalMove -= HandleHorizontalMove;
        GameEvents.OnPaddleSizeUpCollected -= IncreaseSize;
        GameEvents.OnPaddleSizeDownCollected -= DecreaseSize;
        GameEvents.OnLevelStarted -= NewLevelSizeReset;
    }

    void FixedUpdate()
    {
        if(!isInputEnabled || rigidbody == null)
        {
            if(rigidbody != null)
            {
                rigidbody.linearVelocity = Vector2.zero;
            }
            return;
        }
            

        Vector2 targetVelocity = new Vector2(currentInput * speed, 0f);
        rigidbody.linearVelocity = targetVelocity;

        currentInput = 0f;
    }

    public void IncreaseSize()
    {
        if(currentSizeIndex < paddleSprites.Count - 1)
        {
            currentSizeIndex++;
            ApplySize(currentSizeIndex);
        }
    }

    public void DecreaseSize()
    {
        if (currentSizeIndex > 0)
        {
            currentSizeIndex--;
            ApplySize(currentSizeIndex);
        }
    }

    private void ApplySize(int index)
    {
        if(paddleSprites == null || paddleSprites.Count == 0 || index < 0 || index >= paddleSprites.Count)
            return;

        Sprite newSprite = paddleSprites[index];

        if(spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;

            float spriteWidth = newSprite.bounds.size.x;

            if(trigger != null)
            {
                Vector2 size = trigger.size;
                size.x = spriteWidth;
                trigger.size = size;
            }

            if(collider != null)
            {
                Vector2 size = collider.size;
                size.x = spriteWidth;
                collider.size = size;
            }
        }
    }

    private void HandleHorizontalMove(float inputvalue)
    {
        currentInput = inputvalue;
    }

    private void EnableInput()
    {
        isInputEnabled = true;
    }

    private void DisableInput()
    {
        isInputEnabled = false;
        if(rigidbody != null)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void NewLevelSizeReset()
    {
        if(paddleSprites.Count > 0)
        {
            currentSizeIndex = Mathf.Clamp(startingSizeIndex, 0, paddleSprites.Count - 1);
            ApplySize(currentSizeIndex);
        }
    }
}
