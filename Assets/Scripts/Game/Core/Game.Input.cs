using UnityEngine;

namespace Core
{
    public partial class Game
    {
        private void UpdateInput()
        {
            if (IsReplaying)
            {
                return;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                ProcessScreenPosition(Input.mousePosition);
            }
#endif

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    ProcessScreenPosition(touch.position);
                }
            }
        }

        private void ProcessScreenPosition(Vector3 screenPos)
        {
            if (Camera == null || Board == null)
            {
                return;
            }

            var world = Camera.ScreenToWorldPoint(screenPos);
            var local = Board.transform.InverseTransformPoint(world);

            int x = Mathf.FloorToInt(local.x / CellSize);
            int y = Mathf.FloorToInt(local.y / CellSize);

            if (x < 0 || y < 0 ||
                x >= Board.boardData.columnCount ||
                y >= Board.boardData.rowCount)
            {
                return;
            }

            var action = new PlayerAction
            {
                Time = 0f,
                Type = ActionType.Tap,
                CellX = x,
                CellY = y
            };

            if (IsRecording)
            {
                RecordAction(action);
            }
            else if (ActionQueue != null)
            {
                ActionQueue.Enqueue(action);
            }
        }
    }
}
