using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MysteriousText : MonoBehaviour
{
    public float maxDistance = 10f;
    public float minDistance = 2f;

    private Transform player;
    private TextMeshProUGUI text;
    private Color textColor;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        textColor = text.color;

        if (PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        float targetAlpha = Mathf.InverseLerp(maxDistance, minDistance, distance);
        targetAlpha = Mathf.Clamp01(targetAlpha);

        // Smooth fade
        textColor.a = Mathf.Lerp(textColor.a, targetAlpha, Time.deltaTime * 5f);
        text.color = textColor;
    }
}