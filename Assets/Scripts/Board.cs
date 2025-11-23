using System;
using UnityEngine;

namespace Core
{
    public class Board : MonoBehaviour
    {
        public Cell[,] Cells = new Cell[Rows, Cols];
        [SerializeField] public BoardData boardData;
        public Transform ItemsParent;
        public const int Rows = 9;
        public const int Cols = 9;

        public void Prepare()
        {
            CreateCells();
            PrepareCells();
        }

        private void CreateCells()
        {
            for (int y = 0; y < boardData.rowCount; y++)
            for (int x = 0; x < boardData.columnCount; x++)
            {
                Cell cell = Instantiate(boardData.cellPrefab);
                Cells[x, y] = cell;
            }
        }

        private void PrepareCells()
        {
            for (int y = 0; y < boardData.rowCount; y++)
            for (int x = 0; x < boardData.columnCount; x++)
                Cells[x, y].Prepare(x, y, this);
        }

        public Cell GetNeighbourWithDirection(Cell cell, Direction direction)
        {
            int x = cell.X;
            int y = cell.Y;
            switch (direction)
            {
                case Direction.None:
                    break;
                case Direction.Up:
                    y += 1;
                    break;
                case Direction.UpRight:
                    y += 1;
                    x += 1;
                    break;
                case Direction.Right:
                    x += 1;
                    break;
                case Direction.DownRight:
                    y -= 1;
                    x += 1;
                    break;
                case Direction.Down:
                    y -= 1;
                    break;
                case Direction.DownLeft:
                    y -= 1;
                    x -= 1;
                    break;
                case Direction.Left:
                    x -= 1;
                    break;
                case Direction.UpLeft:
                    y += 1;
                    x -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            if (x >= boardData.columnCount || x < 0 || y >= boardData.rowCount || y < 0) return null;

            return Cells[x, y];
        }
    }

    public enum Direction
    {
        None = -1,
        Up = 0,
        UpRight = 1,
        Right = 2,
        DownRight = 3,
        Down = 4,
        DownLeft = 5,
        Left = 6,
        UpLeft = 7
    }
}