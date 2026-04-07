using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;

    private Vector3 startPostion;

    private void Awake()
    {
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        startPostion = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    private void LateUpdate()
    {
        transform.localScale = new Vector3(5f, 5f, 5f);
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (isEnemyProjectile && player != null)
        {
            player.TakeDamage(1, transform);

            Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPostion) > projectileRange)
        {
            Destroy(gameObject);
        }
    }
}
