using System.Windows;
using UICharts.Desktop.ViewModels;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.Services
{
    public class BlockResizeService : IBlockResizeService
    {
        private BlockViewModel? currentBlock;
        private Point startMouse;
        private double startWidth;
        private double startHeight;

        private const double MinWidth = 60;
        private const double MinHeight = 40;

        public void StartResize(BlockViewModel block, Point mousePosition)
        {
            currentBlock = block;
            startMouse = mousePosition;
            startWidth = block.Width;
            startHeight = block.Height;
        }

        public void ResizeTo(Point mousePosition)
        {
            if (currentBlock == null)
                return;

            var dx = mousePosition.X - startMouse.X;
            var dy = mousePosition.Y - startMouse.Y;

            currentBlock.Width = Math.Max(MinWidth, startWidth + dx);
            currentBlock.Height = Math.Max(MinHeight, startHeight + dy);
        }

        public void EndResize()
        {
            currentBlock = null;
        }
    }
}