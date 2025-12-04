using Domain.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Buildings.Industrial
{
    /// <summary>
    /// Фармацевтическая промышленность
    /// </summary>
    public class PharmaceuticalFactory : IndustrialBuilding
    {
        public int ChemicalsCount = 0; // Количество ресурсов из которых будет произведены медикаменты
        public int MedicineCount = 0; // Хранение медикаментов

        /// <summary>
        /// Инициализация данных при создании завода
        /// </summary>
        /// <param name="area"> Площадь завода </param>
        public PharmaceuticalFactory(Area area) : base(floors: 2, maxOccupancy: 80, area: area)
        {
            IndustryType = Common.Enums.IndustryType.Pharmaceutical; // Тип
            InputResource = Common.Enums.ResourseType.Chemicals; // Входные ресурсы
            OutputResource = Common.Enums.ResourseType.Medicine; // Производимая продукция
            CurrentWorkers = 10; // Кол-во рабочих
            ProductionPerWorker = 10; // Производимая продукция на рабочего
            TotalProduced = 0;  // Ну для дальнейшего подсчета эффективности, сейчас не используется в дальнейшем хз
            Profit = 0;  // Выручка

            ChemicalsCount = 100;
        }

        /// <summary>
        /// Добавление материалов для производства
        /// </summary>
        /// <param name="count"> Кол-во материалов </param>
        public void AddChemicals(int count)
        {
            ChemicalsCount += count;
        }


        /// <summary>
        /// Производство (2 химиката - 1 медикамент)
        /// </summary>
        /// <returns> true - успешное производство; false - не хватило материала </returns>
        public bool Produce()
        {
            if (CurrentWorkers == 0) return false;            
            int canProduce = CurrentWorkers * ProductionPerWorker;           
            int chemicalsNeeded = canProduce * 2;

            if (ChemicalsCount >= chemicalsNeeded)
            {                
                ChemicalsCount -= chemicalsNeeded;
                MedicineCount += canProduce;
                TotalProduced += canProduce;
                return true;
            }
            return false; // Не хватило химикатов
        }

        
        /// <summary>
        /// Продажа медикаментов
        /// </summary>
        /// <param name="count"> Кол-во продаваемой продукции </param>
        /// <returns> 0 при ошибке </returns>
        public int SellMedicine(int count)
        {
            if (MedicineCount >= count)
            {
                MedicineCount -= count;
                Profit += count * 25; // 25$ за лекарство
                return count;
            }
            return 0; // Не продали
        }

        /// <summary>
        /// Найм рабочих
        /// </summary>
        /// <param name="count"> Кол-во рабочих </param>
        public void HireWorkers(int count)
        {
            CurrentWorkers += count;
            if (CurrentWorkers > MaxWorkers)
                CurrentWorkers = MaxWorkers;
        }
    }
}
