using Domain.Common.Base.MovingEntities;
using System.Windows;
using System.Windows.Media.Animation;

namespace CitySimulatorWPF.ViewModels
{
    public class MovingEntityVM : Animatable
    {
        public MovingEntity Entity { get; }

        public static readonly DependencyProperty PixelXProperty =
            DependencyProperty.Register(
                nameof(PixelX),
                typeof(double),
                typeof(MovingEntityVM),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty PixelYProperty =
            DependencyProperty.Register(
                nameof(PixelY),
                typeof(double),
                typeof(MovingEntityVM),
                new PropertyMetadata(0.0));

        public double PixelX
        {
            get => (double)GetValue(PixelXProperty);
            set => SetValue(PixelXProperty, value);
        }

        public double PixelY
        {
            get => (double)GetValue(PixelYProperty);
            set => SetValue(PixelYProperty, value);
        }

        public MovingEntityVM(MovingEntity entity)
        {
            Entity = entity;
        }

        private MovingEntityVM()
        {
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MovingEntityVM();
        }
    }
}