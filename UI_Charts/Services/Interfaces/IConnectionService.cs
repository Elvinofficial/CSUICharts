using System.Collections.ObjectModel;
using System.Windows;
using UICharts.Core.Models;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IConnectionService
    {
        void HandleConnectionClick(BlockViewModel block,
                Point mousePosition,
                DiagramModel? currentDiagram,
                ObservableCollection<BlockViewModel> blocks,
                ObservableCollection<ConnectionViewModel> connections);
        void Reset();
        (Point Start, Point End) GetPreviewPoints(
            BlockViewModel fromBlock,
            BlockViewModel? targetBlock,
            Point mousePoint);

        int GetNearestConnectionPointIndex(BlockViewModel block, Point target);
    }
}