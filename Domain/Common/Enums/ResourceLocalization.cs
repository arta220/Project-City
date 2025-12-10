using System;
using System.Collections.Generic;
using Domain.Citizens.States;

namespace Domain.Common.Enums
{
    /// <summary>
    /// Класс для локализации названий ресурсов и продуктов
    /// </summary>
    public static class ResourceLocalization
    {
        private static readonly Dictionary<Enum, string> _resourceNames = new()
        {
            // Natural Resources
            { NaturalResourceType.None, "Отсутствует" },
            { NaturalResourceType.Iron, "Железо" },
            { NaturalResourceType.Oil, "Нефть" },
            { NaturalResourceType.Gas, "Газ" },
            { NaturalResourceType.Wood, "Дерево" },
            { NaturalResourceType.Copper, "Медь" },
            { NaturalResourceType.Energy, "Энергия" },
            { NaturalResourceType.WoodChips, "Древесная щепа" },
            { NaturalResourceType.RecycledPaper, "Макулатура" },
            { NaturalResourceType.Chemicals, "Химические реагенты" },
            { NaturalResourceType.Water, "Вода" },
            { NaturalResourceType.Glass, "Стекло" },
            { NaturalResourceType.Aluminium, "Алюминий" },
            { NaturalResourceType.Ink, "Краска" },

            // Professions
            { CitizenProfession.Chef, "Повар" },
            { CitizenProfession.Seller, "Продавец" },
            { CitizenProfession.Doctor, "Врач" },
            { CitizenProfession.UtilityWorker, "Работник ЖКХ" },
            { CitizenProfession.FactoryWorker, "Рабочий завода" },

            // Products
            { ProductType.None, "Отсутствует" },
            { ProductType.Steel, "Сталь" },
            { ProductType.Fuel, "Топливо" },
            { ProductType.Energy, "Энергия" },
            { ProductType.Paper, "Бумага" },
            { ProductType.Electronics, "Электроника" },
            { ProductType.Plastic, "Пластик" },
            { ProductType.Furniture, "Мебель" },
            { ProductType.Tools, "Инструменты" },

            // Cardboard products
            { ProductType.CardboardSheets, "Картонные листы" },
            { ProductType.CorrugatedCardboard, "Гофрированный картон" },
            { ProductType.SolidCardboard, "Плотный картон" },
            { ProductType.CardboardBoxes, "Картонные коробки" },
            { ProductType.CardboardTubes, "Картонные гильзы" },
            { ProductType.EggPackaging, "Упаковка для яиц" },
            { ProductType.CardboardPallet, "Картонный паллет" },
            { ProductType.DisplayStand, "Картонный стенд" },
            { ProductType.ProtectivePackaging, "Защитная упаковка" },
            { ProductType.CustomShapeCardboard, "Картон сложной формы" },

            // Packaging products
            { ProductType.CardboardBox, "Картонная коробка" },
            { ProductType.PlasticBottle, "Пластиковая бутылка" },
            { ProductType.GlassJar, "Стеклянная банка" },
            { ProductType.AluminiumCan, "Алюминиевая банка" },
            { ProductType.WoodenCrate, "Деревянный ящик" },
            { ProductType.FoodContainer, "Пищевой контейнер" },
            { ProductType.ShippingBox, "Транспортная коробка" },
            { ProductType.CosmeticBottle, "Косметический флакон" },
            { ProductType.PharmaceuticalPack, "Фармацевтическая упаковка" },
            { ProductType.GiftBox, "Подарочная упаковка" },
        };

        /// <summary>
        /// Получить русское название ресурса или продукта
        /// </summary>
        public static string GetLocalizedName(Enum resource)
        {
            if (_resourceNames.TryGetValue(resource, out var name))
                return name;

            return resource.ToString();
        }

        /// <summary>
        /// Получить словарь всех локализованных названий для типа enum
        /// </summary>
        public static Dictionary<Enum, string> GetLocalizedNames<T>() where T : Enum
        {
            var result = new Dictionary<Enum, string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                if (value is T enumValue)
                {
                    result[enumValue] = GetLocalizedName(enumValue);
                }
            }
            return result;
        }
    }
}

