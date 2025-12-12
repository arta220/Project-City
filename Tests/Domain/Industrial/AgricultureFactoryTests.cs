    /// <summary>
    /// Сельскохозяйственный комплекс
    /// Включает растениеводство, животноводство, производство удобрений
    /// Учитывает урожайность и сезонность
    /// </summary>
    public class AgricultureFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 20,
                area: new Area(6, 6),
                type: IndustrialBuildingType.Factory
            );

            // Цех растениеводства - выращивание зерновых культур
            building.AddWorkshop(
                NaturalResourceType.Seeds,
                ProductType.Grains,
                coeff: 5
            );

            // Цех растениеводства - выращивание овощей
            building.AddWorkshop(
                NaturalResourceType.Seeds,
                ProductType.Vegetables,
                coeff: 6
            );

            // Цех растениеводства - выращивание фруктов
            building.AddWorkshop(
                NaturalResourceType.Seeds,
                ProductType.Fruits,
                coeff: 4
            );

            // Цех растениеводства - общий урожай (с применением удобрений)
            building.AddWorkshop(
                NaturalResourceType.Fertilizer,
                ProductType.Crops,
                coeff: 8
            );

            // Цех животноводства - производство молочных продуктов
            building.AddWorkshop(
                NaturalResourceType.Livestock,
                ProductType.DairyProducts,
                coeff: 3
            );

            // Цех животноводства - производство мяса
            building.AddWorkshop(
                NaturalResourceType.Livestock,
                ProductType.Meat,
                coeff: 2
            );

            // Цех животноводства - производство яиц
            building.AddWorkshop(
                NaturalResourceType.Feed,
                ProductType.Eggs,
                coeff: 5
            );

            // Цех производства удобрений (для повышения урожайности)
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                NaturalResourceType.Fertilizer,
                coeff: 4
            );

            // Цех переработки сельскохозяйственной продукции
            building.AddWorkshop(
                ProductType.Crops,
                ProductType.ProcessedFood,
                coeff: 3
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.Seeds] = 500;
            building.MaterialsBank[NaturalResourceType.Fertilizer] = 300;
            building.MaterialsBank[NaturalResourceType.Feed] = 400;
            building.MaterialsBank[NaturalResourceType.Livestock] = 100;
            building.MaterialsBank[NaturalResourceType.Chemicals] = 200;
            building.MaterialsBank[NaturalResourceType.Water] = 600;
            building.MaterialsBank[NaturalResourceType.Energy] = 150;

            return building;
        }
    }

    /// <summary>
    /// Рыбодобывающий комплекс
    /// Включает флот, переработку рыбы и хранение
    /// </summary>
    public class FishingIndustryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 25,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );

            // Цех рыбодобычи - добыча рыбы (требует топливо для флота)
            building.AddWorkshop(
                NaturalResourceType.FuelForFleet,
                NaturalResourceType.Fish,
                coeff: 6
            );

            // Цех переработки рыбы - первичная обработка
            building.AddWorkshop(
                NaturalResourceType.Fish,
                ProductType.ProcessedFish,
                coeff: 5
            );

            // Цех консервирования рыбы
            building.AddWorkshop(
                ProductType.ProcessedFish,
                ProductType.CannedFish,
                coeff: 4
            );

            // Цех заморозки рыбы (рыба + лед для хранения -> замороженная рыба)
            // Примечание: лед используется как ресурс для процесса заморозки
            building.AddWorkshop(
                NaturalResourceType.Fish,
                ProductType.FrozenFish,
                coeff: 4
            );

            // Цех переработки рыбы в рыбные продукты
            building.AddWorkshop(
                ProductType.ProcessedFish,
                ProductType.FishProducts,
                coeff: 4
            );

            // Цех производства рыбной муки (из отходов переработки)
            building.AddWorkshop(
                NaturalResourceType.Fish,
                ProductType.Fishmeal,
                coeff: 7
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.FuelForFleet] = 400;
            building.MaterialsBank[NaturalResourceType.Fish] = 300;
            building.MaterialsBank[NaturalResourceType.Ice] = 500;
            building.MaterialsBank[NaturalResourceType.Energy] = 200;
            building.MaterialsBank[NaturalResourceType.Water] = 300;

            return building;
        }
    }
