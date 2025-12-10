namespace Domain.Common.Enums
{
    /// <summary>
    /// Типы строительных материалов, используемых при строительстве зданий
    /// </summary>
    public enum ConstructionMaterialType
    {
        None = 0,
        // Основные строительные материалы
        Cement,         // Цемент
        Concrete,       // Бетон
        Bricks,         // Кирпич
        Steel,          // Сталь (используется из ProductType.Steel)
        Rebar,          // Арматура (железобетонные изделия)
        Sand,           // Песок
        Gravel,         // Щебень
        Clay,           // Глина
        Limestone,      // Известняк
        
        // Дополнительные материалы
        Glass,          // Стекло (используется из NaturalResourceType.Glass)
        Wood,           // Дерево (используется из NaturalResourceType.Wood)
        Insulation,     // Утеплитель
        Roofing,        // Кровельные материалы
        Pipes,          // Трубы
        Wires,          // Провода
        Tiles           // Плитка
    }
}

