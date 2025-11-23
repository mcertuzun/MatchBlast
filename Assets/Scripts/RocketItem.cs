
namespace Core
{
    public class RocketItem : Item
    {
        public bool IsHorizontal;

        public void PrepareRocket(bool isHorizontal)
        {
            IsHorizontal = isHorizontal;
        }

        public override MatchType GetMatchType()
        {
            return MatchType.Special;
        }

        public override ItemType GetItemType()
        {
            return IsHorizontal ? ItemType.RocketH : ItemType.RocketV;
        }

        public override bool CanBeMatchedByTouch()
        {
            return true;
        }

        public override bool CanBeExplodedByTouch()
        {
            return true;
        }

        public override bool CanBeExplodedByNeighbourMatch()
        {
            return true;
        }
    }
}