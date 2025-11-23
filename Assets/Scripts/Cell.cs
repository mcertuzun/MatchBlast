using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Cell : MonoBehaviour
    {
        [HideInInspector] public int X;
        [HideInInspector] public int Y;

        private Board _board;
        public List<Cell> Neighbours { get; private set; }
        [HideInInspector] public Cell FirstCellBelow;
        [HideInInspector] public bool IsFillingCell;

        public TextMesh LabelText;

        private Item _item;
        public Item Item
        {
            get { return _item; }
            set
            {
                if (_item == value) return;

                var oldItem = _item;
                _item = value;

                if (oldItem != null && Equals(oldItem.Cell, this))
                {
                    oldItem.Cell = null;
                }
                if (value != null)
                {
                    value.Cell = this;
                }
            }
        }

        public int BusyTicks;

        public bool IsBusy
        {
            get { return BusyTicks > 0; }
        }

        public void Prepare(int x, int y, Board board)
        {
            X = x;
            Y = y;
            transform.localPosition = new Vector3(x, y);
            _board = board;
            UpdateNeighbours(_board);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var cellName = X + ":" + Y;
            LabelText.text = cellName;
            gameObject.name = "Cell " + cellName;
        }

        private void UpdateNeighbours(Board board)
        {
            Neighbours = new List<Cell>();
            var up = board.GetNeighbourWithDirection(this, Direction.Up);
            var down = board.GetNeighbourWithDirection(this, Direction.Down);
            var left = board.GetNeighbourWithDirection(this, Direction.Left);
            var right = board.GetNeighbourWithDirection(this, Direction.Right);

            if (up != null) Neighbours.Add(up);
            if (down != null) Neighbours.Add(down);
            if (left != null) Neighbours.Add(left);
            if (right != null) Neighbours.Add(right);

            if (down != null) FirstCellBelow = down;
        }

        public Cell GetFallTarget()
        {
            var targetCell = this;
            while (targetCell.FirstCellBelow != null &&
                   targetCell.FirstCellBelow.Item == null &&
                   !targetCell.FirstCellBelow.IsBusy)
            {
                targetCell = targetCell.FirstCellBelow;
            }
            return targetCell;
        }

        public bool HasItem()
        {
            return Item != null;
        }
    }
}