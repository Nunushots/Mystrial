using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electroball : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 3f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        rb.velocity = direction * speed;
        Destroy(gameObject, lifetime);
    }
}


