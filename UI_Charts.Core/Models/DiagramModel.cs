using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Models
{
    public class DiagramModel
    {
        public string Name { get; set; }

        public List<BlockModel> Blocks { get; set; } = new();
        public List<ConnectionModel> Connections { get; set; } = new();
    }
}
