using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;

namespace CitySimulatorWPF.ViewModels
{
	public partial class MovingEntityVM : ObservableObject
	{
		public MovingEntity Entity { get; }

		[ObservableProperty]
		private int _pixelX;

		[ObservableProperty]
		private int _pixelY;

		public MovingEntityVM(MovingEntity entity)
		{
			Entity = entity;

			UpdatePosition();
		}

		public void UpdatePosition()
		{
			PixelX = Entity.Position.Y * 20;
			PixelY = Entity.Position.X * 20;
		}
	}
}
