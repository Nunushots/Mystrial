using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject projectilePrefab;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    readonly int Attack_Hash = Animator.StringToHash("attack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack()
    {
        animator.SetTrigger(Attack_Hash);

        if(transform.position.x - PlayerController.Instance.transform.position.x < 0 )
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    public void SpawnProjectileAnimEvent()
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.identity); 
    }
}
