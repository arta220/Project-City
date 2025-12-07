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
        GiftBox,            // Подарочная упаковка

        // Ювелирные материалы
        Gold,          
        Silver,         
        Platinum,       
        Diamond,        
        Ruby,           
        Emerald,        
        Pearl,          // Жемчуг
        Sapphire,       // Сапфир
        
        // Базовые ювелирные изделия
        Ring,           // Кольцо
        Necklace,       // Ожерелье
        Bracelet,       // Браслет
        Earrings,       // Серьги
        Pendant,        // Кулон
        
        // Премиум изделия (с камнями)
        DiamondRing,    // Кольцо с бриллиантом
        RubyNecklace,   // Ожерелье с рубином
        EmeraldBracelet,// Браслет с изумрудом
        PearlEarrings,  // Серьги с жемчугом
        SapphirePendant,// Кулон с сапфиром
        
        // Эксклюзивные изделия
        PlatinumRing,   // Платиновое кольцо
        GoldNecklace,  // Золотое ожерелье
        DiamondEarrings,// Серьги с бриллиантами
        MultiGemRing,    // Кольцо с несколькими камнями

        // Стекольные материалы
        RawGlass,        // Сырое стекло
        ColoredGlass,    // Цветное стекло
        TemperedGlass,   // Закаленное стекло
        CrystalGlass,    // Хрустальное стекло
        
        // Базовые стекольные изделия
        GlassBottle,     // Стеклянная бутылка
        GlassVase,       // Стеклянная ваза
        GlassWindow,     // Стеклянное окно
        GlassMirror,     // Зеркало
        GlassTableware,  // Стеклянная посуда
        
        // Премиум стекольные изделия
        CrystalVase,     // Хрустальная ваза
        StainedGlass,    // Витражное стекло
        GlassSculpture,  // Стеклянная скульптура
        DecorativeGlass, // Декоративное стекло
        
        // Эксклюзивные стекольные изделия
        ArtGlass,        // Художественное стекло
        LuxuryGlassware  // Роскошная стеклянная посуда
    }
}

