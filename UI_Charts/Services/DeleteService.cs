using System.Collections.ObjectModel;
using UICharts.Core.Models;
using UICharts.Desktop.ViewModels;
using UICharts.Desktop.Services.Interfaces;

public class DeleteService : IDeleteService
{
    public void DeleteBlock(
        DiagramModel diagram,
        ObservableCollection<BlockViewModel> blocks,
        ObservableCollection<ConnectionViewModel> connections,
        BlockViewModel block)
    {
        var relatedConnectionVms = connections
            .Where(c => c.From == block || c.To == block)
            .ToList();

        foreach (var connectionVm in relatedConnectionVms)
        {
            connections.Remove(connectionVm);
            diagram.Connections.Remove(connectionVm.Model);
        }

        blocks.Remove(block);
        diagram.Blocks.Remove(block.Model);
    }

    public void DeleteConnection(
        DiagramModel diagram,
        ObservableCollection<ConnectionViewModel> connections,
        ConnectionViewModel connection)
    {
        connections.Remove(connection);
        diagram.Connections.Remove(connection.Model);
    }
}