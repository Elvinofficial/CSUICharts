using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UICharts.Core.Models
{
    public class DiagramModel
    {
        public required string Name { get; set; }

        public List<BlockModel> Blocks { get; set; } = new();
        public ObservableCollection<ConnectionModel> Connections { get; set; } = new();

    }
}
