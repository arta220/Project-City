using Domain.Citizens.States;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class Cafe : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Cafe;

        public Cafe(Area area)
            : base(area, serviceTime: 12, maxQueue: 12, workerCount: 4)
        {
            // ПРИМЕР НАСТРОЙКИ ЗДАНИЯ ДЛЯ ВАКАНСИИ
            // количество вакансий
            Vacancies[CitizenProfession.Chef] = 1;
            Vacancies[CitizenProfession.Seller] = 3;
            // максимальный возраст для вакансий
            MaxAges[CitizenProfession.Chef] = 40;
            MaxAges[CitizenProfession.Seller] = 50;
        }
    }
}