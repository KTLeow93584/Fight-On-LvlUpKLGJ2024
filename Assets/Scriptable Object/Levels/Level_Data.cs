using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/CreateLevelData", order = 1)]
public class Level_Data : ScriptableObject
{
    public string sceneName;
    public Sprite levelBG;
}
