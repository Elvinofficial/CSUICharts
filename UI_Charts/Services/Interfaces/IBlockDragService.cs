using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IBlockDragService
    {
        void StartDrag(BlockViewModel block, Point mousePosition);

        void DragTo(Point mousePosition, Size canvasSize);

        void EndDrag();
    }
}
