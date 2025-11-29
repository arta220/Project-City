namespace Domain.Citizens.States
{
    public enum CitizenState
    {
        // Ничего
        Idle,

        // Работа
        GoingToWork,
        Working,
        
        // Учёба
        GoingToSchool,
        Studying,

        // Дом
        GoingHome,
        SearchingHome,

        // Поход в коммерческие здания
        GoingToCommercial,
        UsingCommercialService
    }
}
