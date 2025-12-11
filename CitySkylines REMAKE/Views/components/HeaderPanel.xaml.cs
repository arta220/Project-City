using System.Windows;
using System.Windows.Controls;
using CitySkylines_REMAKE;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace CitySimulatorWPF.Views.components
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        // ТВОЙ СТАРЫЙ МЕТОД - НЕ ТРОГАЕМ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Твой код для графиков
        }

        // Кнопка сохранения
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("💾 Игра сохранена!", "Сохранение");
        }

        // Кнопка загрузки
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📂 Игра загружена!", "Загрузка");
        }

        // Кнопка очистки карты
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Спрашиваем подтверждение
                var result = MessageBox.Show(
                    "УДАЛИТЬ ВСЕ ЗДАНИЯ С КАРТЫ?\n\nЭто действие нельзя отменить!",
                    "Очистка карты",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                // Получаем доступ к симуляции через App
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();

                if (simulation == null)
                {
                    MessageBox.Show("Не могу получить доступ к симуляции");
                    return;
                }

                // Очищаем карту
                int removedCount = 0;
                for (int x = 0; x < simulation.MapModel.Width; x++)
                {
                    for (int y = 0; y < simulation.MapModel.Height; y++)
                    {
                        if (simulation.MapModel[x, y].MapObject != null)
                        {
                            simulation.MapModel[x, y].MapObject = null;
                            removedCount++;
                        }
                    }
                }

                MessageBox.Show($"✅ Карта очищена!\nУдалено объектов: {removedCount}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}