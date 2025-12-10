using Domain.Buildings;
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
    /// Сервис автоматической доставки строительных материалов на строительные площадки
    /// </summary>
    public class ConstructionMaterialLogisticsService : IConstructionMaterialLogisticsService
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly Dictionary<ConstructionSite, Dictionary<Enum, int>> _pendingDeliveries = new();
        private const int DeliveryBatchSize = 50; // Количество материалов за одну доставку (специальное ограничение для задержки анимации строительства)

        public ConstructionMaterialLogisticsService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        /// <summary>
        /// Обновляет состояние сервиса, обрабатывая доставки материалов
        /// </summary>
        /// <param name="time">Текущее время проекта в тиках</param>
        public void Update(SimulationTime time)
        {
            var sitesToProcess = _pendingDeliveries.Keys.ToList();

            foreach (var site in sitesToProcess)
            {
                if (site.IsCancelled || site.Status == ConstructionSiteStatus.Completed)
                {
                    _pendingDeliveries.Remove(site);
                    continue;
                }

                ProcessMaterialDelivery(site);
            }
        }

        /// <summary>
        /// Запрашивает доставку материалов на строительную площадку
        /// </summary>
        /// <param name="constructionSite">строительная площадка</param>
        public void RequestMaterialsDelivery(ConstructionSite constructionSite)
        {
            if (constructionSite == null || constructionSite.Project == null)
                return;

            if (!_pendingDeliveries.ContainsKey(constructionSite))
            {
                _pendingDeliveries[constructionSite] = new Dictionary<Enum, int>();
            }

            // Определяем, какие материалы нужны
            foreach (var requirement in constructionSite.Project.RequiredMaterials)
            {
                var needed = requirement.Value;
                var current = constructionSite.MaterialsBank.TryGetValue(requirement.Key, out int amount) 
                    ? amount 
                    : 0;
                var missing = needed - current;

                if (missing > 0)
                {
                    _pendingDeliveries[constructionSite][requirement.Key] = missing;
                }
            }
        }

        /// <summary>
        /// Поиск ближайшего источника материала среди промышленных зданий
        /// </summary>
        /// <param name="materialType">тип материала</param>
        /// <param name="targetPosition">позиция строительной площадки</param>
        /// <returns>Близжайший завод, производящий требуемый материал</returns>
        public IndustrialBuilding FindMaterialSource(Enum materialType, Position targetPosition)
        {
            var industrialBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>().ToList();

            IndustrialBuilding bestSource = null;
            double minDistance = double.MaxValue;

            foreach (var building in industrialBuildings)
            {
                // Проверяем наличие материала в ProductsBank или MaterialsBank
                bool hasMaterial = (building.ProductsBank.TryGetValue(materialType, out int productAmount) && productAmount > 0) ||
                                   (building.MaterialsBank.TryGetValue(materialType, out int materialAmount) && materialAmount > 0);

                if (!hasMaterial)
                    continue;

                // Получаем позицию здания
                var (placement, found) = _buildingRegistry.TryGetPlacement(building);
                if (!found)
                    continue;

                // Вычисляем расстояние (упрощенное - до центра здания)
                var buildingCenter = new Position(
                    placement.Value.Position.X + building.Area.Width / 2,
                    placement.Value.Position.Y + building.Area.Height / 2
                );

                var distance = CalculateDistance(buildingCenter, targetPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestSource = building;
                }
            }

            return bestSource;
        }

        /// <summary>
        /// Обрабатывает доставку материалов на площадку
        /// </summary>
        /// <param name="site">Строительная площадка</param>
        private void ProcessMaterialDelivery(ConstructionSite site)
        {
            if (!_pendingDeliveries.TryGetValue(site, out var neededMaterials))
                return;

            var (sitePlacement, found) = _buildingRegistry.TryGetPlacement(site);
            if (!found)
                return;

            var sitePosition = sitePlacement.Value.Position;

            var materialsToRemove = new List<Enum>();

            foreach (var materialNeeded in neededMaterials)
            {
                if (materialNeeded.Value <= 0)
                {
                    materialsToRemove.Add(materialNeeded.Key);
                    continue;
                }

                // Находим источник материала
                var source = FindMaterialSource(materialNeeded.Key, sitePosition);
                if (source == null)
                    continue;

                // Получаем доступное количество материала
                int availableAmount = 0;
                if (source.ProductsBank.TryGetValue(materialNeeded.Key, out int productAmount))
                    availableAmount += productAmount;
                if (source.MaterialsBank.TryGetValue(materialNeeded.Key, out int materialAmount))
                    availableAmount += materialAmount;

                if (availableAmount <= 0)
                    continue;

                // Доставляем партию материалов
                int deliveryAmount = Math.Min(DeliveryBatchSize, Math.Min(availableAmount, materialNeeded.Value));

                // Убираем материалы из источника
                if (source.ProductsBank.TryGetValue(materialNeeded.Key, out int prodAmount) && prodAmount > 0)
                {
                    int takeFromProducts = Math.Min(prodAmount, deliveryAmount);
                    source.ProductsBank[materialNeeded.Key] -= takeFromProducts;
                    if (source.ProductsBank[materialNeeded.Key] <= 0)
                        source.ProductsBank.Remove(materialNeeded.Key);
                    deliveryAmount -= takeFromProducts;
                }

                if (deliveryAmount > 0 && source.MaterialsBank.TryGetValue(materialNeeded.Key, out int matAmount))
                {
                    int takeFromMaterials = Math.Min(matAmount, deliveryAmount);
                    source.MaterialsBank[materialNeeded.Key] -= takeFromMaterials;
                    if (source.MaterialsBank[materialNeeded.Key] <= 0)
                        source.MaterialsBank.Remove(materialNeeded.Key);
                }

                // Добавляем материалы на строительную площадку
                site.AddMaterials(materialNeeded.Key, deliveryAmount);

                // Обновляем список необходимых материалов
                neededMaterials[materialNeeded.Key] -= deliveryAmount;
                if (neededMaterials[materialNeeded.Key] <= 0)
                {
                    materialsToRemove.Add(materialNeeded.Key);
                }
            }

            // Удаляем доставленные материалы из списка
            foreach (var material in materialsToRemove)
            {
                neededMaterials.Remove(material);
            }

            // Если все материалы доставлены, удаляем площадку из списка ожидающих
            if (neededMaterials.Count == 0)
            {
                _pendingDeliveries.Remove(site);
            }
        }

        /// <summary>
        /// Вычисляет расстояние между двумя позициями (расстояние между точками на плоскачти)
        /// </summary>
        /// <param name="pos1">Координаты первой точки (объекта)</param>
        /// <param name="pos2">Координаты второй точки (объекта)</param>
        /// <returns>Расстояние в дробных тайлах</returns>
        private double CalculateDistance(Position pos1, Position pos2)
        {
            return Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);
        }
    }
}

