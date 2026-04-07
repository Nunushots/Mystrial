using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    private bool canAttack = true;

    public enum State
    {
        Static,
        Roaming,
        Attacking
    }

    [SerializeField] private State startingState = State.Roaming;

    private State state;
    private EnemyPathfinding enemyPathfinding;

    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = startingState;
    }

    private void Start()
    {
        if (state == State.Roaming)
        {
            roamPosition = GetRoamingPosition();
        }
        else
        {
            enemyPathfinding.MoveTo(Vector2.zero); // ensure no movement
        }
    }

    private void Update()
    {
        if (IntroManager.Instance != null && IntroManager.Instance.IsIntroPlaying)
            return;

        MovememntStateControl();
    }

    private void MovememntStateControl()
    {
        switch (state)
        {
            case State.Static:
                StaticBehaviour();
                break;

            case State.Roaming:
                Roaming();
                break;

            case State.Attacking:
                Attacking();
                break;
        }
    }

    private void Roaming()
    {
        timeRoaming += Time.deltaTime;

        enemyPathfinding.MoveTo(roamPosition);

        if(Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat)
        {
            roamPosition = GetRoamingPosition();
            timeRoaming = 0f;
        }
    }

    private void Attacking()
    {
        float distanceToPlayer =
            Vector2.Distance(transform.position,
            PlayerController.Instance.transform.position);

        if (distanceToPlayer > attackRange)
        {
            state = (startingState == State.Static)
                ? State.Static
                : State.Roaming;
            return;
        }

        if (attackRange !=0 && canAttack)
        {
            canAttack = false;
            (enemyType as IEnemy).Attack();

            if (stopMovingWhileAttacking)
            {
                enemyPathfinding.StopMoving();
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private void StaticBehaviour()
    {
        enemyPathfinding.StopMoving();

        if (Vector2.Distance(transform.position,
            PlayerController.Instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
