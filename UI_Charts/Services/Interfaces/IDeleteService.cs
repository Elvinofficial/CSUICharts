using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UICharts.Core.Models;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IDeleteService
    {
        void DeleteBlock(
            DiagramModel diagram,
            ObservableCollection<BlockViewModel> blocks,
            ObservableCollection<ConnectionViewModel> connections,
            BlockViewModel block);

        void DeleteConnection(
            DiagramModel diagram,
            ObservableCollection<ConnectionViewModel> connections,
            ConnectionViewModel connection);
    }
}