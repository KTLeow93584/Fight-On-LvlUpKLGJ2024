using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class Character_Selection : MonoBehaviour
{
    // -------------------------------------
    [SerializeField]
    private List<Character_Data> characterList = new List<Character_Data>();

    [SerializeField]
    private GameObject gridElementUIPrefab = null;

    [SerializeField]
    private Transform prefabParent = null;

    private List<GameObject> gridElementCellList = new List<GameObject>();
    // -------------------------------------
    const int NUMBER_OF_CHARACTERS_PER_ROW = 9;
    // -------------------------------------
    [SerializeField]
    private Image p1_selectorOutline = null;

    [SerializeField]
    private TextMeshProUGUI p1_selectorText = null;

    [SerializeField]
    private Image p1_portrait = null;

    [SerializeField]
    private TextMeshProUGUI p1_nameText = null;

    [SerializeField]
    private Color p1_submitOutlineColor = Color.white;

    [SerializeField]
    private Image p2_selectorOutline = null;

    [SerializeField]
    private TextMeshProUGUI p2_selectorText = null;

    [SerializeField]
    private Image p2_portrait = null;

    [SerializeField]
    private TextMeshProUGUI p2_nameText = null;

    [SerializeField]
    private Color p2_submitOutlineColor = Color.white;
    // -------------------------------------
    [SerializeField]
    private List<Level_Data> levelList = new List<Level_Data>();

    [SerializeField]
    private GameObject levelSelectorGO = null;

    [SerializeField]
    private Image levelSelectorBGPanel = null;

    private int levelIndex = 0;
    private bool isLevelSwitched = false;

    [SerializeField]
    private Image fadeImage = null;

    [SerializeField]
    private float transitionFadeSpeed = 0.0f;

    [SerializeField]
    private string mainMenuSceneName = "";
    // -------------------------------------
    [SerializeField]
    private AudioSource submitSFX = null;

    [SerializeField]
    private AudioSource cancelSFX = null;

    [SerializeField]
    private AudioSource transitionSFX = null;

    [SerializeField]
    private AudioSource gridMovementSFX = null;
    // -------------------------------------
    //[SerializeField]
    private int p1_gridIndexX = 0;

    //[SerializeField]
    private int p1_gridIndexY = 0;

    private bool p1_lock = false;
    private Color p1_selectorColor = Color.white;
    private Coroutine p1_confirmCoroutine;

    //[SerializeField]
    private int p2_gridIndexX = 1;

    //[SerializeField]
    private int p2_gridIndexY = 0;

    private bool p2_lock = false;
    private Color p2_selectorColor = Color.white;
    private Coroutine p2_confirmCoroutine;
    // -------------------------------------
    private bool isExitingScene = false;
    // -------------------------------------
    void Start()
    {
        if (!p1_selectorOutline.gameObject.activeSelf)
            p1_selectorOutline.gameObject.SetActive(true);

        if (!p2_selectorOutline.gameObject.activeSelf)
            p2_selectorOutline.gameObject.SetActive(true);

        if (p1_selectorText && !p1_selectorText.gameObject.activeSelf)
            p1_selectorText.gameObject.SetActive(true);

        if (p2_selectorText && !p2_selectorText.gameObject.activeSelf)
            p2_selectorText.gameObject.SetActive(true);

        p2_gridIndexX = 1;

        SetP1Outline((p1_gridIndexY * NUMBER_OF_CHARACTERS_PER_ROW) + p1_gridIndexX);
        SetP2Outline((p2_gridIndexY * NUMBER_OF_CHARACTERS_PER_ROW) + p2_gridIndexX);

        if (gridElementUIPrefab)
        {
            for (int i = 0; i < characterList.Count; ++i)
            {
                GameObject cell = Instantiate(gridElementUIPrefab);
                cell.transform.SetParent(prefabParent, false);
                RectTransform cellRTransform = cell.GetComponent<RectTransform>();

                cell.name = "Character Cell_" + characterList[i].characterName;

                cellRTransform.anchoredPosition = new Vector2(
                    (64 * (i % NUMBER_OF_CHARACTERS_PER_ROW)),
                    (64 * Mathf.Floor(i / NUMBER_OF_CHARACTERS_PER_ROW))
                );

                Image icon = cell.transform.Find("Panel").GetComponent<Image>();
                icon.sprite = characterList[i].iconSprite;
            }
        }

        p1_portrait.sprite = characterList[0].portraitSprite;
        p2_portrait.sprite = characterList[1].portraitSprite;

        p1_nameText.text = characterList[0].characterName;
        p2_nameText.text = characterList[1].characterName;

        if (p1_selectorText)
            p1_selectorText.GetComponent<RectTransform>().SetAsLastSibling();

        if (p2_selectorText)
            p2_selectorText.GetComponent<RectTransform>().SetAsLastSibling();

        if (levelSelectorGO.activeSelf)
            levelSelectorGO.SetActive(false);
    }

    void Update()
    {
        // ---------------------
        if (isExitingScene)
            return;
        // ---------------------
        // Level Selection Section
        if (levelSelectorGO.gameObject.activeSelf)
        {
            // Player 1 or Player 2 Cancels
            if (Input.GetAxis("Cancel_P1") > 0 || Input.GetAxis("Cancel_P2") > 0)
            {
                levelSelectorGO.gameObject.SetActive(false);

                CancelLockIn(0);
                CancelLockIn(1);

                if (cancelSFX)
                    cancelSFX.Play();
            }

            // Player 1 or Player 2 Move to Next Level
            if (Input.GetAxis("Horizontal_P1") > 0 || Input.GetAxis("Horizontal_P2") > 0)
            {
                if (!isLevelSwitched)
                {
                    isLevelSwitched = true;
                    MoveToNextLevel();
                }
            }
            // Player 1 or Player 2 Move to Previous Level
            else if (Input.GetAxis("Horizontal_P1") < 0 || Input.GetAxis("Horizontal_P2") < 0)
            {
                if (!isLevelSwitched)
                {
                    isLevelSwitched = true;
                    MoveToPreviousLevel();
                }
            }
            else
            {
                if (isLevelSwitched)
                    isLevelSwitched = false;
            }

            // Level Selected
            if (Input.GetAxis("Submit_P1") > 0 || Input.GetAxis("Submit_P2") > 0)
                LoadTargetLevel();
        }
        // ---------------------
        // Character Selection Section
        else
        {
            // Player 1 or Player 2 Cancels - Exit to Main Menu
            if (!p1_lock && !p2_lock && (Input.GetAxis("Cancel_P1") > 0 || Input.GetAxis("Cancel_P2") > 0))
            {
                LoadMenu();
                return;
            }
            // ------------------------
            if (!p1_lock)
                MapP1GridMovement();

            if (!p2_lock)
                MapP2GridMovement();
            // ------------------------
            // Player 1 Lock In Character
            if (Input.GetAxis("Submit_P1") > 0)
            {
                if (!p1_lock)
                {
                    p1_lock = true;
                    p1_confirmCoroutine = StartCoroutine(OnLockIn(0));
                    SelectPlayerP1();
                }
            }
            // Player 1 Cancel Lock In Character
            else if (Input.GetAxis("Cancel_P1") > 0)
                CancelLockIn(0, true);
            // ------------------------
            // Player 2 Lock In Character
            if (Input.GetAxis("Submit_P2") > 0)
            {
                if (!p2_lock)
                {
                    p2_lock = true;
                    p2_confirmCoroutine = StartCoroutine(OnLockIn(1));
                    SelectPlayerP2();
                }
            }
            // Player 2 Cancel Lock In Character
            else if (Input.GetAxis("Cancel_P2") > 0)
                CancelLockIn(1, true);
            // ------------------------
            p1_selectorColor = p1_selectorOutline.color;
            p2_selectorColor = p2_selectorOutline.color;
        }
        // ---------------------
    }
    // -------------------------------------
    private void MapP1GridMovement()
    {
        if (Input.GetKeyDown(KeyCode.W))
            MovePlayer1(0, 1);
        if (Input.GetKeyDown(KeyCode.S))
            MovePlayer1(0, -1);
        if (Input.GetKeyDown(KeyCode.A))
            MovePlayer1(-1, 0);
        if (Input.GetKeyDown(KeyCode.D))
            MovePlayer1(1, 0);
    }

    private void MovePlayer1(int xOffset, int yOffset)
    {
        // [PREVENT OVERLAP]
        // Both can't be on the same tile.
        //if (p1_gridIndexX + xOffset == p2_gridIndexX && p1_gridIndexY + yOffset == p2_gridIndexY)
        //{
        //    if (xOffset != 0)
        //        xOffset *= 2;
        //    if (yOffset != 0)
        //        yOffset *= 2;
        //}

        int characterIndexAt = (p1_gridIndexY + yOffset * NUMBER_OF_CHARACTERS_PER_ROW) + (p1_gridIndexX + xOffset);

        if (characterIndexAt >= 0 && characterIndexAt < characterList.Count)
        {
            // Debug
            //Debug.Log("[P1] Character Index: " + characterIndexAt + ", Max Count: " + characterList.Count);

            p1_gridIndexX += xOffset;
            p1_gridIndexY += yOffset;
            p1_portrait.sprite = characterList[characterIndexAt].portraitSprite;
            p1_nameText.text = characterList[characterIndexAt].characterName;

            if (gridMovementSFX)
                gridMovementSFX.Play();

            SetP1Outline(characterIndexAt);
        }
    }

    private void SetP1Outline(int characterIndexAt)
    {
        if (!p1_selectorOutline)
            return;

        Vector2 position = new Vector2(
            3 + (64 * (characterIndexAt % NUMBER_OF_CHARACTERS_PER_ROW)),
            -2 + (64 * Mathf.Floor(characterIndexAt / NUMBER_OF_CHARACTERS_PER_ROW))
        );

        p1_selectorOutline.rectTransform.anchoredPosition = position;
        if (p1_selectorText)
            p1_selectorText.rectTransform.anchoredPosition = position;
    }

    private void SelectPlayerP1()
    {
        Character_Data characterData = characterList[p1_gridIndexY * NUMBER_OF_CHARACTERS_PER_ROW + p1_gridIndexX];
        if (DataManager.instance)
            DataManager.instance.SetPlayer1CharacterData(characterData);

        // Debug
        //Debug.Log("Player 1 has locked into: " + characterData);
    }
    // -------------------------------------
    private void MapP2GridMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MovePlayer2(0, 1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            MovePlayer2(0, -1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MovePlayer2(-1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MovePlayer2(1, 0);
    }

    private void MovePlayer2(int xOffset, int yOffset)
    {
        // [PREVENT OVERLAP]
        // Both can't be on the same tile.
        //if (p2_gridIndexX + xOffset == p1_gridIndexX && p2_gridIndexY + yOffset == p1_gridIndexY)
        //{
        //    if (xOffset != 0)
        //        xOffset *= 2;
        //    if (yOffset != 0)
        //        yOffset *= 2;
        //}

        int characterIndexAt = (p2_gridIndexY + yOffset * NUMBER_OF_CHARACTERS_PER_ROW) + (p2_gridIndexX + xOffset);

        if (characterIndexAt >= 0 && characterIndexAt < characterList.Count)
        {
            // Debug
            //Debug.Log("[P2] Character Index: " + characterIndexAt + ", Max Count: " + characterList.Count);

            p2_gridIndexX += xOffset;
            p2_gridIndexY += yOffset;
            p2_portrait.sprite = characterList[characterIndexAt].portraitSprite;
            p2_nameText.text = characterList[characterIndexAt].characterName;

            if (gridMovementSFX)
                gridMovementSFX.Play();

            SetP2Outline(characterIndexAt);
        }
    }

    private void SetP2Outline(int characterIndexAt)
    {
        if (!p2_selectorOutline)
            return;

        Vector2 position = new Vector2(
            3 + (64 * (characterIndexAt % NUMBER_OF_CHARACTERS_PER_ROW)),
            -2 + (64 * Mathf.Floor(characterIndexAt / NUMBER_OF_CHARACTERS_PER_ROW))
        );

        p2_selectorOutline.rectTransform.anchoredPosition = position;
        if (p2_selectorText)
            p2_selectorText.rectTransform.anchoredPosition = position;
    }

    private void SelectPlayerP2()
    {
        Character_Data characterData = characterList[p2_gridIndexY * NUMBER_OF_CHARACTERS_PER_ROW + p2_gridIndexX];
        if (DataManager.instance)
            DataManager.instance.SetPlayer2CharacterData(characterData);

        // Debug
        //Debug.Log("Player 2 has locked into: " + characterData);
    }
    // -------------------------------------
    IEnumerator OnLockIn(int playerIndex)
    {
        Image selector = playerIndex == 0 ? p1_selectorOutline : p2_selectorOutline;
        if (submitSFX)
        {
            if (submitSFX.isPlaying)
                submitSFX.Stop();
            submitSFX.Play();
        }
        // Blink "x" times.
        int blinkCount = 0;
        int maxBlinkCount = 8;

        Color[] colorSwap = new Color[2];
        colorSwap[0] = Color.black;
        colorSwap[1] = Color.white;

        Color originalColor = selector.color;

        while (blinkCount < maxBlinkCount)
        {
            selector.color = colorSwap[blinkCount % 2];
            ++blinkCount;
            yield return new WaitForSeconds(0.08f);
        }
        selector.color = playerIndex == 0 ? p1_submitOutlineColor : p2_submitOutlineColor;

        if (p1_lock && p2_lock)
        {
            levelIndex = 0;

            UpdateTargetLevelInfo();
            levelSelectorGO.gameObject.SetActive(true);
        }
    }
    // -------------------------------------
    private void CancelLockIn(int playerIndex, bool playSFX = false)
    {
        // Player 1
        if (playerIndex == 0)
        {
            if (p1_lock)
                p1_lock = false;
            p1_selectorOutline.color = p1_selectorColor;

            StopCoroutine(p1_confirmCoroutine);
            p1_confirmCoroutine = null;
        }
        // Player 2
        else
        {
            if (p2_lock)
                p2_lock = false;
            p2_selectorOutline.color = p2_selectorColor;

            StopCoroutine(p2_confirmCoroutine);
            p2_confirmCoroutine = null;
        }

        if (playSFX && cancelSFX)
        {
            if (cancelSFX.isPlaying)
                cancelSFX.Stop();
            cancelSFX.Play();
        }
    }
    // -------------------------------------
    public void MoveToNextLevel()
    {
        if (levelList.Count == 0)
            return;

        ++levelIndex;
        if (levelIndex >= levelList.Count)
            levelIndex = 0;
        UpdateTargetLevelInfo();
    }

    public void MoveToPreviousLevel()
    {
        if (levelList.Count == 0)
            return;

        --levelIndex;
        if (levelIndex < 0)
            levelIndex = levelList.Count - 1;
        UpdateTargetLevelInfo();
    }

    void UpdateTargetLevelInfo()
    {
        if (levelSelectorBGPanel)
            levelSelectorBGPanel.sprite = levelList[levelIndex].levelBG;

        if (DataManager.instance)
            DataManager.instance.SetLevelData(levelList[levelIndex]);
    }

    void LoadMenu()
    {
        StartCoroutine("TransitionToMainMenu");
    }

    void LoadTargetLevel()
    {
        if (!isExitingScene)
            isExitingScene = true;
        StartCoroutine("TransitionToGameLevel");
    }
    // -------------------------------------
    IEnumerator TransitionToMainMenu()
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

        SceneManager.LoadScene(mainMenuSceneName);
    }
    // ---------------------------------
    IEnumerator TransitionToGameLevel()
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

        SceneManager.LoadScene(levelList[levelIndex].sceneName);
    }
    // ---------------------------------
}
