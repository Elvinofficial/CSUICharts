using System;
using System.Collections.Generic;
using System.Text;

namespace UICharts.Core.Enums
{
    public enum BlockType
    {
        Process,
        Decision,
        StartEnd,// уже не существует, но остался совместимости
        InputOutput,
        Start,
        End,
        Loop,
        Reference,
        Procedure,
    }
}
