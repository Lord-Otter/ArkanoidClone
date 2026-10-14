using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static List<Ball> ActiveBalls = new List<Ball>();

    [Header("Physics Settings")]
    [SerializeField] [Min(2)] private float speed = 6.5f;
    [SerializeField] private Transform paddle;
    [SerializeField] private Vector3 paddleOffset = new Vector3(0, 0.45f, 0);

    [Header("Bounce Angle Setting")]
    [SerializeField] private float centerBufferRatio = 0.0f;
    [SerializeField] private float minHorizontalAngle = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip paddleHitSound;

    private Rigidbody2D rigidbody;
    private bool isLaunched = false;
    private bool canPaddlebounce = true;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameEvents.TriggerBallSpawned();

        if(paddle == null)
        {
            Paddle paddleComponent = FindFirstObjectByType<Paddle>();
            if(paddleComponent != null)
            {
                paddle = paddleComponent.transform;
            }
        }
    }

    void Update()
    {
        if(!isLaunched)
        {
            if(paddle != null)
            {
                transform.position = paddle.position + paddleOffset;
            }
        }
    }

    private void OnEnable()
    {
        ActiveBalls.Add(this);

        GameEvents.OnLevelStarted += HandleLevelStarted;
        GameEvents.OnAllBricksCleared += HandleLevelCleared;        
        GameEvents.OnConfirmInput += HandleConfirmInput;
        GameEvents.OnMultiBallRequested += HandleMultiBallRequested;
    }

    private void OnDisable()
    {
        if(ActiveBalls.Contains(this))
        {
            ActiveBalls.Remove(this);
        }

        GameEvents.OnLevelStarted -= HandleLevelStarted;
        GameEvents.OnAllBricksCleared -= HandleLevelCleared;        
        GameEvents.OnConfirmInput -= HandleConfirmInput;
        GameEvents.OnMultiBallRequested -= HandleMultiBallRequested;
    }

    private void OnDestroy()
    {
        if(ActiveBalls.Contains(this))
        {
            ActiveBalls.Remove(this);
        }
    }

    private void HandleLevelStarted()
    {
        isLaunched = false;
        canPaddlebounce = true;
        gameObject.SetActive(true);
    }

    private void HandleLevelCleared()
    {
        Destroy(gameObject);
    }

    private void HandleConfirmInput()
    {
        if(!isLaunched && gameObject.activeSelf)
        {
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        GameEvents.TriggerSoundEffect(paddleHitSound);

        isLaunched = true;
        transform.SetParent(null);

        Vector2 launchDirection = new Vector2(Random.Range(-0.2f, 0.2f), 1f).normalized;
        rigidbody.linearVelocity = launchDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isLaunched || !canPaddlebounce)
            return;

        if (other.CompareTag("Player"))
        {
            if(transform.position.y < other.transform.position.y)
                return;

            canPaddlebounce = false;

            if(paddleHitSound != null)
            {
                GameEvents.TriggerSoundEffect(paddleHitSound);
            }

            Vector2 bounceDirection = CalculatePaddleBounce(transform.position, other.transform.position, other.bounds.size.x);

            rigidbody.linearVelocity = bounceDirection * speed;
        }
    }

    private void OnTriggerExit2D(Collider2D Other)
    {
        if (Other.CompareTag("Player"))
        {
            canPaddlebounce = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isLaunched)
            return;

        Vector2 direction = rigidbody.linearVelocity.normalized;

        float minVerticalSine = Mathf.Sin(minHorizontalAngle * Mathf.Deg2Rad);
        
        if(Mathf.Abs(direction.y) < minVerticalSine)
        {
            float xSign = Mathf.Sign(direction.x);
            float ySign = Mathf.Sign(direction.y);
            if (ySign == 0f) ySign = 1f;

            direction.y = ySign * minVerticalSine;
            direction.x = xSign * Mathf.Cos(minHorizontalAngle * Mathf.Deg2Rad);
            
            direction = direction.normalized;
        }

        rigidbody.linearVelocity = direction * speed;
    }

    private Vector2 CalculatePaddleBounce(Vector2 ballPosition, Vector2 paddlePosition, float paddleWidth)
    {
        float halfWidth = paddleWidth / 2f;
        float hitOffset = ballPosition.x - paddlePosition.x;
        
        float normalizedOffset = Mathf.Clamp(hitOffset / halfWidth, -1f, 1f);
        float absoluteOffset = Mathf.Abs(normalizedOffset);

        if(absoluteOffset <= centerBufferRatio)
        {
            return Vector2.up;
        }

        float t = (absoluteOffset - centerBufferRatio) / (1f - centerBufferRatio);
        float calculatedAngle = Mathf.Lerp(90f, 0f, t);

        float finalAngle = Mathf.Max(calculatedAngle, minHorizontalAngle);

        float sign = Mathf.Sign(normalizedOffset);
        float radians = finalAngle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radians) * sign;
        float y = Mathf.Sin(radians);

        return new Vector2(x, y).normalized;
    }
    
    private static void HandleMultiBallRequested()
    {
        const int maxBallsOnScreen = 15;

        if(ActiveBalls.Count >= maxBallsOnScreen)
        {
            return;
        }

        List<Ball> ballsToSplit = new List<Ball>(ActiveBalls);

        foreach (Ball ball in ballsToSplit)
        {
            if(ActiveBalls.Count >= maxBallsOnScreen)
            {
                break;
            }

            if(ball != null && ball.isLaunched)
            {
                ball.SpawnSplitBall();
            }
        }
    }

    private void SpawnSplitBall()
    {
        Vector2 currentVelocity = rigidbody.linearVelocity;
        float currentSpeed = currentVelocity.magnitude > 0f ? currentVelocity.magnitude : speed;

        GameObject clone1 = Instantiate(gameObject, transform.position, Quaternion.identity);
        Ball ball1 = clone1.GetComponent<Ball>();
        if(ball1 != null)
        {
            ball1.isLaunched = true;
            Vector2 rawDir1 = Quaternion.Euler(0, 0, 120f) * currentVelocity.normalized;
            ball1.rigidbody.linearVelocity = ForceMinHorizontalAngle(rawDir1) * currentSpeed;
        }

        GameObject clone2 = Instantiate(gameObject, transform.position, Quaternion.identity);
        Ball ball2 = clone2.GetComponent<Ball>();
        if(ball2 != null)
        {
            ball2.isLaunched = true;
            Vector2 rawDir2 = Quaternion.Euler(0, 0, -120f) * currentVelocity.normalized;
            ball2.rigidbody.linearVelocity = ForceMinHorizontalAngle(rawDir2) * currentSpeed;
        }
    }

    private Vector2 ForceMinHorizontalAngle(Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        float minVerticalSine = Mathf.Sin(minHorizontalAngle * Mathf.Deg2Rad);
        
        if(Mathf.Abs(dir.y) < minVerticalSine)
        {
            float xSign = Mathf.Sign(dir.x);
            if(xSign == 0f)
            {
                xSign = 1f;
            } 
            
            float ySign = Mathf.Sign(dir.y);
            if(ySign == 0f)
            {
                ySign = 1f;
            } 

            dir.y = ySign * minVerticalSine;
            dir.x = xSign * Mathf.Cos(minHorizontalAngle * Mathf.Deg2Rad);
            dir = dir.normalized;
        }

        return dir;
    }
}
