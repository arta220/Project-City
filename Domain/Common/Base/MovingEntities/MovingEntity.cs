using Domain.Map;

namespace Domain.Common.Base.MovingEntities
{
    public abstract class MovingEntity : MapObject
    {
        protected MovingEntity(Area area, float speed) : base(area)
        {
            Speed = speed;
        }
        public INavigationProfile NavigationProfile { get; init; } // Машина не будет ведь в моменте менять навигационный профиль на человеческий?
        public Position Position { get; set; }
        public Position TargetPosition { get; set; }
        public Queue<Position> CurrentPath { get; set; } = new();
        public float Speed { get; set; }
    }

}
