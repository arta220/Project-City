using Domain.Buildings;
using Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Services.JewelryProduction
{
    public class JewelryProductionService
    {
        private List<JewelryFactory> _factories = new List<JewelryFactory>();

        // Статистика для графиков
        public List<int> GlobalProductionHistory { get; private set; } = new List<int>();
        public List<decimal> GlobalRevenueHistory { get; private set; } = new List<decimal>();

        public event Action FactoryRegistered;

        public void RegisterFactory(JewelryFactory factory)
        {
            if (!_factories.Contains(factory))
            {
                _factories.Add(factory);
                FactoryRegistered?.Invoke();
            }
        }

        public ProductionResult ProduceJewelry(JewelryType type, JewelryMaterial material, int quantity)
        {
            if (quantity <= 0)
                return new ProductionResult(false, 0, 0);

            var activeFactories = _factories.Where(f => f.IsActive).ToList();
            if (!activeFactories.Any())
                return new ProductionResult(false, 0, 0);

            var totalProduced = 0;
            var totalRevenue = 0m;
            var remainingQuantity = quantity;

            // Распределяем производство между активными фабриками
            foreach (var factory in activeFactories)
            {
                if (remainingQuantity <= 0) break;

                // Каждая фабрика может произвести часть
                var factoryCapacity = factory.ProductionCapacity;
                var canProduce = Math.Min(remainingQuantity, factoryCapacity);
                var revenue = CalculateRevenue(type, material, canProduce);

                factory.AddProduction(canProduce, revenue);

                totalProduced += canProduce;
                totalRevenue += revenue;
                remainingQuantity -= canProduce;
            }

            // Сохраняем глобальную статистику
            if (totalProduced > 0)
            {
                GlobalProductionHistory.Add(totalProduced);
                GlobalRevenueHistory.Add(totalRevenue);
            }

            return new ProductionResult(totalProduced > 0, totalProduced, totalRevenue);
        }

        private decimal CalculateRevenue(JewelryType type, JewelryMaterial material, int quantity)
        {
            // Базовая цена + премия за материал и тип
            var basePrice = 100m;
            var materialMultiplier = (int)material * 0.5m;
            var typeMultiplier = (int)type * 0.3m;

            return (basePrice + materialMultiplier + typeMultiplier) * quantity;
        }

        // Методы для графиков и статистики
        public int GetTotalJewelryProduced() => _factories.Sum(f => f.TotalProduced);
        public decimal GetTotalRevenue() => _factories.Sum(f => f.TotalRevenue);
        public int GetFactoryCount() => _factories.Count;
        public int GetActiveFactoryCount() => _factories.Count(f => f.IsActive);

        public double GetAverageProductionPerFactory() =>
            _factories.Count > 0 ? _factories.Average(f => (double)f.TotalProduced) : 0;

        public IEnumerable<JewelryFactory> GetActiveFactories() => _factories.Where(f => f.IsActive);

        public void SetFactoryActive(JewelryFactory factory, bool active)
        {
            factory.SetActive(active);
        }
    }

    public record ProductionResult(bool Success, int QuantityProduced, decimal Revenue);
}