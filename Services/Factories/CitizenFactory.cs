using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.Factories
{
    public class CitizenFactory
    {
        private readonly INavigationProfile _profile;

        public CitizenFactory(INavigationProfile profile)
        {
            _profile = profile;
        }

        public Citizen CreateCitizen(Position pos, float speed, CitizenProfession profession, int age = 30, CitizenState state = CitizenState.Idle)
        {
            var citizen = new Citizen(new Area(1, 1), speed)
            {
                Position = pos,
                Profession = profession,
                Age = age,
                State = state,
                NavigationProfile = _profile,
                Money = 0
            };
            return citizen;
        }
    }


}
