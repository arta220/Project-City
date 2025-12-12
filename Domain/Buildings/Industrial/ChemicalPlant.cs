using Domain.Buildings;
using Domain.Citizens.States;
using Domain.Common.Enums;
using Domain.Map;
using System.Collections.Generic;

namespace Domain.Buildings.Industrial
{
    /// <summary>
    /// Универсальный химический завод - может производить различные типы химической продукции
    /// </summary>
    public class ChemicalPlant : IndustrialBuilding
    {
        /// <summary>
        /// Тип специализации завода (какую основную продукцию производит)
        /// </summary>
        public ChemicalPlantSpecialization Specialization { get; }

        /// <summary>
        /// Уровень технологического развития (влияет на эффективность)
        /// </summary>
        public int TechnologyLevel { get; private set; }

        /// <summary>
        /// Безопасность производства (меньше - больше аварий)
        /// </summary>
        public int SafetyLevel { get; private set; }

        /// <summary>
        /// Уровень загрязнения окружающей среды
        /// </summary>
        public int PollutionLevel { get; private set; }

        /// <summary>
        /// История аварий на производстве
        /// </summary>
        public List<string> AccidentHistory { get; } = new();

        public ChemicalPlant(int floors, int maxOccupancy, Area area,
                           ChemicalPlantSpecialization specialization)
            : base(floors, maxOccupancy, area, IndustrialBuildingType.Factory)
        {
            Specialization = specialization;
            TechnologyLevel = 1;
            SafetyLevel = 80; // 80% безопасность
            PollutionLevel = 30; // 30% загрязнение

            InitializeForSpecialization(specialization);
        }

        private void InitializeForSpecialization(ChemicalPlantSpecialization specialization)
        {
            // Настройка вакансий в зависимости от специализации
            switch (specialization)
            {
                case ChemicalPlantSpecialization.Petrochemicals:
                    Vacancies[CitizenProfession.ChemicalEngineer] = 8;
                    Vacancies[CitizenProfession.Chemist] = 6;
                    Vacancies[CitizenProfession.FactoryWorker] = 30;
                    MaxAges[CitizenProfession.ChemicalEngineer] = 60;
                    MaxAges[CitizenProfession.Chemist] = 62;
                    MaxAges[CitizenProfession.FactoryWorker] = 50;

                    // Цех нефтехимии
                    AddWorkshop(ResourceType.Petroleum, ProductType.Petrochemicals, 5);
                    AddWorkshop(ResourceType.NaturalGas, ProductType.Petrochemicals, 4);
                    AddWorkshop(ProductType.Petrochemicals, ProductType.Plastics, 3);
                    AddWorkshop(ProductType.Petrochemicals, ProductType.SyntheticRubber, 2);

                    // Начальные материалы
                    MaterialsBank[ResourceType.Petroleum] = 800;
                    MaterialsBank[ResourceType.NaturalGas] = 600;
                    MaterialsBank[ResourceType.Energy] = 400;
                    break;

                case ChemicalPlantSpecialization.AgriculturalChemicals:
                    Vacancies[CitizenProfession.Chemist] = 6;
                    Vacancies[CitizenProfession.FactoryWorker] = 25;
                    MaxAges[CitizenProfession.Chemist] = 60;
                    MaxAges[CitizenProfession.FactoryWorker] = 55;

                    // Цех производства удобрений и пестицидов
                    AddWorkshop(ResourceType.Ammonia, ProductType.Fertilizers, 6);
                    AddWorkshop(ResourceType.Sulfur, ProductType.Pesticides, 4);
                    AddWorkshop(ResourceType.Minerals, ProductType.Fertilizers, 3);

                    // Начальные материалы
                    MaterialsBank[ResourceType.Ammonia] = 500;
                    MaterialsBank[ResourceType.Sulfur] = 400;
                    MaterialsBank[ResourceType.Minerals] = 300;
                    break;

                case ChemicalPlantSpecialization.ConsumerChemicals:
                    Vacancies[CitizenProfession.Chemist] = 8;
                    Vacancies[CitizenProfession.FactoryWorker] = 35;
                    MaxAges[CitizenProfession.Chemist] = 62;
                    MaxAges[CitizenProfession.FactoryWorker] = 50;

                    // Цех бытовой химии
                    AddWorkshop(ResourceType.Chemicals, ProductType.Detergents, 8);
                    AddWorkshop(ResourceType.Chemicals, ProductType.Paints, 5);
                    AddWorkshop(ResourceType.OrganicCompounds, ProductType.Cosmetics, 4);
                    AddWorkshop(ResourceType.Chemicals, ProductType.Cleaners, 6);

                    // Начальные материалы
                    MaterialsBank[ResourceType.Chemicals] = 600;
                    MaterialsBank[ResourceType.OrganicCompounds] = 400;
                    MaterialsBank[ResourceType.Water] = 300;
                    break;

                case ChemicalPlantSpecialization.IndustrialChemicals:
                    Vacancies[CitizenProfession.ChemicalEngineer] = 10;
                    Vacancies[CitizenProfession.FactoryWorker] = 40;
                    MaxAges[CitizenProfession.ChemicalEngineer] = 65;
                    MaxAges[CitizenProfession.FactoryWorker] = 55;

                    // Цех промышленной химии
                    AddWorkshop(ResourceType.Chemicals, ProductType.IndustrialSolvents, 7);
                    AddWorkshop(ResourceType.Chemicals, ProductType.Adhesives, 5);
                    AddWorkshop(ResourceType.Chemicals, ProductType.Dyes, 6);
                    AddWorkshop(ResourceType.Chemicals, ProductType.Explosives, 3);

                    // Начальные материалы
                    MaterialsBank[ResourceType.Chemicals] = 700;
                    MaterialsBank[ResourceType.Salt] = 500;
                    MaterialsBank[ResourceType.Energy] = 400;
                    break;
            }

            // Общие для всех химических заводов материалы
            MaterialsBank[ResourceType.Water] = 1000;
        }

        /// <summary>
        /// Запустить производственный цикл с учетом безопасности
        /// </summary>
        public new void RunOnce()
        {
            // Проверка безопасности перед запуском
            if (CheckSafety())
            {
                base.RunOnce();
                PollutionLevel += 1; // Увеличиваем загрязнение

                // Проверка на аварию (чем ниже безопасность, тем выше шанс)
                if (ShouldAccidentHappen())
                {
                    TriggerAccident();
                }
            }
        }

        /// <summary>
        /// Проверить безопасность производства
        /// </summary>
        private bool CheckSafety()
        {
            // Минимальный уровень безопасности для работы - 30%
            return SafetyLevel > 30 && CurrentWorkers.Count > 5;
        }

        /// <summary>
        /// Определить, должна ли произойти авария
        /// </summary>
        private bool ShouldAccidentHappen()
        {
            Random rand = new Random();
            int chance = 100 - SafetyLevel; // Шанс аварии обратно пропорционален безопасности
            return rand.Next(100) < chance;
        }

        /// <summary>
        /// Инициировать аварию на производстве
        /// </summary>
        private void TriggerAccident()
        {
            string[] accidentTypes =
            {
                "Незначительная утечка химикатов",
                "Взрыв в лаборатории",
                "Пожар в цехе",
                "Отравление персонала",
                "Загрязнение окружающей среды",
                "Поломка оборудования"
            };

            Random rand = new Random();
            string accident = accidentTypes[rand.Next(accidentTypes.Length)];
            AccidentHistory.Add($"{DateTime.Now:yyyy-MM-dd}: {accident}");

            // Последствия аварии
            SafetyLevel -= 10;
            PollutionLevel += 20;

            // Уничтожаем часть продукции
            if (ProductsBank.Count > 0)
            {
                var productList = ProductsBank.Keys.ToList();
                var randomProduct = productList[rand.Next(productList.Count)];
                ProductsBank[randomProduct] = Math.Max(0, ProductsBank[randomProduct] - 50);
            }
        }

        /// <summary>
        /// Улучшить технологии завода
        /// </summary>
        public void UpgradeTechnology()
        {
            TechnologyLevel++;
            // Улучшение увеличивает эффективность и снижает загрязнение
            foreach (var workshop in Workshops)
            {
                // Можно добавить логику улучшения цехов
            }
            PollutionLevel = Math.Max(0, PollutionLevel - 5);
        }

        /// <summary>
        /// Повысить уровень безопасности
        /// </summary>
        public void ImproveSafety(int amount = 10)
        {
            SafetyLevel = Math.Min(100, SafetyLevel + amount);
        }

        /// <summary>
        /// Очистить загрязнение
        /// </summary>
        public void ReducePollution(int amount = 15)
        {
            PollutionLevel = Math.Max(0, PollutionLevel - amount);
        }

        /// <summary>
        /// Добавить новый цех производства
        /// </summary>
        public void AddProductionLine(Enum input, Enum output, int coeff = 1)
        {
            AddWorkshop(input, output, coeff);
        }

        /// <summary>
        /// Получить текущий уровень производства
        /// </summary>
        public int GetProductionEfficiency()
        {
            // Эффективность = базовый уровень + технология - загрязнение
            int baseEfficiency = 70;
            return Math.Max(10, Math.Min(100,
                baseEfficiency + (TechnologyLevel * 5) - (PollutionLevel / 10)));
        }
    }

    /// <summary>
    /// Типы специализации химического завода
    /// </summary>
    public enum ChemicalPlantSpecialization
    {
        Petrochemicals,        // Нефтехимия
        AgriculturalChemicals, // Агрохимикаты
        ConsumerChemicals,     // Бытовые химикаты
        IndustrialChemicals    // Промышленные химикаты
    }
}