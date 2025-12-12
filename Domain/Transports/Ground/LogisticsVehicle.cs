using Domain.Common.Base;
using Domain.Map;
using Domain.Transports;

namespace Domain.Transports.Ground
{
    /// <summary>
    /// Базовый класс для логистического транспорта
    /// </summary>
    public abstract class LogisticsVehicle : Transport
    {
        /// <summary>
        /// Текущий груз (в условных единицах)
        /// </summary>
        public int CurrentCargo { get; set; }

        /// <summary>
        /// Максимальная грузоподъемность
        /// </summary>
        public int MaxCargo { get; protected set; }

        /// <summary>
        /// Текущий пробег
        /// </summary>
        public int Mileage { get; protected set; }

        /// <summary>
        /// Состояние транспорта (0-100%)
        /// </summary>
        public int Condition { get; protected set; }

        /// <summary>
        /// Топливо (0-100%)
        /// </summary>
        public int FuelLevel { get; protected set; }

        protected LogisticsVehicle(Area area, float speed, int maxCargo, TransportType type)
            : base(area, speed)
        {
            Type = type;
            MaxCargo = maxCargo;
            CurrentCargo = 0;
            Condition = 100;
            FuelLevel = 100;
            Mileage = 0;
        }

        /// <summary>
        /// Загрузить груз
        /// </summary>
        public virtual bool LoadCargo(int amount)
        {
            if (CurrentCargo + amount > MaxCargo)
                return false;

            CurrentCargo += amount;
            return true;
        }

        /// <summary>
        /// Разгрузить
        /// </summary>
        public virtual void UnloadCargo()
        {
            CurrentCargo = 0;
        }

        /// <summary>
        /// Проверить техническое состояние
        /// </summary>
        public virtual void MaintenanceCheck()
        {
            // Каждые 1000 "км" ухудшается состояние
            if (Mileage % 1000 == 0 && Condition > 20)
            {
                Condition -= 5;
            }

            // Расход топлива
            if (CurrentCargo > 0)
            {
                FuelLevel = Math.Max(0, FuelLevel - 1);
            }
        }

        /// <summary>
        /// Отремонтировать
        /// </summary>
        public virtual void Repair()
        {
            Condition = 100;
        }

        /// <summary>
        /// Заправить
        /// </summary>
        public virtual void Refuel()
        {
            FuelLevel = 100;
        }

        /// <summary>
        /// Проверить готовность к поездке
        /// </summary>
        public virtual bool IsReadyForTrip()
        {
            return Condition > 30 && FuelLevel > 20 && CurrentCargo <= MaxCargo;
        }

        /// <summary>
        /// Совершить поездку (увеличивает пробег)
        /// </summary>
        public virtual void MakeTrip(int distance)
        {
            Mileage += distance;
        }
    }

    /// <summary>
    /// Фургон для доставки (маленький)
    /// </summary>
    public class DeliveryVan : LogisticsVehicle
    {
        public DeliveryVan()
            : base(new Area(2, 1), 60f, 500, TransportType.DeliveryVan)
        {
            Capacity = 2; // Водитель + помощник
        }
    }

    /// <summary>
    /// Грузовик средней грузоподъемности
    /// </summary>
    public class CargoTruck : LogisticsVehicle
    {
        public CargoTruck()
            : base(new Area(3, 2), 50f, 2000, TransportType.CargoTruck)
        {
            Capacity = 2;
        }
    }

    /// <summary>
    /// Фура с полуприцепом (большая грузоподъемность)
    /// </summary>
    public class SemiTrailerTruck : LogisticsVehicle
    {
        public bool TrailerAttached { get; private set; } = true;

        public SemiTrailerTruck()
            : base(new Area(4, 2), 40f, 5000, TransportType.SemiTrailerTruck)
        {
            Capacity = 2;
        }

        /// <summary>
        /// Отцепить прицеп
        /// </summary>
        public void DetachTrailer()
        {
            if (TrailerAttached && CurrentCargo == 0)
            {
                TrailerAttached = false;
                MaxCargo = 1000; // Без прицепа меньше грузоподъемность
            }
        }

        /// <summary>
        /// Прицепить прицеп
        /// </summary>
        public void AttachTrailer()
        {
            if (!TrailerAttached)
            {
                TrailerAttached = true;
                MaxCargo = 5000;
            }
        }

        public override bool LoadCargo(int amount)
        {
            if (!TrailerAttached && amount > 1000)
                return false;

            return base.LoadCargo(amount);
        }
    }
}