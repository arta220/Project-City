using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Enums
{
    public enum NaturalResourceType
    {
        None = 0,
        Iron,
        Oil,
        Gas,
        Wood,
        Copper,
        Energy,
        WoodChips,      // Древесная щепа (для картона)
        RecycledPaper,   // Макулатура (для картона)
        Chemicals,       // Химические реагенты (для картона и упаковки)
        Water,           // Вода (для картона)
        Glass,           // Стекло (для упаковки)
        Aluminium,       // Алюминий (для упаковки)
        Ink,            // Краска (для упаковки)
        Sand,           // Песок (для строительства)
        Gravel,         // Щебень (для строительства)
        Clay,           // Глина (для строительства)
        Limestone       // Известняк (для строительства)
    }
}
