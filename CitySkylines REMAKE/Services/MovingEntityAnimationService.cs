using CitySimulatorWPF.ViewModels;
using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Domain.Transports.Air;
using Domain.Transports.Sea;
using Services.EntityMovement.PathFind;
using Services.EntityMovement.Profile;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace CitySimulatorWPF.Services
{
    public class MovingEntityAnimationService : IMovingEntityAnimationService
    {
        private const int TileSize = 20;
        private const int EntitySize = 6;

        private readonly IPathFinder _pathFinder;
        private readonly MapModel _mapModel;

        public ObservableCollection<MovingEntityVM> Entities { get; } = new();

        public MovingEntityAnimationService(
            IPathFinder pathFinder,
            MapModel mapModel)
        {
            _pathFinder = pathFinder;
            _mapModel = mapModel;
        }

        public void SendPlane(Position target)
        {
            var profile = new PlaneNavigationProfile(_mapModel);

            var plane = new Plane(
                speed: 5.0f,
                navigationProfile: profile)
            {
                Position = new Position(0, 0),
                TargetPosition = target
            };

            StartMovement(plane, target);
        }

        public void SendShip(Position target)
        {
            var profile = new ShipNavigationProfile(_mapModel);

            var ship = new Ship(
                speed: 2.5f,
                navigationProfile: profile)
            {
                Position = new Position(0, 0),
                TargetPosition = target
            };

            StartMovement(ship, target);
        }

        private void StartMovement(
            MovingEntity entity,
            Position target)
        {
            var path = _pathFinder
                .FindPath(
                    entity.Position,
                    target,
                    entity.NavigationProfile)
                ?.ToList();

            if (path == null || path.Count == 0)
                return;

            entity.CurrentPath.Clear();

            foreach (var position in path)
                entity.CurrentPath.Enqueue(position);

            var vm = new MovingEntityVM(entity);

            SetInitialPixelPosition(vm, entity.Position);

            Entities.Add(vm);

            AnimateNext(vm);
        }

        private void AnimateNext(MovingEntityVM vm)
        {
            var entity = vm.Entity;

            if (entity.CurrentPath.Count == 0)
            {
                Entities.Remove(vm);
                return;
            }

            var nextPosition = entity.CurrentPath.Dequeue();

            var targetPixelX = PositionToPixelX(nextPosition);
            var targetPixelY = PositionToPixelY(nextPosition);

            var speed = Math.Max(entity.Speed, 0.1f);

            var duration = new Duration(
                TimeSpan.FromSeconds(1.0 / speed));

            var xAnimation = new DoubleAnimation(
                fromValue: vm.PixelX,
                toValue: targetPixelX,
                duration);

            var yAnimation = new DoubleAnimation(
                fromValue: vm.PixelY,
                toValue: targetPixelY,
                duration);

            xAnimation.FillBehavior = FillBehavior.HoldEnd;
            yAnimation.FillBehavior = FillBehavior.HoldEnd;

            xAnimation.Completed += (sender, args) =>
            {
                vm.PixelX = targetPixelX;
                vm.PixelY = targetPixelY;

                vm.BeginAnimation(
                    MovingEntityVM.PixelXProperty,
                    null);

                vm.BeginAnimation(
                    MovingEntityVM.PixelYProperty,
                    null);

                entity.Position = nextPosition;

                AnimateNext(vm);
            };

            vm.BeginAnimation(
                MovingEntityVM.PixelXProperty,
                xAnimation);

            vm.BeginAnimation(
                MovingEntityVM.PixelYProperty,
                yAnimation);
        }

        private static void SetInitialPixelPosition(
            MovingEntityVM vm,
            Position position)
        {
            vm.PixelX = PositionToPixelX(position);
            vm.PixelY = PositionToPixelY(position);
        }

        private static double PositionToPixelX(Position position)
        {
            return position.Y * TileSize
                   + (TileSize - EntitySize) / 2.0;
        }

        private static double PositionToPixelY(Position position)
        {
            return position.X * TileSize
                   + (TileSize - EntitySize) / 2.0;
        }
    }
}