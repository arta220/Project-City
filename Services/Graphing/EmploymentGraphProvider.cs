// Services/Graphing/EmploymentByProfessionPieChartProvider.cs
using Domain.Citizens;
using Domain.Citizens.States;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using Services.CitizensSimulation;
using System;

namespace Services.Graphing
{
    public class EmploymentGraphProvider : IGraphDataProvider
    {
        private readonly CitizenSimulationService _simulationService;

        public string SystemName => "Найм работников";
        public string GraphTitle => "Занятость по профессиям";
        public string XAxisTitle => "";
        public string YAxisTitle => "";

        public EmploymentGraphProvider(CitizenSimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel
            {
                Title = GraphTitle,
                TitleFontSize = 16,
                TitleFontWeight = FontWeights.Bold,
                PlotAreaBorderThickness = new OxyThickness(0),
                Background = OxyColors.White,
                DefaultFontSize = 12
            };

            var pieSeries = new PieSeries
            {
                StrokeThickness = 1,
                Stroke = OxyColors.White,
                InsideLabelFormat = "{1}",
                OutsideLabelFormat = "{0}: {1}",
                TickDistance = 0.3,
                Diameter = 0.8,
                FontSize = 11,
                ExplodedDistance = 0
            };

            var dataPoints = GetProfessionData();

            foreach (var dataPoint in dataPoints)
            {
                pieSeries.Slices.Add(new PieSlice(dataPoint.Label, dataPoint.Value)
                {
                    Fill = dataPoint.Color
                });
            }

            plotModel.Series.Add(pieSeries);
            return plotModel;
        }

        private List<EmploymentPieDataPoint> GetProfessionData()
        {
            var citizens = _simulationService.Citizens.ToList();
            var dataPoints = new List<EmploymentPieDataPoint>();

            if (citizens.Count == 0)
                return dataPoints;

            // Получаем все профессии из enum
            var allProfessions = System.Enum.GetValues(typeof(CitizenProfession))
                .Cast<CitizenProfession>();

            var colorIndex = 0;
            var colorPalette = GetProfessionColors();

            foreach (var profession in allProfessions)
            {
                var count = citizens.Count(c => c.Profession == profession && c.WorkPlace != null);

                if (count > 0)
                {
                    var professionName = GetProfessionDisplayName(profession);
                    var color = colorPalette.ContainsKey(profession)
                        ? colorPalette[profession]
                        : GetDefaultColor(colorIndex++);

                    dataPoints.Add(new EmploymentPieDataPoint(
                        professionName,
                        count,
                        color
                    ));
                }
            }

            // Добавляем безработных
            var unemployed = citizens.Count(c => c.WorkPlace == null);
            if (unemployed > 0)
            {
                dataPoints.Add(new EmploymentPieDataPoint(
                    "Безработные",
                    unemployed,
                    OxyColors.DeepSkyBlue
                ));
            }

            return dataPoints.OrderByDescending(d => d.Value).ToList();
        }

        private Dictionary<CitizenProfession, OxyColor> GetProfessionColors()
        {
            return new Dictionary<CitizenProfession, OxyColor>
            {
                { CitizenProfession.Chef, OxyColors.Tomato }, // Tomato
                { CitizenProfession.Seller, OxyColors.MediumSeaGreen }, // Medium Sea Green
                { CitizenProfession.Doctor, OxyColors.DodgerBlue }, // Dodger Blue
                { CitizenProfession.UtilityWorker, OxyColors.Goldenrod }, // Goldenrod
            };
        }

        private OxyColor GetDefaultColor(int index)
        {
            var colors = new[]
            {
                OxyColors.MediumAquamarine, // Medium Aquamarine
                OxyColors.MediumPurple, // Medium Purple
                OxyColors.Chocolate,  // Chocolate
                OxyColors.RosyBrown, // Rosy Brown
                OxyColors.SeaGreen,   // Sea Green
            };
            return colors[index % colors.Length];
        }

        private string GetProfessionDisplayName(CitizenProfession profession)
        {
            return profession switch
            {
                CitizenProfession.Chef => "Повара",
                CitizenProfession.Seller => "Продавцы",
                CitizenProfession.Doctor => "Врачи",
                CitizenProfession.UtilityWorker => "Коммунальщики",
                _ => profession.ToString()
            };
        }
    }
}