using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    public partial class Game : MonoBehaviour
    {
        [Header("Debug")] public bool OverrideGizmoToggle;
        public bool GizmosEnabledOverride = true;

        private void OnDrawGizmos()
        {
            GameConfig config = ServiceProvider.GetGameConfig;
            bool enabled = true;
            float size = 0.9f;

            if (config != null)
            {
                enabled = config.EnableCellGizmos;
                size = config.CellGizmoSize;
            }

            if (OverrideGizmoToggle) enabled = GizmosEnabledOverride;

            if (!enabled)
                return;

            if (Board == null || Board.boardData == null)
                return;

            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            Gizmos.matrix = Board.transform.localToWorldMatrix;

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                Cell cell = Board.Cells[x, y];
                Vector3 center;

                if (cell != null)
                {
                    Vector3 localPos = Board.transform.InverseTransformPoint(cell.transform.position);
                    center = new Vector3(localPos.x, localPos.y, 0f);
                }
                else
                {
                    center = new Vector3(x, y, 0f);
                }

                Color c = Color.gray;

                if (cell != null && cell.HasItem())
                {
                    Item item = cell.Item;
                    GameConfig gc = config;
                    if (gc != null && item != null)
                        switch (item.GetItemType())
                        {
                            case ItemType.GreenCube: c = gc.GreenColor; break;
                            case ItemType.YellowCube: c = gc.YellowColor; break;
                            case ItemType.BlueCube: c = gc.BlueColor; break;
                            case ItemType.RedCube: c = gc.RedColor; break;
                            case ItemType.Bomb: c = gc.BombColor; break;
                            case ItemType.RocketH:
                            case ItemType.RocketV: c = gc.RocketColor; break;
                            default: c = Color.white; break;
                        }
                    else
                        c = Color.white;
                }

                if (cell != null && cell.BusyTicks > 0) c = Color.Lerp(c, Color.yellow, 0.6f);

                Gizmos.color = new Color(c.r, c.g, c.b, 0.3f);
                Gizmos.DrawCube(center, new Vector3(size, size, 0.01f));
                Gizmos.color = c;
                Gizmos.DrawWireCube(center, new Vector3(size, size, 0.01f));

#if UNITY_EDITOR
                Handles.color = Color.white;
                Vector3 labelPos = center + Vector3.up * 0.2f;
                Handles.Label(Board.transform.TransformPoint(labelPos), x + "," + y);
#endif
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}