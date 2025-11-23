using Domain.Base;

namespace Domain.Buildings
{
    public class ResidentialBuilding : Building
    {
        public ResidentialBuilding(string name, string iconPath, int floors = 1, int width = 1, int height = 1)
            : base(floors, width, height)
        {
            Name = name;
            IconPath = iconPath;
        }

        public override Building Clone()
        {
            var clone = (ResidentialBuilding)this.MemberwiseClone();
            if (this.Position != null)
            {
                clone.Position = new Domain.Map.Position(this.Position.X, this.Position.Y);
            }
            return clone;
        }
    }
}
