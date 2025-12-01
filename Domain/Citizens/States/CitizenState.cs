namespace Domain.Citizens.States
{
    public enum CitizenState
    {
        // Ничего
        Idle,

        // Работа
        SearchingWork,
        GoingToWork,
        Working,
        
        // Учёба
        GoingToStudy,
        Studying,

        // Дом
        GoingHome,
        SearchingHome,

        // Коммерческие состояния
        GoingToCommercial,
        WaitingInCommercialQueue,
        UsingCommercialService,
        LeavingCommercial,

        InTransport,
        GoingToTransport
    }
}
