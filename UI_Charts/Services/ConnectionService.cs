using System.Collections.ObjectModel;
using UICharts.Core.Models;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services
{
    public class ConnectionService : IConnectionService
    {
        private BlockViewModel? connectionStartBlock;

        public void Reset()
        {
            connectionStartBlock = null;
        }
        public void HandleConnectionClick(
            BlockViewModel block,
            DiagramModel? currentDiagram,
            ObservableCollection<ConnectionViewModel> connections)
        {
            if (currentDiagram == null)
                return;

            if (connectionStartBlock == null)
            {
                connectionStartBlock = block;
                return;
            }

            if (connectionStartBlock == block)
                return;

            var model = new ConnectionModel
            {
                FromBlockId = connectionStartBlock.Model.Id,
                ToBlockId = block.Model.Id
            };

            var vm = new ConnectionViewModel(
                model,
                connectionStartBlock,
                block);

            currentDiagram.Connections.Add(model);
            connections.Add(vm);

            connectionStartBlock = null;
        }
    }
}