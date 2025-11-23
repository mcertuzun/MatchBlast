using System.Collections.Generic;

namespace Core
{
    public partial class Game
    {
        public bool IsRecording { get; private set; }
        public bool IsReplaying { get; private set; }

        private readonly List<PlayerAction> _recorded = new List<PlayerAction>();
        private int _replayIndex;
        private int _tickCounter;

        public void BeginRecording()
        {
            _recorded.Clear();
            _replayIndex = 0;
            _tickCounter = 0;
            IsRecording = true;
            IsReplaying = false;
        }

        public void RecordAction(PlayerAction action)
        {
            if (!IsRecording)
            {
                return;
            }

            action.Time = 0f;
            _recorded.Add(action);

            if (ActionQueue != null)
            {
                ActionQueue.Enqueue(action);
            }
        }

        public void StartReplay()
        {
            if (_recorded.Count == 0 || ActionQueue == null || Level == null)
            {
                return;
            }

            IsRecording = false;
            IsReplaying = true;
            _replayIndex = 0;
            _tickCounter = 0;

            if (ActionQueue != null)
            {
                ActionQueue.Clear();
            }

            Level.RestartLevel();
        }

        private void TickReplay()
        {
            if (!IsReplaying || ActionQueue == null)
            {
                return;
            }

            _tickCounter++;
            var config = ServiceProvider.GetGameConfig;
            int ticksBetween = config != null ? config.GravityTicksBetweenActions : 3;
            if (ticksBetween <= 0) ticksBetween = 1;
            if (_tickCounter % ticksBetween != 0)
            {
                return;
            }

            if (_replayIndex >= _recorded.Count)
            {
                IsReplaying = false;
                return;
            }

            var action = _recorded[_replayIndex++];
            action.Time = 0f;
            ActionQueue.Enqueue(action);
        }
    }
}
