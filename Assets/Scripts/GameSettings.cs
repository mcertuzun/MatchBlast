using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameSetting", fileName ="GameSetting" )]
public class GameSettings : ScriptableObject
{
    public float targetFrameRate;
    // void Awake ()
    // {
    //     Application.targetFrameRate = 60;
    // }
}