using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.Citizens.Education;

namespace Tests;

[TestClass]
public class Education
{
    private EducationService _service;

    [TestInitialize]
    public void Init()
    {
        // Инициализация сервиса образования перед каждым тестом
        _service = new EducationService();
    }

    [TestMethod]
    // Тест: если у гражданина нет места учёбы (StudyPlace == null), уровень образования не меняется
    public void UpdateEducation_NoStudyPlace_DoesNotChangeLevel()
    {
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            Age = 10,
            EducationLevel = EducationType.None,
            StudyPlace = null
        };

        _service.UpdateEducation(citizen, new SimulationTime(0));

        Assert.AreEqual(EducationType.None, citizen.EducationLevel);
    }

    [TestMethod]
    // Тест: при наличии учебного заведения у гражданина и возрасте 10 лет уровень должен подняться до School
    public void UpdateEducation_WithStudyPlace_SetsSchoolForAge10()
    {
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            Age = 10,
            EducationLevel = EducationType.None
        };

        var school = new EducationBuilding(floors: 1, maxOccupancy: 100, area: new Area(3, 3))
        {
            Type = EducationType.School,
            Capacity = 100
        };

        citizen.StudyPlace = school;

        _service.UpdateEducation(citizen, new SimulationTime(0));

        Assert.AreEqual(EducationType.School, citizen.EducationLevel);
    }

    [TestMethod]
    // Тест: если уровень образования уже выше ожидаемого (University > School), он не должен понижаться
    public void UpdateEducation_DoesNotDowngradeIfAlreadyHigher()
    {
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            Age = 10,
            EducationLevel = EducationType.University
        };

        var school = new EducationBuilding(floors: 1, maxOccupancy: 100, area: new Area(3, 3))
        {
            Type = EducationType.School,
            Capacity = 100
        };

        citizen.StudyPlace = school;

        _service.UpdateEducation(citizen, new SimulationTime(0));

        Assert.AreEqual(EducationType.University, citizen.EducationLevel);
    }

    [DataTestMethod]
    // Параметризованный тест: проверяет соответствие возраста требуемому уровню образования на границах
    [DataRow(6, EducationType.None)]
    [DataRow(7, EducationType.School)]
    [DataRow(14, EducationType.School)]
    [DataRow(15, EducationType.College)]
    [DataRow(17, EducationType.College)]
    [DataRow(18, EducationType.University)]
    public void UpdateEducation_AgeBoundaries_MapToExpectedLevel(int age, EducationType expected)
    {
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            Age = age,
            EducationLevel = EducationType.None
        };

        var eb = new EducationBuilding(floors: 1, maxOccupancy: 100, area: new Area(2, 2))
        {
            Type = expected == EducationType.None ? EducationType.School : expected,
            Capacity = 50
        };

        // Если ожидается уровень None, оставляем StudyPlace пустым, иначе прикрепляем здание
        if (expected != EducationType.None)
            citizen.StudyPlace = eb;

        _service.UpdateEducation(citizen, new SimulationTime(0));

        if (expected == EducationType.None)
            Assert.AreEqual(EducationType.None, citizen.EducationLevel);
        else
            Assert.AreEqual(expected, citizen.EducationLevel);
    }
}
