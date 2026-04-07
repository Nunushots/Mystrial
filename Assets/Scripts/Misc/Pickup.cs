using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour
{
    public enum PickUpType
    {
        GoldCoin,
        ManaGlobe,
        HealthGlobe
    }

    [SerializeField] private PickUpType pickupType;
    [SerializeField] private int manaRestoreAmount = 5;
    [SerializeField] private float pickUpDistance = 5f;
    [SerializeField] private float accelerationrate = .2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;

    private Vector3 moveDir;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());
    }

    private void Update()
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;

        if (Vector3.Distance(transform.position, playerPos) < pickUpDistance)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += accelerationrate * Time.deltaTime;
        }
        else
        {
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            DetectPickupType();
            Destroy(this.gameObject);
        }
    }

    private IEnumerator AnimCurveSpawnRoutine()
    {
        Vector2 startPoint = transform.position;

        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;

            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position =
                Vector2.Lerp(startPoint, endPoint, linearT)
                + new Vector2(0f, height);

            yield return null;
        }
    }

    private void DetectPickupType()
    {
        switch (pickupType)
        {
            case PickUpType.GoldCoin:
                EconomyManager.Instance.UpdateCurrentGold();
                Debug.Log("Gold Coin");
                break;

            case PickUpType.HealthGlobe:
                PlayerHealth.Instance.HealPlayer();
                Debug.Log("Health Globe");
                break;

            case PickUpType.ManaGlobe:
                PlayerMana playerMana = PlayerController.Instance.GetComponent<PlayerMana>();

                if (playerMana != null)
                {
                    playerMana.RestoreMana(manaRestoreAmount);
                }

                Debug.Log("Mana Globe");
                break;
        }
    }
}
