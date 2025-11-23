using UnityEngine;

namespace Core
{
    public class Level : MonoBehaviour
    {
        public LevelName CurrentLevel;
        public Board Board;
        public FallAndFillManager FallAndFillManager;
        [Header("Deterministic Seed")] public int Seed = 12345;

        private LevelData _levelData;

        private void Start()
        {
            RandomService.SetSeed(Seed);

            PrepareBoard();
            PrepareLevel();
            StartFalls();

            if (Game.Instance != null) Game.Instance.BeginRecording();
        }

        private void PrepareLevel()
        {
            _levelData = LevelDataFactory.CreateLevelData(CurrentLevel);

            for (int y = 0; y < _levelData.GridData.GetLength(0); y++)
            for (int x = 0; x < _levelData.GridData.GetLength(1); x++)
            {
                Cell cell = Board.Cells[x, y];

                ItemType itemType = _levelData.GridData[x, y];
                Item item = ItemFactory.CreateItem(itemType, Board.ItemsParent);
                if (item == null) continue;

                cell.Item = item;
                item.transform.position = cell.transform.position;
            }

            if (_levelData.FillingColumns != null)
            {
                int topRow = Board.boardData.rowCount - 1;
                for (int x = 0; x < Board.boardData.columnCount && x < _levelData.FillingColumns.Length; x++)
                    if (_levelData.FillingColumns[x])
                    {
                        Cell fillCell = Board.Cells[x, topRow];
                        if (fillCell != null) fillCell.IsFillingCell = true;
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
        }

        private void ClearBoardItems()
        {
            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                Cell cell = Board.Cells[x, y];
                if (cell == null) continue;
                if (cell.Item != null) cell.Item.RemoveItem();
                cell.BusyTicks = 0;
            }
        }
    }

    public enum LevelName
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5_1,
        Level5_2,
        LevelTest1,
        LevelTest2
    }
}