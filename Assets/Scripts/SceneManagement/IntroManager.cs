using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance;

    public bool IsIntroPlaying { get; private set; }

    [SerializeField] private GameObject introPanel1;
    [SerializeField] private GameObject introPanel2;

    [SerializeField] private float intro1Duration = 3f;
    [SerializeField] private float intro2Duration = 3f;

    private bool skipRequested = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IsIntroPlaying = true;
        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        // Detect ANY key press
        if (Input.anyKeyDown)
        {
            skipRequested = true;
        }
    }

    private IEnumerator PlayIntro()
    {
        // PANEL 1
        introPanel1.SetActive(true);
        AudioManager.Instance.PlayIntro1();

        yield return WaitOrSkip(intro1Duration);

        introPanel1.SetActive(false);
        skipRequested = false;

        // PANEL 2
        introPanel2.SetActive(true);
        AudioManager.Instance.PlayIntro2();

        yield return WaitOrSkip(intro2Duration);

        introPanel2.SetActive(false);

        StartGame();
    }

    private IEnumerator WaitOrSkip(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (skipRequested)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void StartGame()
    {
        IsIntroPlaying = false;
        Debug.Log("Intro Finished. Game Playing = " + IsIntroPlaying);
    }
}