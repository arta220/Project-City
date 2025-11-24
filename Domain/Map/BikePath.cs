using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Map
{
    public class BikePath : MapObject
    {
        public bool IsProtected { get; set; }

        public BikePath(Area area, bool isProtected = false) : base(area)
        {
            IsProtected = isProtected;
        }
    }
}