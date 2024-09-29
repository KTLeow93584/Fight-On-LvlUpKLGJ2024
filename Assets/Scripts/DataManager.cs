using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;

    [SerializeField]
    private Level_Data selectedLevel = null;

    [SerializeField]
    public Character_Data player1 = null;

    [SerializeField]
    public Character_Data player2 = null;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 90;
    }

    public void SetPlayer1CharacterData(Character_Data player)
    {
        player1 = player;
    }

    public void SetPlayer2CharacterData(Character_Data player)
    {
        player2 = player;
    }

    public void SetLevelData(Level_Data level)
    {
        selectedLevel = level;
    }
}
