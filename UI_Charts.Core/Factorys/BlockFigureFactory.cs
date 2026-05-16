using System;
using System.Collections.Generic;
using System.Text;
using UICharts.Core.Enums;
using UICharts.Core.Figures;
using UICharts.Core.Interfaces;

namespace UICharts.Core.Factorys
{
    public class BlockFigureFactory
    {
        public IBlockFigure GetFigure(BlockType type)
        {
            return type switch
            {
                BlockType.Decision => new DecisionFigure(),
                BlockType.InputOutput => new InputOutputFigure(),
                BlockType.StartEnd => new StartEndFigure(),
                BlockType.Start => new StartFigure(),
                BlockType.End => new EndFigure(),
                BlockType.Reference => new ReferenceFigure(),
                BlockType.Loop => new LoopFigure(),
                BlockType.Procedure => new ProcedureFigure(),
                _ => new ProcessFigure(),
            };
        }
    }
}
