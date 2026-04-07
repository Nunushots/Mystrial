using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float lifetime = 4f;

    [SerializeField] private float slowMultiplier = 0.5f; // 50% speed

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyPathfinding enemy = other.GetComponent<EnemyPathfinding>();
        if (enemy != null)
        {
            enemy.SetSpeedMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EnemyPathfinding enemy = other.GetComponent<EnemyPathfinding>();
        if (enemy != null)
        {
            enemy.ResetSpeed();
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

