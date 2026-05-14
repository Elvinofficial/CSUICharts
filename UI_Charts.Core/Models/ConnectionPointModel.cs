using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Models
{
    public class ConnectionPointModel(
        double xRatio, double yRatio)
    {
        public double XRatio { get; set; } = xRatio;
        public double YRatio { get; set; } = yRatio;
    }
}
