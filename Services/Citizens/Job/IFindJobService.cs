using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;

namespace Services.Interfaces
{
    public interface IFindJobService
    {
        IEnumerable<Building> FindJob(CitizenProfession profession);
    }
}
