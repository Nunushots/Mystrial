using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private bool isBoss = false;

    private int currentHealth;
    private Flash flash;
    private Animator animator;
    private bool isDead = false;

    // References to other enemy behaviour scripts
    private EnemyAI enemyAI;
    private EnemyPathfinding enemyPathfinding;

    private void Start()
    {
        // If this is boss and already dead remove instantly
        if (isBoss && BossState.bossDead)
        {
            Destroy(gameObject);
            return;
        }

        flash = GetComponent<Flash>();
        animator = GetComponent<Animator>();

        enemyAI = GetComponent<EnemyAI>();
        enemyPathfinding = GetComponent<EnemyPathfinding>();

        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        // Ignore damage if already dead
        if (isDead) return;

        currentHealth -= damage;

        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth > 0 || isDead) return;

        isDead = true;

        //  Mark boss as dead HERE (before animation plays)
        if (isBoss)
        {
            BossState.bossDead = true;
        }

        AudioManager.Instance.PlayEnemyDeath();

        // Stop enemy behaviour scripts
        if (enemyAI != null) enemyAI.enabled = false;
        if (enemyPathfinding != null) enemyPathfinding.enabled = false;

        // Play death animation
        if (animator != null)
            animator.SetTrigger("death");
    }

    //  Called by animation event at END of death animation
    public void DestroySelf()
    {
        GetComponent<PickUpSpawner>().DropItems();
        Destroy(gameObject);
    }
}
