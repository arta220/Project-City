using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Common.Enums;
using Services.Finance;
using Services.Materials;
using System.Collections.ObjectModel;
using System.Linq;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для диалога продажи материалов
    /// </summary>
    /// <remarks>
    /// Класс управляет логикой продажи природных ресурсов:
    /// 1. Отображает список материалов из инвентаря с ценами продажи
    /// 2. Позволяет выбирать количество каждого материала для продажи
    /// 3. Проверяет наличие достаточного количества материала
    /// 4. Выполняет финансовую операцию продажи
    /// 5. Обновляет инвентарь материалов
    /// </remarks>
    public partial class SellMaterialsViewModel : ObservableObject
    {
        private readonly IFinanceService _financeService;
        private readonly IMaterialInventoryService _inventory;

        /// <summary>
        /// Получает коллекцию материалов, доступных для продажи
        /// </summary>
        /// <value>
        /// Коллекция <see cref="MaterialItemViewModel"/>, представляющая каждый тип материала с его ценой продажи и количеством в инвентаре
        /// </value>
        public ObservableCollection<MaterialItemViewModel> Items { get; } = new();

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="SellMaterialsViewModel"/>
        /// </summary>
        /// <param name="financeService">Сервис управления финансами</param>
        /// <param name="inventory">Сервис управления инвентарем материалов</param>
        /// <remarks>
        /// Конструктор инициализирует ViewModel с фиксированными ценами продажи материалов:
        /// - Железо: 8 (80% от цены покупки)
        /// - Нефть: 20 (80% от цены покупки)
        /// - Газ: 16 (80% от цены покупки)
        /// - Дерево: 4 (80% от цены покупки)
        /// - Медь: 9.6 (80% от цены покупки)
        /// </remarks>
        public SellMaterialsViewModel(IFinanceService financeService, IMaterialInventoryService inventory)
        {
            _financeService = financeService;
            _inventory = inventory;
            var prices = new System.Collections.Generic.Dictionary<NaturalResourceType, float>
            {
                { NaturalResourceType.Iron, 8f },
                { NaturalResourceType.Oil, 20f },
                { NaturalResourceType.Gas, 16f },
                { NaturalResourceType.Wood, 4f },
                { NaturalResourceType.Copper, 9.6f }
            };

            foreach (var kv in prices)
            {
                Items.Add(new MaterialItemViewModel(kv.Key, kv.Key.ToString(), kv.Value, _inventory.GetQuantity(kv.Key)));
            }
        }

        /// <summary>
        /// Команда для выполнения продажи выбранных материалов
        /// </summary>
        /// <remarks>
        /// При выполнении команды:
        /// 1. Рассчитывается общая стоимость выбранных материалов для продажи
        /// 2. Если стоимость больше 0, проверяется наличие достаточного количества каждого материала
        /// 3. Материалы удаляются из инвентаря
        /// 4. Добавляется доход от продажи в бюджет города
        /// 5. Обновляется количество оставшихся материалов
        /// </remarks>
        [RelayCommand]
        private void Sell()
        {
            float total = Items.Sum(i => i.Price * i.SelectedQuantity);
            if (total <= 0) return;

            foreach (var item in Items)
            {
                if (item.SelectedQuantity > 0 && _inventory.GetQuantity(item.ResourceType) >= item.SelectedQuantity)
                {
                    _inventory.Remove(item.ResourceType, item.SelectedQuantity);
                    item.Owned -= item.SelectedQuantity;
                }
            }

            _financeService.AddIncome(total, Domain.Finance.IncomeCategory.Other, "Продажа материалов");
        }
    }
}