using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Citizens;
using Domain.Citizens.States;
using System.ComponentModel;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для представления жителя на карте города.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Инкапсулирует состояние жителя (<see cref="Citizen"/>).
    /// - Отслеживает изменения позиции жителя и обновляет координаты для визуализации.
    /// - Преобразует координаты в пиксели для UI (<see cref="_pixelX"/> и <see cref="_pixelY"/>).
    ///
    /// Контекст использования:
    /// - Привязан к визуальным элементам карты для отображения жителей.
    /// - Используется в коллекции <see cref="MapVM.Citizens"/> для биндинга ObservableCollection.
    ///
    /// Расширяемость:
    /// - Можно добавить визуальные свойства (цвет, иконка, анимация состояния).
    /// - Можно расширить логику отслеживания других состояний жителя (например, настроение, здоровье).
    /// </remarks>
    public partial class CitizenVM : ObservableObject
    {
        public Citizen Citizen { get; }

        [ObservableProperty]
        private int _x;
        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private string _citizenColor;

        [ObservableProperty]
        public int _pixelX;

        [ObservableProperty]
        public int _pixelY;

        public CitizenVM(Citizen citizen)
        {
            Citizen = citizen;
            UpdatePosition();
            UpdateColor();

            Citizen.PropertyChanged += OnCitizenPropertyChanged;
        }

        private void OnCitizenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Citizen.Position))
            {
                UpdatePosition();
            }

            if (e.PropertyName == nameof(Citizen.CurrentTransport) ||
                e.PropertyName == nameof(Citizen.State))
            {
                UpdateColor();
            }
        }

        public void UpdatePosition()
        {
            X = Citizen.Position.X;
            Y = Citizen.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;
        }

        private void UpdateColor()
        {
            // Простейшая логика подсветки жителя:
            // - едет в транспорте: красный
            // - идёт на работу или домой пешком: оранжевый
            // - в остальных случаях: зелёный
            if (Citizen.IsInTransport)
            {
                CitizenColor = "Red";
            }
            else if (Citizen.State == CitizenState.GoingWork ||
                     Citizen.State == CitizenState.GoingHome)
            {
                CitizenColor = "Orange";
            }
            else
            {
                CitizenColor = "Green";
            }
        }
    }
}
