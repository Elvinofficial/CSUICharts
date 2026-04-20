using System;
using System.Collections.Generic;
using System.Text;
using UICharts.Core.Enums;

namespace UICharts.Core.Models
{
    public class FigureItemModel
    {
        public required string Name { get; set; }
        public BlockType Type { get; set; }
    }
}
