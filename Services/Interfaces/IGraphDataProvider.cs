using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Services.Interfaces
{
    public interface IGraphDataProvider
    {
        string SystemName { get; }
        string GraphTitle { get; }
        string XAxisTitle { get; }
        string YAxisTitle { get; }
        PlotModel CreatePlotModel();
    }
}