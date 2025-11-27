using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для визуального представления игрового объекта (<see cref="MapObject"/>).
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Инкапсулирует модель <see cref="MapObject"/> для привязки к UI.
    /// - Хранит дополнительные данные для отображения, такие как название и путь к иконке.
    /// - Не содержит логики симуляции или изменения состояния модели.
    ///
    /// Контекст использования:
    /// - Используется в панели выбора зданий или объектов на карте.
    /// - Передаётся в <see cref="MapVM"/> и другие компоненты UI для отображения иконок и названий.
    ///
    /// </remarks>
    public partial class ObjectVM : ObservableObject
    {
        /// <summary>
        /// Модель игрового объекта.
        /// </summary>
        public MapObject Model { get; }

        /// <summary>
        /// Название объекта для UI.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Путь к иконке объекта для отображения в интерфейсе.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Создаёт ViewModel для объекта.
        /// </summary>
        /// <param name="model">Модель игрового объекта</param>
        /// <param name="name">Название объекта для UI</param>
        /// <param name="iconPath">Путь к иконке для UI</param>
        public ObjectVM(MapObject model, string name, string iconPath)
        {
            Model = model;
            Name = name;
            IconPath = iconPath;
        }
    }
}
