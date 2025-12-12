namespace Domain.Common.Enums
{
    /// <summary>
    /// Типы продуктов, производимых промышленными зданиями
    /// </summary>
    public enum ProductType
    {
        None = 0,
        Steel,          // Сталь (из железа)
        Fuel,           // Топливо (из нефти)
        Energy,         // Энергия (из газа)
        Paper,          // Бумага (из дерева)
        Electronics,    // Электроника (из меди)
        Plastic,        // Пластик (из нефти)
        Furniture,      // Мебель (из дерева)
        Tools,          // Инструменты (из стали)
        // Продукты картонного завода
        CardboardSheets,        // Картонные листы
        CorrugatedCardboard,    // Гофрированный картон
        SolidCardboard,         // Плотный картон
        CardboardBoxes,         // Картонные коробки
        CardboardTubes,         // Картонные гильзы
        EggPackaging,           // Упаковка для яиц
        CardboardPallet,        // Картонный паллет
        DisplayStand,           // Картонный стенд
        ProtectivePackaging,    // Защитная упаковка
        CustomShapeCardboard,   // Картон сложной формы
        // Продукты завода упаковки
        CardboardBox,       // Картонная коробка
        PlasticBottle,      // Пластиковая бутылка
        GlassJar,           // Стеклянная банка
        AluminiumCan,       // Алюминиевая кружка
        WoodenCrate,        // Деревянный ящик
        FoodContainer,      // Пищевой контейнер
        ShippingBox,        // Транспортная коробка
        CosmeticBottle,     // Косметический флакон
        PharmaceuticalPack, // Фармацевтическая упаковка
        GiftBox            // Подарочная упаковка
        // Продукты сельского хозяйства
        Crops,             // Урожай (растениеводство)
        Vegetables,        // Овощи
        Fruits,            // Фрукты
        Grains,            // Зерновые культуры
        DairyProducts,     // Молочные продукты (животноводство)
        Meat,              // Мясо (животноводство)
        Eggs,              // Яйца (животноводство)
        ProcessedFood,     // Переработанные продукты питания
        // Продукты рыбодобывающей отрасли
        ProcessedFish,     // Переработанная рыба
        CannedFish,        // Консервированная рыба
        FrozenFish,        // Замороженная рыба
        FishProducts,      // Рыбные продукты
        Fishmeal           // Рыбная мука
    }
}

