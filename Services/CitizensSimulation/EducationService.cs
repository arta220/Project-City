using Services.Interfaces;
using Domain.Citizens;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Сервис, отвечающий за образование граждан.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется внутри <see cref="CitizenController"/> для обновления состояния граждан, находящихся в процессе обучения.
    /// - Вызывается на каждом тике симуляции через <see cref="CitizenSimulationService"/>.
    /// 
    /// </remarks>
    public class EducationService : IEducationService
    {
        /// <summary>
        /// Обновляет образовательный прогресс гражданина на текущем тике.
        /// </summary>
        /// <param name="citizen">Гражданин, которого необходимо обновить.</param>
        /// <param name="tick">Номер текущего тика симуляции.</param>
        public void UpdateEducation(Citizen citizen, int tick)
        {
            throw new NotImplementedException();
        }
    }
}
