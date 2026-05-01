using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Models
{
    public class ConnectionModel
    {
        public Guid FromBlockId { get; set; }
        public Guid ToBlockId { get; set; }
    }
}
