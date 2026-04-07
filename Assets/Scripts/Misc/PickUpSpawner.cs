using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoinPrefab, healthGlobePrefab, StaminaGlobePrefab;

    public void DropItems()
    {
        int randomNum = Random.Range(1, 5);

        if(randomNum == 1)
        {
            Instantiate(healthGlobePrefab, transform.position, Quaternion.identity);
        }

        if (randomNum == 2)
        {
            Instantiate(StaminaGlobePrefab, transform.position, Quaternion.identity);
        }

        if (randomNum == 3)
        {
            int randomAmountOfGold = Random.Range(1, 5);

            for(int i = 0; i < randomAmountOfGold; i++)
            {
                Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
            }

        }
    }
}
