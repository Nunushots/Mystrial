    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour
{
    public enum SpellType
    {
        Electroball,
        Lightning,
        ForceField,
        Laser
    }

    public SpellType currentSpell = SpellType.Electroball;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Spawn Point")]
    public Transform staffTip;

    [Header("Prefabs")]
    public GameObject electroballPrefab;
    public GameObject lightningPrefab;
    public GameObject forceFieldPrefab;
    public GameObject laserPrefab;

    [Header("Mana Costs")]
    public int electroballManaCost = 1;
    public int lightningManaCost = 2;
    public int forcefieldManaCost = 1; // 1 per cast (3 total possible)
    public int laserManaCost = 5;

    [Header("Electroball Settings")]
    public float electroballFireRate = 0.25f;
    private float electroballTimer = 0f;

    [Header("Lightning Settings")]
    public float lightningCooldown = 0.6f;
    public float lightningYOffset = 1.5f;
    private float lightningTimer = 0f;

    [Header("Forcefield Settings")]
    public int maxForcefieldCharges = 3;
    public float forcefieldRechargeTime = 5f;
    private int currentForcefieldCharges;
    private float forcefieldRechargeTimer = 0f;

    [Header("Laser Settings")]
    public float laserCooldown = 2f;
    private float laserTimer = 0f;

    Vector2 storedMousePos;
    Vector2 storedDirection;

    private PlayerMana playerMana;

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
        currentForcefieldCharges = maxForcefieldCharges;
    }

    void Update()
    {
        if (IntroManager.Instance != null && IntroManager.Instance.IsIntroPlaying)
            return;

        HandleSpellSwitching();
        HandleForcefieldRecharge();

        electroballTimer -= Time.deltaTime;
        lightningTimer -= Time.deltaTime;
        laserTimer -= Time.deltaTime;

        if (currentSpell == SpellType.Electroball)
        {
            HandleElectroballInput();
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && CanCastCurrentSpell())
                StartCast();
        }
    }

    void HandleSpellSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentSpell = SpellType.Electroball;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentSpell = SpellType.Lightning;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentSpell = SpellType.ForceField;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentSpell = SpellType.Laser;
    }

    void HandleElectroballInput()
    {
        if (!Input.GetMouseButton(0)) return;
        if (electroballTimer > 0f) return;

        StartCast();
        electroballTimer = electroballFireRate;
    }

    bool CanCastCurrentSpell()
    {
        switch (currentSpell)
        {
            case SpellType.Electroball:
                return electroballTimer <= 0f &&
                       playerMana.HasEnoughMana(electroballManaCost);

            case SpellType.Lightning:
                return lightningTimer <= 0f &&
                       playerMana.HasEnoughMana(lightningManaCost);

            case SpellType.ForceField:
                return currentForcefieldCharges > 0 &&
                       playerMana.HasEnoughMana(forcefieldManaCost);

            case SpellType.Laser:
                return laserTimer <= 0f &&
                       playerMana.HasEnoughMana(laserManaCost);
        }

        return true;
    }

    void StartCast()
    {
        storedMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        storedDirection = (storedMousePos - (Vector2)transform.position).normalized;

        spriteRenderer.flipX = storedDirection.x < 0;

        animator.SetTrigger("cast");
        AudioManager.Instance.PlayCast();
    }

    // Called by animation event
    public void ReleaseSpell()
    {
        switch (currentSpell)
        {
            case SpellType.Electroball:
                SpawnElectroball(staffTip.position, storedDirection);
                break;

            case SpellType.Lightning:
                SpawnLightning(storedMousePos);
                break;

            case SpellType.ForceField:
                SpawnForceField(storedMousePos);
                break;

            case SpellType.Laser:
                SpawnLaser(staffTip.position, storedDirection);
                break;
        }
    }

    void SpawnElectroball(Vector2 pos, Vector2 dir)
    {
        if (!playerMana.HasEnoughMana(electroballManaCost)) return;

        playerMana.UseMana(electroballManaCost);

        GameObject ball = Instantiate(electroballPrefab, pos, Quaternion.identity);
        ball.GetComponent<Electroball>().Initialize(dir);
    }

    void SpawnLightning(Vector2 targetPos)
    {
        if (lightningTimer > 0f) return;
        if (!playerMana.HasEnoughMana(lightningManaCost)) return;

        playerMana.UseMana(lightningManaCost);

        Vector2 spawnPos = new Vector2(targetPos.x, targetPos.y + lightningYOffset);
        Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

        lightningTimer = lightningCooldown;
    }

    void SpawnForceField(Vector2 targetPos)
    {
        if (currentForcefieldCharges <= 0) return;
        if (!playerMana.HasEnoughMana(forcefieldManaCost)) return;

        playerMana.UseMana(forcefieldManaCost);

        Instantiate(forceFieldPrefab, targetPos, Quaternion.identity);

        currentForcefieldCharges--;

        if (currentForcefieldCharges <= 0)
            forcefieldRechargeTimer = forcefieldRechargeTime;
    }

    void HandleForcefieldRecharge()
    {
        if (currentForcefieldCharges > 0) return;

        forcefieldRechargeTimer -= Time.deltaTime;

        if (forcefieldRechargeTimer <= 0f)
            currentForcefieldCharges = maxForcefieldCharges;
    }

    void SpawnLaser(Vector2 pos, Vector2 dir)
    {
        if (laserTimer > 0f) return;
        if (!playerMana.HasEnoughMana(laserManaCost)) return;

        playerMana.UseMana(laserManaCost);

        GameObject laser = Instantiate(laserPrefab, pos, Quaternion.identity);
        laser.GetComponent<LaserBeam>().Initialize(dir, GetComponent<PlayerController>());

        laserTimer = laserCooldown;
    }
}

