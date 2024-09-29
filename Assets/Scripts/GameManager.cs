using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;
using System.Linq;

public enum InputMapper
{
    UP = 1,
    DOWN = 2,
    LEFT = 3,
    RIGHT = 4,
    PUNCH = 5,
    KICK = 6,
    CHARGE = 7,
    NOINPUT = 8
};

public enum GameState
{
    START = 0,
    END = 1,
};

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private float gameTime = 60.0f;

    [SerializeField]
    private TextMeshProUGUI timerTextComponent = null;

    [SerializeField]
    private TextMeshProUGUI gameEndTextComponent = null;

    [SerializeField]
    private Transform playersParentTransform = null;

    [SerializeField]
    private Vector3 p1StartingPosition = Vector3.zero;

    [SerializeField]
    private Vector3 p2StartingPosition = Vector3.zero;

    [SerializeField]
    private Image fadeImage = null;

    [SerializeField]
    private float transitionFadeSpeed = 1.0f;

    [SerializeField]
    private string characterSelectSceneName = "";

    [SerializeField]
    private GameState currentState = GameState.START;

    [SerializeField]
    private Player_Character player1 = null;

    [SerializeField]
    private Player_Character player2 = null;

    Coroutine gameEndCoroutine = null;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            InitializePlayerCharacters();
        }
        else
            Destroy(this.gameObject);
    }

    private void InitializePlayerCharacters()
    {
        if (DataManager.instance)
        {
            Player_Character characterScript = null;

            GameObject player1GO = Instantiate(DataManager.instance.player1.targetPrefab);
            player1GO.transform.SetParent(playersParentTransform);
            player1GO.transform.position = p1StartingPosition;
            player1GO.name = DataManager.instance.player2.characterName + "_P1";

            characterScript = player1GO.GetComponent<Player_Character>();
            characterScript.assignedPlayerNum = 1;
            characterScript.OnInit(1);

            GameObject player2GO = Instantiate(DataManager.instance.player2.targetPrefab);
            player2GO.transform.SetParent(playersParentTransform);
            player2GO.transform.position = p2StartingPosition;
            player2GO.name = DataManager.instance.player2.characterName + "_P2";

            characterScript = player2GO.GetComponent<Player_Character>();
            characterScript.assignedPlayerNum = 2;
            characterScript.OnInit(2);
        }
    }

    private void Start()
    {
        Transform[] allTransforms = FindObjectsOfType<Transform>(true);
        string[] targetName = { "_P1", "_P2" };
        foreach (string iterName in targetName)
        {
            Transform playerTransform = allTransforms.FirstOrDefault(obj => obj.name.EndsWith(iterName) && obj.GetComponent<Player_Character>() != null);

            if (iterName == "_P1")
                player1 = playerTransform.GetComponent<Player_Character>();
            else
                player2 = playerTransform.GetComponent<Player_Character>();
        }

        if (gameEndTextComponent && gameEndTextComponent.gameObject.activeSelf)
            gameEndTextComponent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentState != GameState.START)
            return;

        gameTime -= Time.deltaTime;
        if (gameTime < 0)
        {
            gameTime = 0;

            if (currentState == GameState.START)
                OnGameEnded();
        }

        timerTextComponent.text = Mathf.Ceil(gameTime).ToString();
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    // TODO: Decide Winner then move to Victor Scene
    public void OnGameEnded()
    {
        if (gameEndCoroutine == null)
            gameEndCoroutine = StartCoroutine("TransitionGameEndState");
    }

    IEnumerator TransitionGameEndState()
    {
        yield return new WaitForSeconds(1.0f);

        currentState = GameState.END;

        string textStr = "";

        if (!player1.IsAlive())
        {
            player2.OnVictory();
            textStr = "Player 2 Wins!";
        }
        else if (!player2.IsAlive())
        {
            player1.OnVictory();
            textStr = "Player 1 Wins!";
        }
        else
        {
            if (Mathf.Ceil(player1.GetHealth()) > Mathf.Ceil(player2.GetHealth()))
            {
                player1.OnVictory();
                player2.OnForceDefeat();

                textStr = "Player 1 Wins!";
            }
            else if (Mathf.Ceil(player2.GetHealth()) > Mathf.Ceil(player1.GetHealth()))
            {
                player2.OnVictory();
                player1.OnForceDefeat();

                textStr = "Player 2 Wins!";
            }
            else
            {
                player1.OnMatchTied();
                player2.OnMatchTied();
                textStr = "Draw!";
            }
        }

        if (gameEndTextComponent && !gameEndTextComponent.gameObject.activeSelf)
            gameEndTextComponent.gameObject.SetActive(true);

        gameEndTextComponent.text = textStr;

        yield return new WaitForSeconds(3.0f);

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

        // Debug
        //Debug.Log("Game Has Ended, Refreshing Scene.");

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(characterSelectSceneName);
    }
}
