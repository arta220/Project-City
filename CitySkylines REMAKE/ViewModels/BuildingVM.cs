using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;
using Domain.Enums;
using Services;

// Smirnov
namespace CitySimulatorWPF.ViewModels
{
    public partial class BuildingVM : ObjectVM
    {
        private readonly Building _building;
        private readonly Simulation _simulation;

        [ObservableProperty]
        private bool _isBlinkingRed;

        public BuildingVM(Building building, Simulation simulation, string name, string iconPath)
            : base(building, name, iconPath)
        {
            _building = building;
            _simulation = simulation;
        }

        public void UpdateVisualState()
        {
            IsBlinkingRed = _building.HasBrokenUtilities;
        }

        public Dictionary<UtilityType, int> GetBrokenUtilities()
        {
            return _simulation.GetBrokenUtilities(_building);
        }

        public void FixUtility(UtilityType utilityType)
        {
            _simulation.FixBuildingUtility(_building, utilityType);
            UpdateVisualState();
        }
    }
}