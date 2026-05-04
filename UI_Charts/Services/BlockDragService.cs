using System.Windows;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services
{
    public class BlockDragService // интерфейсы хотелось бы
    {
        private BlockViewModel? draggedBlock;
        private Point startMouse;
        private double startX;
        private double startY;

        public void StartDrag(BlockViewModel block, Point mousePosition)
        {
            draggedBlock = block;
            startMouse = mousePosition;
            startX = block.X;
            startY = block.Y;
        }

        public void DragTo(Point currentMouse)
        {
            if (draggedBlock == null)
                return;

            var dx = currentMouse.X - startMouse.X;
            var dy = currentMouse.Y - startMouse.Y;

            draggedBlock.X = startX + dx;
            draggedBlock.Y = startY + dy;
        }

        public void EndDrag()
        {
            draggedBlock = null;
        }
    }
}