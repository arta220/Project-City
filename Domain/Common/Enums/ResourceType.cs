using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Enums
{
    public enum ResourceType
    {
        //PharmaceuticalFactory
        Chemicals,        // Химикаты
        Medicine,         // Лекарства

        //RecyclingPlantFactory
        PlasticWaste,     // Пластиковые отходы
        Plastic,

        Petroleum,           // Нефть
        NaturalGas,          // Природный газ
        Salt,               // Соль
        Sulfur,             // Сера
        Ammonia,            // Аммиак
        OrganicCompounds,   // Органические соединения
        Minerals,           // Минералы
        Water,              // Вода (уже в NaturalResourceType, но для единообразия)
        Energy              // Энергия
    }
}
