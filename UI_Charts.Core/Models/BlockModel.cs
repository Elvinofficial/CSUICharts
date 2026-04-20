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
        public double X { get; set; }
        public double Y { get; set; }

        public string Text { get; set; }
    }
}
