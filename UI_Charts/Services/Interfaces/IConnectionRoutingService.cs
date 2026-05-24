using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UICharts.Core.Enums;
using UICharts.Core.Models;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IConnectionRoutingService
    {
        IReadOnlyList<Point> BuildRoute(
     Point start,
     ConnectionSide startSide,
     Point end,
     ConnectionSide endSide,
     IEnumerable<RoutePointModel> bendPoints,
     IEnumerable<Rect> obstacles);
    }
}
