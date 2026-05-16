using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using UICharts.Core.Enums;

namespace UICharts.Core.Models
{
    public class BlockModel 
    {
        public BlockType Type { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();

        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public string Text { get; set; }
    }
}
