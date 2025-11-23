using UnityEngine;

namespace Core
{
    public partial class Game : MonoBehaviour
    {
        [Header("References")]
        public Board Board;
        public FallAndFillManager FallAndFillManager;
        public ActionQueue ActionQueue;
        public Level Level;

        [Header("Input")]
        public Camera Camera;
        public float CellSize = 1f;

        private float _time;

        public static Game Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            if (Camera == null)
            {
                Camera = Camera.main;
            }
        }

        private void Start()
        {
            // Level kendi Start'ında boardu hazırlar.
        }

        private void Update()
        {
            UpdateInput();
        }

        private void FixedUpdate()
        {
            _time += Time.fixedDeltaTime;
            TickReplay();
            TickGravity();
        }
    }
}
