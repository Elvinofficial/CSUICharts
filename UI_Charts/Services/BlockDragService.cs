using System.Windows;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;
namespace UICharts.Desktop.Services
{
    public class BlockDragService: IBlockDragService // интерфейсы хотелось бы
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

        public void DragTo(Point currentMouse, Size canvasSize)
        {
            if (draggedBlock == null)
                return;

            var dx = currentMouse.X - startMouse.X;
            var dy = currentMouse.Y - startMouse.Y;

            draggedBlock.X = startX + dx;
            draggedBlock.Y = startY + dy;

            draggedBlock.X = Math.Max(0, Math.Min(canvasSize.Width - draggedBlock.Width, draggedBlock.X));
            draggedBlock.Y = Math.Max(0, Math.Min(canvasSize.Height - draggedBlock.Height, draggedBlock.Y));
        }

        public void EndDrag()
        {
            draggedBlock = null;
        }
    }
}