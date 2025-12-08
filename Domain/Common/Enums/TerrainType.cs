using System.ComponentModel;

namespace Domain.Common.Enums
{
    public enum TerrainType
    {
        [Description("Вода")]
        Water,

        [Description("Равнина")]
        Plain,

        [Description("Луг")]
        Meadow,

        [Description("Лес")]
        Forest,

        [Description("Горы")]
        Mountain
    }
}