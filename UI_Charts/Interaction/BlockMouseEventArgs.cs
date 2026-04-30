using System.Windows;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Interaction
{
    public class BlockMouseEventArgs
    {
        public BlockMouseEventArgs(
            BlockViewModel block,
            Point mousePosition,
            int clickCount,
            bool isShiftPressed)
        {
            Block = block;
            MousePosition = mousePosition;
            ClickCount = clickCount;
            IsShiftPressed = isShiftPressed;
        }

        public BlockViewModel Block { get; }
        public Point MousePosition { get; }
        public int ClickCount { get; }
        public bool IsShiftPressed { get; }
    }
}
