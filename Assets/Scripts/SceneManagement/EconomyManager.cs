using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    private int currentGold = 0;

    const string Coin_Amount_Text = "Gold Amount Text";

    public void UpdateCurrentGold()
    {
        currentGold += 1;

        if(goldText == null)
        {
            goldText = GameObject.Find(Coin_Amount_Text).GetComponent<TMP_Text>();
        }

        goldText.text = currentGold.ToString("D3");
    }
}
