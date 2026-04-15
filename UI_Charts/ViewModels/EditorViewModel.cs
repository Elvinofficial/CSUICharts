using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class EditorViewModel : BindableBase
    {
        private DiagramModel? currentDiagram;

        public DiagramModel? CurrentDiagram
        {
            get => currentDiagram;
            set => SetProperty(ref currentDiagram, value);
        }
    }
}
