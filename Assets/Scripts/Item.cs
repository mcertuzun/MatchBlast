using UnityEngine;

namespace Core
{
    public abstract class Item : MonoBehaviour
    {
        public ParticleSystem _particleSystem;

        public bool IsDestroyed { get; private set; }
        public bool IsFalling { get; internal set; }

        private Cell _cell;
        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value) return;

                var oldCell = _cell;
                _cell = value;

                if (oldCell != null && oldCell.Item == this)
                {
                    oldCell.Item = null;
                }

                if (value != null)
                {
                    value.Item = this;
                    gameObject.name = _cell.gameObject.name + " " + GetType().Name;
                }
            }
        }

        public virtual MatchType GetMatchType()
        {
            return MatchType.None;
        }

        public virtual ItemType GetItemType()
        {
            return ItemType.None;
        }

        public virtual bool CanBeExplodedByTouch()
        {
            return false;
        }

        public virtual bool CanBeExploring()
        {
            return false;
        }

        public virtual bool CanBeMatchedByTouch()
        {
            return false;
        }

        public virtual bool CanFall()
        {
            return true;
        }

        public virtual void afterFalled()
        {
        }

        public virtual bool CanBeExplodedByNeighbourMatch()
        {
            return false;
        }

        public virtual bool TryExecute(MatchType matchType = MatchType.None)
        {
            RemoveItem();
            return true;
        }

        public void RemoveItem()
        {
            if (Cell != null && Cell.Item == this)
            {
                Cell.Item = null;
            }

            Cell = null;
            IsDestroyed = true;
            Destroy(gameObject);
        }

        public void Fall()
        {
            if (!CanFall() || Cell == null)
            {
                return;
            }

            var targetCell = Cell.GetFallTarget();
            if (targetCell == null || targetCell == Cell)
            {
                return;
            }

            var fallSystem = ServiceProvider.GetItemFallSystem;
            if (fallSystem != null)
            {
                fallSystem.StartFall(this, targetCell);
            }
            else
            {
                Cell = targetCell;
                var p = transform.position;
                p.x = targetCell.transform.position.x;
                p.y = targetCell.transform.position.y;
                transform.position = p;
                afterFalled();
            }
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public virtual void SetHinted(int hinted)
        {
        }

        public virtual void SetParticle(ParticleSystem particle)
        {
            _particleSystem = particle;
        }

        public virtual ParticleSystem GetParticle()
        {
            return _particleSystem;
        }
    }
}