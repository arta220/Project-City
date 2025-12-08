using Services.Common;
using Domain.Finance;
using Services.Time;
using System;

namespace Services.Finance
{
    /// <summary>
    /// Определение сервиса обслуживания города, ответственного за регулярные расходы на поддержание инфраструктуры
    /// </summary>
    public interface ICityMaintenanceService : IUpdatable
    {
        /// <summary>
        /// Получение общей стоимости обслуживания города с момента запуска сервиса
        /// </summary>
        float TotalMaintenanceCost { get; }

        /// <summary>
        /// Событие, возникающее при успешном выполнении обслуживания
        /// </summary>
        event Action<string> MaintenancePerformed;

        /// <summary>
        /// Событие, возникающее при неудачной попытке обслуживания
        /// </summary>
        event Action<string> MaintenanceFailed;
    }

    /// <summary>
    /// Реализация сервиса обслуживания города, выполняющего регулярные платежи за поддержание инфраструктуры
    /// </summary>
    public class CityMaintenanceService : ICityMaintenanceService
    {
        private const int TicksPerWeek = 7;
        private const float MaintenanceCost = 1000f;

        private readonly IFinanceService _financeService;
        private readonly ISimulationTimeService _timeService;
        private int _lastMaintenanceTick = 0;

        /// <summary>
        /// Получение общей стоимости обслуживания города с момента запуска сервиса
        /// </summary>
        public float TotalMaintenanceCost { get; private set; }

        /// <summary>
        /// Событие, возникающее при успешном выполнении обслуживания
        /// </summary>
        public event Action<string> MaintenancePerformed;

        /// <summary>
        /// Событие, возникающее при неудачной попытке обслуживания
        /// </summary>
        public event Action<string> MaintenanceFailed;

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="CityMaintenanceService"/>.
        /// </summary>
        /// <param name="financeService">Сервис управления финансами</param>
        /// <param name="timeService">Сервис времени симуляции</param>
        public CityMaintenanceService(IFinanceService financeService, ISimulationTimeService timeService)
        {
            _financeService = financeService;
            _timeService = timeService;
        }

        /// <summary>
        /// Обновление состояния сервиса с проверкой необходимости выполнения обслуживания
        /// </summary>
        /// <param name="time">Текущее время симуляции</param>
        public void Update(Domain.Common.Time.SimulationTime time)
        {
            int currentTick = time.TotalTicks;

            if (currentTick >= _lastMaintenanceTick + TicksPerWeek)
            {
                if (TryPerformMaintenance(currentTick))
                {
                    _lastMaintenanceTick = currentTick;
                }
            }
        }

        /// <summary>
        /// Пытается выполнить обслуживание города, списывая средства с бюджета.
        /// </summary>
        /// <param name="currentTick">Текущий тик симуляции</param>
        /// <returns> true,если обслуживание выполнено успешно; иначе - false</returns>
        private bool TryPerformMaintenance(int currentTick)
        {
            if (_financeService.Budget.Balance < MaintenanceCost)
            {
                MaintenanceFailed?.Invoke($"Недостаточно средств для обслуживания. Требуется: {MaintenanceCost}, доступно: {_financeService.Budget.Balance}");
                return false;
            }

            try
            {
                _financeService.AddExpense(
                    amount: MaintenanceCost,
                    category: ExpenseCategory.Maintenance,
                    description: $"Еженедельное обслуживание города (тик {currentTick})"
                );

                TotalMaintenanceCost += MaintenanceCost;
                MaintenancePerformed?.Invoke($"Обслуживание выполнено. Списано: {MaintenanceCost}");

                return true;
            }
            catch (Exception ex)
            {
                MaintenanceFailed?.Invoke($"Ошибка при выполнении обслуживания: {ex.Message}");
                return false;
            }
        }
    }
}