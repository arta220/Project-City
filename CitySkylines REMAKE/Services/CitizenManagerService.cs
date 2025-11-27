using CitySimulatorWPF.ViewModels;
using Domain.Citizens;
using Services.CitizensSimulation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CitySimulatorWPF.Services
{
    /// <summary>
    /// Интерфейс сервиса управления визуальными представлениями жителей.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Поддерживает ObservableCollection<CitizenVM> для UI.
    /// - Слушает события CitizenSimulationService (добавление, удаление, обновление).
    ///
    /// Контекст использования:
    /// - Используется MapVM или любым другим UI-слоем для отображения жителей на карте.
    /// - Позволяет отделить логику симуляции (CitizenSimulationService) от визуализации.
    ///
    /// Расширяемость:
    /// - Можно добавить фильтрацию по состоянию жителя (идет на работу, дома и т.д.).
    /// - Можно интегрировать с системой событий или уведомлений при изменении состояния жителей.
    /// </remarks>
    public interface ICitizenManagerService
    {
        /// <summary>
        /// Коллекция визуальных представлений жителей.
        /// </summary>
        ObservableCollection<CitizenVM> Citizens { get; }

        /// <summary>
        /// Подключает сервис визуализации к симуляции жителей.
        /// </summary>
        /// <param name="simulation">Сервис симуляции жителей.</param>
        void StartSimulation(CitizenSimulationService simulation);

        /// <summary>
        /// Отключает сервис визуализации от симуляции и очищает коллекцию.
        /// </summary>
        void StopSimulation();
    }

    /// <summary>
    /// Реализация сервиса управления визуализацией жителей на карте.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Создает и обновляет CitizenVM для каждого Citizen из CitizenSimulationService.
    /// - Подписывается на события CitizenAdded, CitizenRemoved и CitizenUpdated.
    ///
    /// Контекст использования:
    /// - Используется MapVM для связывания симуляции жителей с визуальной частью.
    ///
    /// Расширяемость:
    /// - Можно добавить сортировку или группировку жителей по домам, работам или состояниям.
    /// - Можно интегрировать с системой навигации или визуальных эффектов (например, анимация движения).
    /// </remarks>
    public class CitizenManagerService : ICitizenManagerService
    {
        private CitizenSimulationService _simulation;

        public ObservableCollection<CitizenVM> Citizens { get; } = new ObservableCollection<CitizenVM>();

        public void StartSimulation(CitizenSimulationService simulation)
        {
            if (simulation == null) throw new ArgumentNullException(nameof(simulation));

            _simulation = simulation;

            foreach (var citizen in _simulation.Citizens)
            {
                Citizens.Add(new CitizenVM(citizen));
            }

            _simulation.CitizenAdded += OnCitizenAdded;
            _simulation.CitizenRemoved += OnCitizenRemoved;
            _simulation.CitizenUpdated += OnCitizenUpdated;
        }

        public void StopSimulation()
        {
            if (_simulation == null) return;

            _simulation.CitizenAdded -= OnCitizenAdded;
            _simulation.CitizenRemoved -= OnCitizenRemoved;
            _simulation.CitizenUpdated -= OnCitizenUpdated;

            Citizens.Clear();
            _simulation = null;
        }

        private void OnCitizenAdded(Citizen citizen)
        {
            Citizens.Add(new CitizenVM(citizen));
        }

        private void OnCitizenRemoved(Citizen citizen)
        {
            var vm = Citizens.FirstOrDefault(c => c.Citizen == citizen);
            if (vm != null)
                Citizens.Remove(vm);
        }

        private void OnCitizenUpdated(Citizen citizen)
        {
            var vm = Citizens.FirstOrDefault(c => c.Citizen == citizen);
            vm?.UpdatePosition();
        }
    }
}
