using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

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

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private float gameTime = 60.0f;

    [SerializeField]
    private TextMeshProUGUI timerTextComponent = null;

    void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Update()
    {
        gameTime -= Time.deltaTime;
        if (gameTime < 0)
        {
            gameTime = 0;
            OnGameEnded();
        }

        timerTextComponent.text = Mathf.Ceil(gameTime).ToString();
    }

    // TODO: Decide Winner then move to Victor Scene
    void OnGameEnded()
    {
        // Debug
        Debug.Log("Game Has Ended, Refreshing Scene.");
        Scene activeScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(activeScene.name);
    }
}
