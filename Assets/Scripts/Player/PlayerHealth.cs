using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool isDead {  get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1;

    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;
    private Vector2 checkpointPosition;
    private Knockback knockback;
    private Flash flash;
    private string checkpointSceneName;
    const string Health_Slider_Text = "Healthbar";
    readonly int Death_Hash = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();

        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void Start()
    {
        isDead = false;
        currentHealth = maxHealth;
        checkpointPosition = transform.position;

        UpdateHealthSlider();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy)
        {
            TakeDamage(1, other.transform);
        }
    }

    public void HealPlayer()
    {
        if(currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (isDead) return;         
        if (!canTakeDamage) return;

        knockback.GetKonckedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());

        canTakeDamage = false;
        currentHealth -= damageAmount;

        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckPlayerDeath();
    }

    private void CheckPlayerDeath()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            currentHealth = 0;

            canTakeDamage = false;   // Stop further damage
            UpdateHealthSlider();

            // Disable collider so enemies stop triggering damage
            GetComponent<Collider2D>().enabled = false;

            // Optional: stop movement
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            GetComponent<Animator>().SetTrigger(Death_Hash);
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private IEnumerator DeathRoutine()
    {
        AudioManager.Instance.StopBackgroundMusic();
        AudioManager.Instance.PlayDeathMusic();
        AudioManager.Instance.PlayDeathSFX();

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(RespawnWithFade());
    }

    private void UpdateHealthSlider()
    {
        if(healthSlider == null)
        {
            healthSlider = GameObject.Find(Health_Slider_Text).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        checkpointSceneName = SceneManager.GetActiveScene().name;
    }

    private void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        canTakeDamage = true;

        transform.position = checkpointPosition;

        GetComponent<Collider2D>().enabled = true;

        Animator animator = GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0f);

        UpdateHealthSlider();

        AudioManager.Instance.StopDeathMusic();
        AudioManager.Instance.PlayBackgroundMusic();
    }

    private IEnumerator RespawnWithFade()
    {
        UIFade.Instance.FadeToBlack();

        yield return new WaitForSeconds(1f); // Match fade duration

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != checkpointSceneName)
        {
            SceneManager.LoadScene(checkpointSceneName);
            yield return null; // wait one frame after scene loads
        }

        Respawn(); // Now we properly call it

        UIFade.Instance.FadeToWhite();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            vcam.Follow = transform;
        }
    }
}
