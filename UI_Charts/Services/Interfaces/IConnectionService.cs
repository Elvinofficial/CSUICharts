using System.Collections.ObjectModel;
using UICharts.Core.Models;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IConnectionService
    {
        void HandleConnectionClick(BlockViewModel block, DiagramModel? currentDiagram, ObservableCollection<ConnectionViewModel> connections);
        void Reset();
    }
}