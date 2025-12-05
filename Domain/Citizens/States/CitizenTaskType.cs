namespace Domain.Citizens.States
{
    public enum CitizenTaskType
    {
        MoveToPosition,
        EnterBuilding,
        Work,
        Study,
        Sleep,
        Shop,

        // Простейшие задачи для работы с личным транспортом
        WalkToCar,   // дойти пешком до своей машины
        EnterCar,    // сесть в машину
        ExitCar      // выйти из машины
    }
}
