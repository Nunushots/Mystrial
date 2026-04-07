using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4;
    private bool isDashing = false;

    [SerializeField] private Transform staffTip;
    [SerializeField] private TrailRenderer myTrailRenderer;

    [SerializeField] private int maxStamina = 10;
    [SerializeField] private int dashStaminaCost = 3;
    [SerializeField] private float staminaRegenRate = 4f;

    // ICE TILEMAP
    [SerializeField] private Tilemap iceTilemap;

    private bool isSliding = false;
    private Vector2 slideDirection;

    private float currentStamina;
    private Slider staminaSlider;

    const string Stamina_Slider_Text = "Staminabar";

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private Knockback knockback;

    private bool canMove = true;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        currentStamina = maxStamina;
        UpdateStaminaSlider();

        if (iceTilemap == null)
        {
            Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

            foreach (Tilemap tm in tilemaps)
            {
                if (tm.gameObject.name == "Ice")
                {
                    iceTilemap = tm;
                    break;
                }
            }
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded; // ADD THIS
    }

    private void OnDisable()
    {
        playerControls.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded; // ADD THIS
    }

    private void Update()
    {
        PlayerInput();
        RegenerateStamina();
    }

    private void FixedUpdate()
    {
        if (PlayerHealth.Instance != null && PlayerHealth.Instance.isDead)
            return;

        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        // Respect attack lock
        if (!canMove)
        {
            movement = Vector2.zero;
            myAnimator.SetFloat("moveX", 0);
            myAnimator.SetFloat("moveY", 0);
            return;
        }

        Vector2 input = playerControls.Movement.Move.ReadValue<Vector2>();

        if (isSliding)
        {
            movement = slideDirection;
        }
        else
        {
            movement = input.normalized;

            // Start sliding ONLY on ice
            if (IsOnIce() && movement != Vector2.zero)
            {
                isSliding = true;
                slideDirection = movement;
            }
        }

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void RegenerateStamina()
    {
        if (isDashing) return;

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaSlider();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindIceTilemapInScene();
    }

    private void FindIceTilemapInScene()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap tm in tilemaps)
        {
            if (tm.gameObject.name == "Ice")
            {
                iceTilemap = tm;
                return;
            }
        }

        Debug.LogWarning("Ice tilemap not found in new scene!");
    }

    // CHECK IF PLAYER IS ON ICE TILEMAP
    private bool IsOnIce()
    {
        if (iceTilemap == null) return false;

        Vector3Int cellPos = iceTilemap.WorldToCell(transform.position);
        return iceTilemap.HasTile(cellPos);
    }

    private void Move()
    {
        if (knockback.gettingKnockedBack || PlayerHealth.Instance.isDead)
            return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Stop sliding when leaving ice
        if (isSliding && !IsOnIce())
        {
            isSliding = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSliding)
        {
            isSliding = false;
            movement = Vector2.zero;
        }
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        bool facingLeft = mousePos.x < playerScreenPoint.x;

        mySpriteRenderer.flipX = facingLeft;

        if (staffTip != null)
        {
            Vector3 localPos = staffTip.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (facingLeft ? -1 : 1);
            staffTip.localPosition = localPos;
        }
    }

    // Called by LaserBeam
    public void StartAttack()
    {
        canMove = false;
        movement = Vector2.zero;
    }

    public void EndAttack()
    {
        canMove = true;
    }

    private void Dash()
    {
        if (isDashing) return;
        if (currentStamina < dashStaminaCost) return;

        currentStamina -= dashStaminaCost;
        UpdateStaminaSlider();

        isDashing = true;
        moveSpeed *= dashSpeed;

        myTrailRenderer.emitting = true;
        AudioManager.Instance.PlayDash();

        StartCoroutine(EndDashRoutine());
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;

        yield return new WaitForSeconds(dashTime);

        moveSpeed /= dashSpeed;
        myTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCD);

        isDashing = false;
    }

    private void UpdateStaminaSlider()
    {
        if (staminaSlider == null)
        {
            staminaSlider = GameObject.Find(Stamina_Slider_Text).GetComponent<Slider>();
        }

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }
}