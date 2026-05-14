using System;
using System.Collections.Generic;
using System.Text;
using UICharts.Core.Enums;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;

namespace UICharts.Core.Figures
{
    public class DecisionFigure : IBlockFigure
    {
        public BlockType Type => BlockType.Decision;
        public double DefaultWidth => 120;
        public double DefaultHeight => 80;
        public IReadOnlyList<ConnectionPointModel> ConnectionPoints { get; } =
        [
            new(0.5, 0),
            new(1, 0.5),
            new(0.5, 1),
            new(0, 0.5)
        ];
    }
}
