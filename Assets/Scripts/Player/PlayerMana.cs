using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : Singleton<PlayerMana>
{
    [SerializeField] private int maxMana = 20;

    private int currentMana;
    private Slider manaSlider;

    const string Mana_Slider_Text = "Manabar";

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaSlider();
    }

    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaSlider();
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaSlider();
    }

    public void RestoreFullMana()
    {
        currentMana = maxMana;
        UpdateManaSlider();
    }

    private void UpdateManaSlider()
    {
        if (manaSlider == null)
        {
            manaSlider = GameObject.Find(Mana_Slider_Text).GetComponent<Slider>();
        }

        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;
    }
}