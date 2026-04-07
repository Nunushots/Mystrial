using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class IceOnly : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private int walkableLayer;
    private Vector2 lastValidPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        walkableLayer = LayerMask.NameToLayer("EnemyWalkable");
        lastValidPosition = rb.position;
    }

    private void LateUpdate()
    {
        bool onValidTile = false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.bounds.size,
            0f
        );

        foreach (Collider2D c in hits)
        {
            if (c.gameObject.layer == walkableLayer)
            {
                onValidTile = true;
                break;
            }
        }

        if (onValidTile)
        {
            lastValidPosition = rb.position;
        }
        else
        {
            //  Force back AFTER movement script runs
            rb.position = lastValidPosition;
        }
    }
}