using Domain.Citizens;

namespace Services.Interfaces
{
    public interface IEducationService
    {
        //bool TryEnroll(Citizen citizen, School school); Школ ещё нет, потом решить вопрос
        void UpdateEducation(Citizen citizen);
    }
}
