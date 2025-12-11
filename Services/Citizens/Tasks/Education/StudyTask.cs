using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Enums;
using Domain.Common.Time;

public class StudyTask : ICitizenTask
{
    private readonly Citizen _citizen;
    private readonly EducationBuilding _school;
    private int _ticksRemaining;

    public bool IsCompleted => _ticksRemaining <= 0;

    public StudyTask(Citizen citizen, EducationBuilding school)
    {
        _citizen = citizen;
        _school = school;

        // Можно задать длительность на основе возраста или типа учебного заведения
        _ticksRemaining = GetTicksForEducation(_citizen.Age);
    }

    private int GetTicksForEducation(int age)
    {
        return age switch
        {
            >= 7 and < 15 => 10, // школа
            >= 15 and < 18 => 15, // колледж
            >= 18 => 20, // университет
            _ => 0
        };
    }

    public void Execute(Citizen citizen, SimulationTime time)
    {
        if (_ticksRemaining > 0)
        {
            _ticksRemaining--;
        }
        else
        {
            // Устанавливаем уровень образования в зависимости от возраста
            _citizen.EducationLevel = _citizen.Age switch
            {
                >= 7 and < 15 => EducationType.School,
                >= 15 and < 18 => EducationType.College,
                >= 18 => EducationType.University,
                _ => EducationType.None
            };
        }
    }
}
