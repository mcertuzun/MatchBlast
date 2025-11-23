using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ItemFallSystem : MonoBehaviour, IProvidable
    {
        private class ActiveFall
        {
            public Item Item;
            public Vector3 StartPosition;
            public Vector3 TargetPosition;
            public float Duration;
            public float Elapsed;
        }

        private readonly List<ActiveFall> _activeFalls = new();

        [Header("Fall Tuning")] [SerializeField]
        private float _cellsPerSecond = 8f;

        [SerializeField] private float _minDuration = 0.08f;
        [SerializeField] private float _maxDuration = 0.35f;

        private void Awake()
        {
            ServiceProvider.Register(this);
            GameConfig config = ServiceProvider.GetGameConfig;
            if (config != null)
            {
                _cellsPerSecond = config.FallCellsPerSecond;
                _minDuration = config.FallMinDuration;
                _maxDuration = config.FallMaxDuration;
            }
        }

        public void StartFall(Item item, Cell targetCell)
        {
            if (item == null || targetCell == null) return;

            Vector3 startPos = item.transform.position;
            Vector3 targetPos = targetCell.transform.position;

            if (Mathf.Approximately(startPos.y, targetPos.y))
            {
                item.Cell = targetCell;
                item.transform.position = targetPos;
                item.afterFalled();
                return;
            }

            for (int i = 0; i < _activeFalls.Count; i++)
            {
                ActiveFall f = _activeFalls[i];
                if (f.Item == item)
                {
                    f.StartPosition = item.transform.position;
                    f.TargetPosition = targetPos;
                    f.Elapsed = 0f;
                    f.Duration = ComputeDuration(f.StartPosition, f.TargetPosition);
                    item.Cell = targetCell;
                    item.IsFalling = true;
                    return;
                }
            }

            item.Cell = targetCell;
            item.IsFalling = true;

            ActiveFall fall = new()
            {
                Item = item,
                StartPosition = startPos,
                TargetPosition = targetPos,
                Elapsed = 0f,
                Duration = ComputeDuration(startPos, targetPos)
            };
            _activeFalls.Add(fall);
        }

        private float ComputeDuration(Vector3 start, Vector3 target)
        {
            float distance = Mathf.Abs(start.y - target.y);
            if (distance < Mathf.Epsilon) return _minDuration;

            float duration = distance / Mathf.Max(_cellsPerSecond, 0.01f);
            return Mathf.Clamp(duration, _minDuration, _maxDuration);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            for (int i = _activeFalls.Count - 1; i >= 0; i--)
            {
                ActiveFall fall = _activeFalls[i];

                if (fall.Item == null)
                {
                    _activeFalls.RemoveAt(i);
                    continue;
                }

                fall.Elapsed += dt;
                float t = fall.Duration > 0f ? Mathf.Clamp01(fall.Elapsed / fall.Duration) : 1f;

                float eased = t * t;

                Vector3 p = Vector3.Lerp(fall.StartPosition, fall.TargetPosition, eased);
                fall.Item.transform.position = p;

                if (t >= 1f)
                {
                    fall.Item.transform.position = fall.TargetPosition;
                    fall.Item.IsFalling = false;
                    fall.Item.afterFalled();
                    _activeFalls.RemoveAt(i);
                }
            }
        }
    }
}