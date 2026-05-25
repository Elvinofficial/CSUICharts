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

            if (canvasSize.Width <= 0 || canvasSize.Height <= 0)
                return;

            const double gridSize = 10;

            var dx = currentMouse.X - startMouse.X;
            var dy = currentMouse.Y - startMouse.Y;

            var newX = startX + dx;
            var newY = startY + dy;

            var maxX = Math.Max(0, canvasSize.Width - draggedBlock.Width);
            var maxY = Math.Max(0, canvasSize.Height - draggedBlock.Height);

            newX = Math.Max(0, Math.Min(maxX, newX));
            newY = Math.Max(0, Math.Min(maxY, newY));

            newX = Snap(newX, gridSize);
            newY = Snap(newY, gridSize);

            draggedBlock.X = newX;
            draggedBlock.Y = newY;
        }

        private double Snap(double value, double gridSize)
        {
            return Math.Round(value / gridSize) * gridSize;
        }

        public void EndDrag()
        {
            draggedBlock = null;
        }
    }
}