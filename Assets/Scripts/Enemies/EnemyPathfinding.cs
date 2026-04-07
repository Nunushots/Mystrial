using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private float currentSpeed;
    private Rigidbody2D rb;
    private Vector2 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed; // start at normal speed
    }

    private void FixedUpdate()
    {
        if (IntroManager.Instance != null && IntroManager.Instance.IsIntroPlaying)
            return;

        rb.MovePosition(rb.position + moveDir * (currentSpeed * Time.fixedDeltaTime));
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }

    // Called by forcefield
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeed = moveSpeed * multiplier;
    }

    // Reset when leaving forcefield
    public void ResetSpeed()
    {
        currentSpeed = moveSpeed;
    }

    public void StopMoving()
    {
        moveDir = Vector3.zero;
    }
}

