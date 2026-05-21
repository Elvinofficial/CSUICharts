using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Models
{
    public class ConnectionModel
    {
        public Guid FromBlockId { get; set; }
        public Guid ToBlockId { get; set; }

        public int FromPointIndex { get; set; }
        public int ToPointIndex { get; set; }
    }
}
