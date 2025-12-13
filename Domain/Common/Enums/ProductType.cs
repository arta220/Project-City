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
        GiftBox,           // Подарочная упаковка
                           // Продукты косметического завода
        // Продукты завода косметики
        SkinCream,          // Крем для кожи
        Shampoo,            // Шампунь
        Perfume,            // Духи
        Makeup,             // Декоративная косметика
        HairCareProduct,    // Средство для ухода за волосами
        Sunscreen,          // Солнцезащитное средство
        MakeupKit,          // Набор для макияжа
        HygieneProduct,     // Гигиенический продукт
        ScentedCandle,      // Ароматическая свеча
        CosmeticSet,        // Косметический набор
        // Продукты алкогольного завода
        Beer,               // Пиво
        Vodka,              // Водка
        Wine,               // Вино
        Whiskey,            // Виски
        Rum,                // Ром
        Tequila,            // Текила
        Gin,                // Джин
        Brandy,             // Бренди
        Champagne,          // Шампанское
        Liqueur             // Ликёр


    }
}

