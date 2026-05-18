using System.Windows;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Interaction
{
    public class BlockMouseEventArgs(
        BlockViewModel block,
        Point mousePosition,
        int clickCount,
        bool isShiftPressed,
        Size canvasSize)
    {
        public BlockViewModel Block { get; } = block;
        public Point MousePosition { get; } = mousePosition;
        public int ClickCount { get; } = clickCount;
        public bool IsShiftPressed { get; } = isShiftPressed;

        public Size CanvasSize { get; } = canvasSize;
    }
}
