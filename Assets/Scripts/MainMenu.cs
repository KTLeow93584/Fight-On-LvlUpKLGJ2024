using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class MainMenu : MonoBehaviour
{
    // ---------------------------------
    public enum MenuState
    {
        PRESS_ANYTHING,
        START,
        HOW_TO_PLAY,
        CREDITS,
        QUIT,
    };
    public MenuState menuState = MenuState.PRESS_ANYTHING;
    // ---------------------------------
    [SerializeField]
    private GameObject pressAnythingGO = null;

    [SerializeField]
    private GameObject menuSelectionContainerGO = null;

    [SerializeField]
    private GameObject howToPlayContainerGO = null;

    [SerializeField]
    private GameObject creditsContainerGO = null;
    // ---------------------------------
    [SerializeField]
    private TextMeshProUGUI continueText = null;

    [SerializeField]
    private float fadeSpeed = 0.0f;

    [SerializeField]
    private float pauseDelay = 0.0f;
    private float pauseTime = 0.0f;

    private bool startPause = false;
    private bool isFadingIn = false;
    // ---------------------------------
    [SerializeField]
    private AudioSource transitionSFX = null;

    [SerializeField]
    private AudioSource submitSFX = null;

    [SerializeField]
    private AudioSource backSFX = null;

    [SerializeField]
    private Image fadeImage = null;

    [SerializeField]
    private float transitionFadeSpeed = 0.0f;

    [SerializeField]
    private string characterSelectionSceneName = "";
    // ---------------------------------
    private Coroutine transitionCoroutine = null;
    // ---------------------------------
    void Awake()
    {
        UpdateMenuState(MenuState.PRESS_ANYTHING);
    }
    // ---------------------------------
    // Update is called once per frame
    void Update()
    {
        // -------------------
        if (menuState == MenuState.PRESS_ANYTHING)
        {
            // Continue Text Animating
            if (startPause)
            {
                pauseTime += Time.deltaTime;
                if (pauseTime > pauseDelay)
                {
                    pauseTime = 0.0f;
                    startPause = false;
                }
            }
            else
            {
                float newAlpha = continueText.color.a;
                float fadeValue = (fadeSpeed * Time.deltaTime);

                if (isFadingIn)
                {
                    newAlpha += fadeValue;
                    if (newAlpha >= 1.0f)
                    {
                        isFadingIn = !isFadingIn;
                        continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 1.0f);
                        startPause = true;
                    }
                }
                else
                {
                    newAlpha -= fadeValue;
                    if (newAlpha <= 0.0f)
                    {
                        isFadingIn = !isFadingIn;
                        continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.0f);
                        startPause = true;
                    }
                }

                if (newAlpha != continueText.color.a)
                    continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, newAlpha);
            }
            // -------------------
            // Transition to Character Selection Scene
            if (Input.anyKeyDown)
            {
                if (submitSFX)
                    submitSFX.Play();

                UpdateMenuState(MenuState.START);
            }
            // -------------------
        }
        // -------------------
    }
    // ---------------------------------
    public void StartGame()
    {
        if (transitionCoroutine == null)
            transitionCoroutine = StartCoroutine("TransitionToCharacterSelection");
    }

    public void QuitGame()
    {
        if (transitionCoroutine == null)
            transitionCoroutine = StartCoroutine("TransitionToQuit");
    }

    public void MoveToHowToPlay()
    {
        UpdateMenuState(MenuState.HOW_TO_PLAY);
    }

    public void MoveToCredits()
    {
        if (submitSFX)
            submitSFX.Play();

        UpdateMenuState(MenuState.CREDITS);
    }

    public void MoveToMenu()
    {
        if (backSFX)
            backSFX.Play();
        UpdateMenuState(MenuState.START);
    }
    // ---------------------------------
    public void UpdateMenuState(MenuState newState)
    {
        menuState = newState;

        if (pressAnythingGO)
            pressAnythingGO.SetActive(menuState == MenuState.PRESS_ANYTHING);

        if (menuSelectionContainerGO)
            menuSelectionContainerGO.SetActive(menuState == MenuState.START);

        if (howToPlayContainerGO)
            howToPlayContainerGO.SetActive(menuState == MenuState.HOW_TO_PLAY);

        if (creditsContainerGO)
            creditsContainerGO.SetActive(menuState == MenuState.CREDITS);
    }
    // ---------------------------------
    IEnumerator TransitionToCharacterSelection()
    {
        if (transitionSFX)
            transitionSFX.Play();

        if (fadeImage)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0.0f);
            if (!fadeImage.gameObject.activeSelf)
                fadeImage.gameObject.SetActive(true);

            while (fadeImage.color.a < 1.0f)
            {
                fadeImage.color = new Color(
                    fadeImage.color.r,
                    fadeImage.color.g,
                    fadeImage.color.b,
                    Mathf.Clamp(fadeImage.color.a + (transitionFadeSpeed * Time.deltaTime), 0.0f, 1.0f)
                );
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(characterSelectionSceneName);
    }
    // ---------------------------------
    IEnumerator TransitionToQuit()
    {
        if (transitionSFX)
            transitionSFX.Play();

        if (fadeImage)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0.0f);
            if (!fadeImage.gameObject.activeSelf)
                fadeImage.gameObject.SetActive(true);

            while (fadeImage.color.a < 1.0f)
            {
                fadeImage.color = new Color(
                    fadeImage.color.r,
                    fadeImage.color.g,
                    fadeImage.color.b,
                    Mathf.Clamp(fadeImage.color.a + (transitionFadeSpeed * Time.deltaTime), 0.0f, 1.0f)
                );
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForSeconds(0.5f);

        Application.Quit();
    }
    // ---------------------------------
}
