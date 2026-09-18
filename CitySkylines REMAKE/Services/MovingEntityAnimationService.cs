using CitySimulatorWPF.ViewModels;
using Domain.Buildings;
using Domain.Common.Base;
using Domain.Map;
using Domain.Transports;
using Services;
using Services.PathFind;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CitySimulatorWPF.Services
{
	public class MovingEntityAnimationService
	{
		private readonly Simulation _simulation;
		private readonly IPathFinder _pathFinder;

		public ObservableCollection<MovingEntityVM> Entities { get; } = new();

		public MovingEntityAnimationService(
			IPathFinder pathFinder,
			Simulation simulation)
		{
			_pathFinder = pathFinder;
			_simulation = simulation;
		}

		public void SendPlane(Position target)
		{
			var plane = new Plane(5.0f)
			{
				Position = GetStartPosition(),
				TargetPosition = target
			};

			StartMovement(plane, target);
		}

		public void SendPlaneToAirport()
		{
			Airport airport = null;

			for (int x = 0; x < _simulation.MapModel.Width; x++)
			{
				for (int y = 0; y < _simulation.MapModel.Height; y++)
				{
					var mapObject = _simulation.MapModel[x, y].MapObject;

					if (mapObject is Airport foundAirport)
					{
						airport = foundAirport;
						break;
					}
				}

				if (airport != null)
					break;
			}

			if (airport == null)
				return;

			var result = _simulation.GetMapObjectPlacement(airport);

			if (!result.found || result.placement == null)
				return;

			var placement = result.placement.Value;

			var targets = new[]
			{
				new Position(placement.Left - 1, placement.Top),
				new Position(placement.Right, placement.Top),
				new Position(placement.Left, placement.Top - 1),
				new Position(placement.Left, placement.Bottom)
			};

			foreach (var target in targets)
			{
				if (!_simulation.MapModel.IsPositionInBounds(target))
					continue;

				var path = _pathFinder.FindPath(
					GetStartPosition(),
					target);

				if (path.Count > 0)
				{
					SendPlane(target);
					return;
				}
			}
		}

		public void SendShip(Position target)
		{
			var ship = new Ship(3.0f)
			{
				Position = GetStartPosition(),
				TargetPosition = target
			};

			StartMovement(ship, target);
		}

		private Position GetStartPosition()
		{
			return new Position(
				_simulation.MapModel.Width - 1,
				_simulation.MapModel.Height - 1);
		}

		private void StartMovement(
			MovingEntity entity,
			Position target)
		{
			var path = _pathFinder.FindPath(
				entity.Position,
				target);

			if (path == null || path.Count == 0)
				return;

			entity.CurrentPath.Clear();

			foreach (var position in path)
			{
				entity.CurrentPath.Enqueue(position);
			}

			var vm = new MovingEntityVM(entity);

			Entities.Add(vm);

			var timer = new DispatcherTimer();

			timer.Interval = TimeSpan.FromMilliseconds(
				1000.0 / Math.Max(entity.Speed, 0.1f));

			timer.Tick += (sender, args) =>
			{
				if (entity.CurrentPath.Count == 0)
				{
					timer.Stop();
					Entities.Remove(vm);
					return;
				}

				entity.Position = entity.CurrentPath.Dequeue();

				vm.UpdatePosition();
			};

			timer.Start();
		}
	}
}