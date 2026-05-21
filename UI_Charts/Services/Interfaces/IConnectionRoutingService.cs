using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UICharts.Core.Enums;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IConnectionRoutingService
    {
        IReadOnlyList<Point> BuildRoute(
            Point start,
            ConnectionSide startSide,
            Point end,
            ConnectionSide endSide);
    }
}
