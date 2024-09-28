using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "ScriptableObjects/CreateCharaterData", order = 1)]
public class Character_Data : ScriptableObject
{
    public string characterName;
    public GameObject targetPrefab;

    public Sprite portraitSprite;
    public Sprite iconSprite;
}
