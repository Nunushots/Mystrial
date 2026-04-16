using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroManager : MonoBehaviour
{
    public static BossIntroManager Instance;

    public bool IsIntroPlaying { get; private set; }

    [Header("Boss Reference")]
    [SerializeField] private GameObject boss;

    [Header("UI Panel")]
    [SerializeField] private GameObject introPanel;

    private bool keyPressed = false;
    private bool introTriggered = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Detect boss destruction
        if (!introTriggered && (boss == null || !boss.activeInHierarchy))
        {
            introTriggered = true;
            PlayIntroSequence();
        }

        // Detect key press to close panel
        if (IsIntroPlaying && Input.anyKeyDown)
        {
            keyPressed = true;
        }
    }

    private void PlayIntroSequence()
    {
        if (IsIntroPlaying) return;

        IsIntroPlaying = true;

        // Lock player
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = false;

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Show panel
        introPanel.SetActive(true);

        // Play audio
        AudioManager.Instance.PlayIntro1();

        // Wait until key press
        yield return new WaitUntil(() => keyPressed);

        introPanel.SetActive(false);

        StartGame();
    }

    private void StartGame()
    {
        IsIntroPlaying = false;
        keyPressed = false;

        // Unlock player
        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;

        Debug.Log("Boss Intro Finished. Game Resumed.");
    }
}