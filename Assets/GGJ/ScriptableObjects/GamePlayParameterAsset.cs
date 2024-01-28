using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePlayParameterData", menuName = "ScriptableObjects/CreateGamePlayParameter")]
public class GamePlayParameterAsset : ScriptableObject
{
    public float remainingTime = 10.0f;
    public float playerBarMoveTime = 1.0f;
    public float playerChargeSeconds = 3.0f;
    public float playerFirePower = 4.0f;
}
