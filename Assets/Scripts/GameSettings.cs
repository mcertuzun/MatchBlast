using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameSetting", fileName ="GameSetting" )]
public class GameSettings : ScriptableObject
{
    public int targetFrameRate = 60;
    void Awake ()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}