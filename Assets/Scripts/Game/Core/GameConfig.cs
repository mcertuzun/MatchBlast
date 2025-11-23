using UnityEngine;

namespace Core
{
    public class GameConfig : MonoBehaviour, IProvidable
    {
        [Header("Match / Booster thresholds")] public int MinMatchForBlast = 2;
        public int MinMatchForBooster = 5;

        [Header("Busy durations (ticks)")] public int DefaultBusyDuration = 3;
        public int BombBusyDuration = 4;
        public int RocketBusyDuration = 3;

        [Header("Replay / Gravity")] public int GravityTicksBetweenActions = 1;

        [Header("Fall animation")] public float FallCellsPerSecond = 8f;
        public float FallMinDuration = 0.08f;
        public float FallMaxDuration = 0.35f;

        [Header("Colors")] public Color GreenColor = Color.green;
        public Color YellowColor = Color.yellow;
        public Color BlueColor = Color.cyan;
        public Color RedColor = Color.red;

        public Color BombColor = new(0.9f, 0.2f, 0.2f);
        public Color RocketColor = new(0.6f, 0.0f, 1.0f);

        [Header("Debug")] public bool EnableCellGizmos = true;
        public float CellGizmoSize = 0.9f;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }
    }
}