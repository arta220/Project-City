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
    /// ViewModel для диалога покупки материалов
    /// </summary>
    /// <remarks>
    /// Класс управляет логикой покупки природных ресурсов:
    /// 1. Отображает список доступных для покупки материалов с ценами
    /// 2. Позволяет выбирать количество каждого материала
    /// 3. Выполняет финансовую операцию покупки
    /// 4. Обновляет инвентарь материалов
    /// </remarks>
    public partial class BuyMaterialsViewModel : ObservableObject
    {
        private readonly IFinanceService _financeService;
        private readonly IMaterialInventoryService _inventory;

        /// <summary>
        /// Получает коллекцию материалов, доступных для покупки
        /// </summary>
        /// <value>
        /// Коллекция <see cref="MaterialItemViewModel"/>, представляющая каждый тип материала с его ценой и количеством
        /// </value>
        public ObservableCollection<MaterialItemViewModel> Items { get; } = new();

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="BuyMaterialsViewModel"/>
        /// </summary>
        /// <param name="financeService">Сервис управления финансами</param>
        /// <param name="inventory">Сервис управления инвентарем материалов</param>
        /// <remarks>
        /// Конструктор инициализирует ViewModel с фиксированными ценами на материалы:
        /// - Железо: 10
        /// - Нефть: 25
        /// - Газ: 20
        /// - Дерево: 5
        /// - Медь: 12
        /// </remarks>
        public BuyMaterialsViewModel(IFinanceService financeService, IMaterialInventoryService inventory)
        {
            _financeService = financeService;
            _inventory = inventory;
            var prices = new System.Collections.Generic.Dictionary<NaturalResourceType, float>
            {
                { NaturalResourceType.Iron, 10f },
                { NaturalResourceType.Oil, 25f },
                { NaturalResourceType.Gas, 20f },
                { NaturalResourceType.Wood, 5f },
                { NaturalResourceType.Copper, 12f }
            };

            foreach (var kv in prices)
            {
                Items.Add(new MaterialItemViewModel(kv.Key, kv.Key.ToString(), kv.Value, _inventory.GetQuantity(kv.Key)));
            }
        }

        /// <summary>
        /// Команда для выполнения покупки выбранных материалов
        /// </summary>
        /// <remarks>
        /// При выполнении команды:
        /// 1. Рассчитывается общая стоимость выбранных материалов
        /// 2. Если стоимость больше 0, списываются средства из бюджета
        /// 3. Добавляются купленные материалы в инвентарь
        /// 4. Сбрасывается выбранное количество для каждого материала
        /// 5. Обновляется количество имеющихся материалов
        /// </remarks>
        [RelayCommand]
        private void Buy()
        {
            float total = Items.Sum(i => i.Price * i.SelectedQuantity);
            if (total <= 0) return;

            _financeService.AddExpense(total, Domain.Finance.ExpenseCategory.Other, "Покупка материалов");
            foreach (var item in Items)
            {
                if (item.SelectedQuantity > 0)
                {
                    _inventory.Add(item.ResourceType, item.SelectedQuantity);
                    item.Owned += item.SelectedQuantity;
                    item.SelectedQuantity = 0;
                }
            }
        }
    }
}