using System.Windows;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IBlockResizeService
    {
        void StartResize(BlockViewModel block, Point mousePosition);
        void ResizeTo(Point mousePosition);
        void EndResize();
    }
}