using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class FallAndFillManager : MonoBehaviour
    {
        private bool _isActive;
        private Board _board;
        private LevelData _levelData;
        private Cell[] _fillingCells;

        public void Init(Board board, LevelData levelData)
        {
            _board = board;
            _levelData = levelData;
            CreateFillingCells();
        }

        private void CreateFillingCells()
        {
            List<Cell> cellList = new();
            for (int y = 0; y < _board.boardData.rowCount; y++)
            for (int x = 0; x < _board.boardData.columnCount; x++)
            {
                Cell cell = _board.Cells[x, y];
                if (cell != null && cell.IsFillingCell) cellList.Add(cell);
            }

            _fillingCells = cellList.ToArray();
        }

        public void StartFalls()
        {
            _isActive = true;
        }

        public void StopFalls()
        {
            _isActive = false;
        }

        private void DoFalls()
        {
            int rows = _board.boardData.rowCount;
            int cols = _board.boardData.columnCount;

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                Cell cell = _board.Cells[x, y];
                if (cell == null) continue;

                Item item = cell.Item;
                if (item == null) continue;

                Cell below = cell.FirstCellBelow;
                if (below != null && !below.IsBusy && below.Item == null) item.Fall();
            }
        }

        private void DoFills()
        {
            CubeSpawner spawner = ServiceProvider.GetCubeSpawner;

            for (int i = 0; i < _fillingCells.Length; i++)
            {
                Cell cell = _fillingCells[i];
                if (cell == null) continue;

                if (cell.Item == null)
                {
                    Item item = null;

                    if (spawner != null)
                        item = spawner.SpawnFromLevelData(_levelData, _board.ItemsParent);
                    else
                        item = ItemFactory.CreateItem(_levelData.GetNextFillItemType(), _board.ItemsParent);

                    cell.Item = item;

                    float offsetY = 0.0f;
                    Cell targetCellBelow = cell.GetFallTarget().FirstCellBelow;
                    if (targetCellBelow != null)
                        if (targetCellBelow.Item != null)
                            offsetY = targetCellBelow.Item.transform.position.y + 1;

                    Vector3 p = cell.transform.position;
                    p.y += 2;
                    p.y = p.y > offsetY ? p.y : offsetY;

                    if (!cell.HasItem()) continue;
                    cell.Item.transform.position = p;
                    cell.Item.Fall();
                }
            }
        }

        public void TickLogic()
        {
            if (!_isActive) return;
            DoFalls();
            DoFills();
        }

        private void Update() { }
    }
}