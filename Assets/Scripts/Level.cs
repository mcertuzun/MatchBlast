using UnityEngine;

namespace Core
{
    public class Level : MonoBehaviour 
    {
        public LevelDataFactory.LevelName CurrentLevel;
        public Board Board;
        public FallAndFillManager FallAndFillManager;
        [Header("Deterministic Seed")]
        public int Seed = 12345;

        private LevelData _levelData;

        void Start ()
        {
            RandomService.SetSeed(Seed);

            PrepareBoard();
            PrepareLevel();
            StartFalls();

            if (Game.Instance != null)
            {
                Game.Instance.BeginRecording();
            }
        }

        private void PrepareLevel()
        {
            _levelData = LevelDataFactory.CreateLevelData(CurrentLevel);

            for (var y = 0; y < _levelData.GridData.GetLength(0); y++)
            {
                for (var x = 0; x < _levelData.GridData.GetLength(1); x++)
                {
                    var cell = Board.Cells[x, y];

                    var itemType = _levelData.GridData[x, y];
                    var item = ItemFactory.CreateItem(itemType, Board.ItemsParent);
                    if (item == null) continue;					

                    cell.Item = item;
                    item.transform.position = cell.transform.position;
                }
            }

            if (_levelData.FillingColumns != null)
            {
                var topRow = Board.boardData.rowCount - 1;
                for (var x = 0; x < Board.boardData.columnCount && x < _levelData.FillingColumns.Length; x++)
                {
                    if (_levelData.FillingColumns[x])
                    {
                        var fillCell = Board.Cells[x, topRow];
                        if (fillCell != null)
                        {
                            fillCell.IsFillingCell = true;
                        }
                    }
                }
            }
        }

        private void PrepareBoard()
        {
            Board.Prepare();
        }

        private void StartFalls()
        {
            FallAndFillManager.Init(Board, _levelData);
            FallAndFillManager.StartFalls();
        }

        public void RestartLevel()
        {
            ClearBoardItems();
            RandomService.SetSeed(Seed);
            PrepareLevel();
            FallAndFillManager.Init(Board, _levelData);
            FallAndFillManager.StartFalls();

            
           Game.Instance.BeginRecording();
        }

        private void ClearBoardItems()
        {
            var rows = Board.boardData.rowCount;
            var cols = Board.boardData.columnCount;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var cell = Board.Cells[x, y];
                    if (cell == null) continue;
                    if (cell.Item != null)
                    {
                        cell.Item.RemoveItem();
                    }
                    cell.BusyTicks = 0;
                }
            }
        }
    }
}