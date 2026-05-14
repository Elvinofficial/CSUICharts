using System.Collections.ObjectModel;
using UICharts.Core.Models;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IDiagramMappingService
    {
        void Map(DiagramModel? diagram, ObservableCollection<BlockViewModel> blocks, ObservableCollection<ConnectionViewModel> connections);
    }
}