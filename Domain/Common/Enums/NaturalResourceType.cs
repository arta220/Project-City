using System.ComponentModel;

namespace Domain.Common.Enums
{
    public enum NaturalResourceType
    {
        [Description("Нет")]
        None,

        [Description("Железо")]
        Iron,

        [Description("Нефть")]
        Oil,

        [Description("Газ")]
        Gas,

        [Description("Медь")]
        Copper,

        [Description("Дерево")]
        Wood
    }
}