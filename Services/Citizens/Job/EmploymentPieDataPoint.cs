// Services/Graphing/EmploymentPieDataPoint.cs
using OxyPlot;

namespace Services.Graphing
{
    public class EmploymentPieDataPoint
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public OxyColor Color { get; set; }

        public EmploymentPieDataPoint(string label, double value, OxyColor color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }
}