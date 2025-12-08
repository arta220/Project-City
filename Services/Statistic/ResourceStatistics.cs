using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Statistic
{
    /// <summary>
    /// Снимок состояния ресурсов на карте в конкретный момент времени.
    /// </summary>
    public class ResourceSnapshot
    {
        public int Tick { get; set; }

        public float Iron { get; set; }
        public float Copper { get; set; }
        public float Oil { get; set; }
        public float Gas { get; set; }
        public float Wood { get; set; }
    }

    /// <summary>
    /// История изменения ресурсов во времени.
    /// </summary>
    public class ResourceStatistics
    {
        public List<ResourceSnapshot> History { get; } = new();
    }
}
