using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Models
{
    public class WorkspaceItemModel
    {
        public required string Name { get; set; }
        public required DiagramModel Diagram { get; set; }
    }
}