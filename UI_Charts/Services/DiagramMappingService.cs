using System.Collections.ObjectModel;
using UICharts.Core.Models;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services
{
    public class DiagramMappingService : IDiagramMappingService
    {
        public void Map(
            DiagramModel? diagram,
            ObservableCollection<BlockViewModel> blocks,
            ObservableCollection<ConnectionViewModel> connections)
        {
            blocks.Clear();
            connections.Clear();

            if (diagram == null)
                return;

            var map = new Dictionary<Guid, BlockViewModel>();

            foreach (var blockModel in diagram.Blocks)
            {
                var blockVm = new BlockViewModel(blockModel);
                blocks.Add(blockVm);

                map[blockModel.Id] = blockVm;
            }

            foreach (var connectionModel in diagram.Connections)
            {
                if (!map.TryGetValue(connectionModel.FromBlockId, out var fromVm))
                    continue;

                if (!map.TryGetValue(connectionModel.ToBlockId, out var toVm))
                    continue;

                var connectionVm = new ConnectionViewModel(
                    connectionModel,
                    fromVm,
                    toVm);

                connections.Add(connectionVm);
            }
        }
    }
}