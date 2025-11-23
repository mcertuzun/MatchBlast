namespace Core
{
    public partial class Game
    {
        private void TickGravity()
        {
            if (ActionQueue != null)
            {
                var gravityAction = new PlayerAction
                {
                    Time = _time,
                    Type = ActionType.Gravity,
                    CellX = -1,
                    CellY = -1
                };
                ActionQueue.Enqueue(gravityAction);
                ActionQueue.ProcessActions(_time);
            }
            else if (FallAndFillManager != null)
            {
                FallAndFillManager.TickLogic();
            }
        }
    }
}
