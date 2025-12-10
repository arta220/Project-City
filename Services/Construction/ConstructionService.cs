using Domain.Buildings.Construction;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Construction
{
    /// <summary>
    /// Сервис управления процессом строительства зданий
    /// </summary>
    public class ConstructionService : IConstructionService
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly List<ConstructionSite> _activeConstructionSites = new();

        /// <summary>
        /// Событие завершения строительства на площадке
        /// </summary>
        public event Action<ConstructionSite> ConstructionCompleted;

        /// <summary>
        /// Инициализирует сервис управления строительством
        /// </summary>
        /// <param name="buildingRegistry">Реестр зданий для доступа к размещенным объектам</param>
        public ConstructionService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        /// <summary>
        /// Обновляет состояние всех активных строительных площадок
        /// </summary>
        /// <param name="time">Текущее время симуляции</param>
        public void Update(SimulationTime time)
        {
            var sitesToRemove = new List<ConstructionSite>();

            foreach (var site in _activeConstructionSites)
            {
                if (site.IsCancelled)
                {
                    sitesToRemove.Add(site);
                    continue;
                }

                ProcessConstruction(site, time);
            }

            // Удаляем завершенные или отмененные площадки
            foreach (var site in sitesToRemove)
            {
                _activeConstructionSites.Remove(site);
            }
        }

        /// <summary>
        /// Начинает процесс строительства на указанной площадке
        /// </summary>
        /// <param name="constructionSite">Строительная площадка для запуска строительства</param>
        /// <returns>True, если строительство успешно запущено, иначе false</returns>
        public bool StartConstruction(ConstructionSite constructionSite)
        {
            if (constructionSite == null || constructionSite.Project == null)
                return false;

            if (_activeConstructionSites.Contains(constructionSite))
                return false;

            constructionSite.Status = ConstructionSiteStatus.Preparing;
            _activeConstructionSites.Add(constructionSite);
            return true;
        }

        /// <summary>
        /// Отменяет строительство на указанной площадке
        /// </summary>
        /// <param name="constructionSite">Строительная площадка для отмены строительства</param>
        /// <returns>True, если строительство успешно отменено, иначе false</returns>
        public bool CancelConstruction(ConstructionSite constructionSite)
        {
            if (constructionSite == null || !_activeConstructionSites.Contains(constructionSite))
                return false;

            constructionSite.IsCancelled = true;
            constructionSite.Status = ConstructionSiteStatus.Cancelled;
            return true;
        }

        /// <summary>
        /// Получает список всех активных строительных площадок
        /// </summary>
        /// <returns>Коллекция активных строительных площадок</returns>
        public IEnumerable<ConstructionSite> GetActiveConstructionSites()
        {
            return _activeConstructionSites.AsReadOnly();
        }

        /// <summary>
        /// Обрабатывает процесс строительства на площадке
        /// </summary>
        /// <param name="site">Строительная площадка для обработки</param>
        /// <param name="time">Текущее время симуляции</param>
        private void ProcessConstruction(ConstructionSite site, SimulationTime time)
        {
            // Если отменено, пропускаем
            if (site.IsCancelled)
                return;

            // Проверяем наличие материалов
            if (!site.HasEnoughMaterials())
            {
                site.Status = ConstructionSiteStatus.Preparing;
                return;
            }

            // Проверяем наличие рабочих
            if (!site.HasEnoughWorkers())
            {
                site.Status = ConstructionSiteStatus.Preparing;
                return;
            }

            // Начинаем строительство
            if (site.Status == ConstructionSiteStatus.Preparing)
            {
                site.Status = ConstructionSiteStatus.Building;
            }

            if (site.Status == ConstructionSiteStatus.Building)
            {
                // Увеличиваем прогресс строительства
                site.Project.CurrentTicks += site.Project.ConstructionSpeed;
                
                // Вычисляем прогресс в процентах
                site.Project.Progress = (int)((double)site.Project.CurrentTicks / site.Project.TotalTicksRequired * 100);
                
                // Ограничиваем прогресс 100%
                if (site.Project.Progress > 100)
                    site.Project.Progress = 100;

                // Если строительство завершено
                if (site.Project.Progress >= 100)
                {
                    site.Status = ConstructionSiteStatus.Completed;
                    // Потребляем материалы
                    site.ConsumeMaterials(site.Project.RequiredMaterials);
                    // Уведомляем о завершении строительства
                    ConstructionCompleted?.Invoke(site);
                }
            }
        }
    }
}

