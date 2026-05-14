using System;
using System.Collections.Generic;
using System.Text;
using UICharts.Core.Enums;
using UICharts.Core.Models;

namespace UICharts.Core.Interfaces
{
    public interface IBlockFigure
    {
        BlockType Type { get; }
        double DefaultWidth { get; }
        double DefaultHeight { get; }
        IReadOnlyList<ConnectionPointModel> ConnectionPoints { get; }
    }
}
