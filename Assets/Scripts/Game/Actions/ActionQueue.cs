using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum ActionType
    {
        Tap = 0,
        Gravity = 1,
        ScanGroups = 2
    }

    public enum ClickOutcome
    {
        None = 0,
        TooSmall = 1,
        Blast = 2,
        Booster = 3
    }

    [Serializable]
    public struct PlayerAction
    {
        public float Time;
        public ActionType Type;
        public int CellX;
        public int CellY;
    }

    public class ActionQueue : MonoBehaviour
    {
        public Board Board;
        public FallAndFillManager FallAndFillManager;

        private readonly Queue<PlayerAction> _actions = new();
        private readonly List<PlayerAction> _executed = new();

        public void Enqueue(PlayerAction action)
        {
            _actions.Enqueue(action);
        }

        public void Clear()
        {
            _actions.Clear();
            _executed.Clear();
        }

        public void ProcessActions(float currentTime)
        {
            while (_actions.Count > 0 && _actions.Peek().Time <= currentTime)
            {
                PlayerAction action = _actions.Dequeue();
                ExecuteAction(action);
                _executed.Add(action);
            }
        }

        private void ExecuteAction(PlayerAction action)
        {
            switch (action.Type)
            {
                case ActionType.Tap:
                    ExecuteTap(action);
                    break;
                case ActionType.Gravity:
                    TickBusyCells();
                    if (FallAndFillManager != null) FallAndFillManager.TickLogic();
                    break;
                case ActionType.ScanGroups:
                    ScanBoardForGroups();
                    break;
            }
        }

        private void ExecuteTap(PlayerAction action)
        {
            if (Board == null) return;

            if (!IsInsideBoard(action.CellX, action.CellY)) return;

            Cell cell = Board.Cells[action.CellX, action.CellY];
            if (cell == null || !cell.HasItem()) return;

            Item item = cell.Item;
            ItemType itemType = item.GetItemType();

            if (itemType == ItemType.Bomb)
            {
                ExplodeBomb(cell);
                return;
            }

            if (itemType == ItemType.RocketH || itemType == ItemType.RocketV)
            {
                ExplodeRocket(cell, itemType);
                return;
            }

            if (!item.CanBeMatchedByTouch()) return;

            MatchType matchType = item.GetMatchType();
            if (matchType == MatchType.None) return;

            List<Cell> group = FindConnectedMatchCells(cell, matchType);
            GameConfig config = ServiceProvider.GetGameConfig;
            int minMatchForBlast = config != null ? config.MinMatchForBlast : 2;
            if (group.Count < minMatchForBlast) return;

            if (IsBombPattern(group))
            {
                CreateBombFromGroup(cell, group, matchType);
            }
            else if (IsRocketPattern(group, out bool horizontal))
            {
                CreateRocketFromGroup(cell, group, matchType, horizontal);
            }
            else
            {
                config = ServiceProvider.GetGameConfig;
                int busy = config != null ? config.DefaultBusyDuration : 3;
                LockCells(group, busy);
                HandleBlastClick(group, matchType);
            }
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Board.boardData.columnCount && y < Board.boardData.rowCount;
        }

        private ClickOutcome ClassifyClick(int groupSize)
        {
            GameConfig config = ServiceProvider.GetGameConfig;
            int minBlast = config != null ? config.MinMatchForBlast : 2;
            int minBooster = config != null ? config.MinMatchForBooster : 5;

            if (groupSize < minBlast) return ClickOutcome.TooSmall;

            if (groupSize >= minBooster) return ClickOutcome.Booster;

            return ClickOutcome.Blast;
        }

        private void HandleBlastClick(List<Cell> group, MatchType matchType)
        {
            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                if (c.Item != null) c.Item.TryExecute(matchType);
            }
        }

        private void CreateBombFromGroup(Cell originCell, List<Cell> group, MatchType matchType)
        {
            GameConfig config = ServiceProvider.GetGameConfig;
            int busy = config != null ? config.BombBusyDuration : 4;
            LockCells(group, busy);

            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                if (c == originCell) continue;

                if (c.Item != null) c.Item.TryExecute(matchType);
            }

            Transform parent = Board.ItemsParent;
            Item bomb = ItemFactory.CreateItem(ItemType.Bomb, parent);
            if (originCell.Item != null) originCell.Item.TryExecute(matchType);

            originCell.Item = bomb;
            bomb.transform.position = originCell.transform.position;
        }

        private void CreateRocketFromGroup(Cell originCell, List<Cell> group, MatchType matchType, bool horizontal)
        {
            GameConfig config = ServiceProvider.GetGameConfig;
            int busy = config != null ? config.RocketBusyDuration : 3;
            LockCells(group, busy);

            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                if (c == originCell) continue;

                if (c.Item != null) c.Item.TryExecute(matchType);
            }

            Transform parent = Board.ItemsParent;
            ItemType rocketType = horizontal ? ItemType.RocketH : ItemType.RocketV;
            Item rocket = ItemFactory.CreateItem(rocketType, parent);
            if (originCell.Item != null) originCell.Item.TryExecute(matchType);

            originCell.Item = rocket;
            rocket.transform.position = originCell.transform.position;
        }

        private void LockCells(List<Cell> group, int duration)
        {
            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                c.BusyTicks = duration;
            }
        }

        private void TickBusyCells()
        {
            if (Board == null) return;

            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                Cell cell = Board.Cells[x, y];
                if (cell == null) continue;
                if (cell.BusyTicks > 0) cell.BusyTicks -= 1;
            }
        }

        private bool IsBombPattern(List<Cell> group)
        {
            if (group.Count < 5) return false;

            HashSet<(int x, int y)> set = new();
            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                set.Add((c.X, c.Y));
            }

            for (int i = 0; i < group.Count; i++)
            {
                Cell pivot = group[i];
                int cx = pivot.X;
                int cy = pivot.Y;

                int horiz = 1;
                int xLeft = cx - 1;
                while (set.Contains((xLeft, cy)))
                {
                    horiz++;
                    xLeft--;
                }

                int xRight = cx + 1;
                while (set.Contains((xRight, cy)))
                {
                    horiz++;
                    xRight++;
                }

                int vert = 1;
                int yDown = cy - 1;
                while (set.Contains((cx, yDown)))
                {
                    vert++;
                    yDown--;
                }

                int yUp = cy + 1;
                while (set.Contains((cx, yUp)))
                {
                    vert++;
                    yUp++;
                }

                if (horiz >= 3 && vert >= 3) return true;
            }

            return false;
        }

        private bool IsRocketPattern(List<Cell> group, out bool horizontal)
        {
            horizontal = false;
            if (group.Count != 4) return false;

            HashSet<int> xs = new();
            HashSet<int> ys = new();

            for (int i = 0; i < group.Count; i++)
            {
                Cell c = group[i];
                xs.Add(c.X);
                ys.Add(c.Y);
            }

            if (ys.Count == 1 && xs.Count == 4)
            {
                int minX = int.MaxValue;
                int maxX = int.MinValue;
                foreach (int x in xs)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }

                if (maxX - minX == 3)
                {
                    horizontal = true;
                    return true;
                }
            }

            if (xs.Count == 1 && ys.Count == 4)
            {
                int minY = int.MaxValue;
                int maxY = int.MinValue;
                foreach (int y in ys)
                {
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }

                if (maxY - minY == 3)
                {
                    horizontal = false;
                    return true;
                }
            }

            return false;
        }

        private void ExplodeBomb(Cell center)
        {
            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                int x = center.X + dx;
                int y = center.Y + dy;

                if (!IsInsideBoard(x, y)) continue;

                Cell cell = Board.Cells[x, y];
                if (cell == null || !cell.HasItem()) continue;

                Item item = cell.Item;
                if (item.CanBeExplodedByNeighbourMatch() || item.CanBeExplodedByTouch()) item.TryExecute(MatchType.Special);
            }
        }

        private void ExplodeRocket(Cell centerCell, ItemType rocketType)
        {
            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            bool horizontal = rocketType == ItemType.RocketH;

            if (horizontal)
            {
                int y = centerCell.Y;
                for (int x = 0; x < cols; x++)
                {
                    Cell cell = Board.Cells[x, y];
                    if (cell == null || !cell.HasItem()) continue;

                    Item item = cell.Item;
                    if (item.CanBeExplodedByNeighbourMatch() || item.CanBeExplodedByTouch()) item.TryExecute(MatchType.Special);
                }
            }
            else
            {
                int x = centerCell.X;
                for (int y = 0; y < rows; y++)
                {
                    Cell cell = Board.Cells[x, y];
                    if (cell == null || !cell.HasItem()) continue;

                    Item item = cell.Item;
                    if (item.CanBeExplodedByNeighbourMatch() || item.CanBeExplodedByTouch()) item.TryExecute(MatchType.Special);
                }
            }
        }

        private List<Cell> FindConnectedMatchCells(Cell startCell, MatchType matchType)
        {
            List<Cell> result = new();
            Stack<Cell> stack = new();
            HashSet<Cell> visited = new();

            stack.Push(startCell);

            while (stack.Count > 0)
            {
                Cell current = stack.Pop();

                if (visited.Contains(current)) continue;

                visited.Add(current);

                if (current.Item == null) continue;

                if (current.Item.GetMatchType() != matchType) continue;

                result.Add(current);

                foreach (Cell n in current.Neighbours)
                    if (!visited.Contains(n))
                        stack.Push(n);
            }

            return result;
        }

        private void ScanBoardForGroups()
        {
            if (Board == null) return;

            int rows = Board.boardData.rowCount;
            int cols = Board.boardData.columnCount;

            int maxGroup = 0;
            HashSet<Cell> visited = new();

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                Cell cell = Board.Cells[x, y];
                if (cell == null || !cell.HasItem() || visited.Contains(cell)) continue;

                MatchType mt = cell.Item.GetMatchType();
                if (mt == MatchType.None) continue;

                List<Cell> group = FindConnectedMatchCells(cell, mt);
                for (int i = 0; i < group.Count; i++) visited.Add(group[i]);

                if (group.Count > maxGroup) maxGroup = group.Count;
            }
        }

        public IEnumerable<PlayerAction> GetExecutedActions()
        {
            return _executed;
        }
    }
}