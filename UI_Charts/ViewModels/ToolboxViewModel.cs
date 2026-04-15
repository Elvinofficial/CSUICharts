using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class ToolboxViewModel : BindableBase
    {
        public ObservableCollection<FigureItemModel> Figures { get; } = new();

        private FigureItemModel selectedFigure;

        public FigureItemModel SelectedFigure
        {
            get => selectedFigure;
            set
            {
                if (SetProperty(ref selectedFigure, value))
                {
                    RaisePropertyChanged(nameof(SelectedFigureName));
                }
            }
        }

        public string SelectedFigureName => SelectedFigure?.Name ?? "Не выбрана";

        public ToolboxViewModel()
        {
            Figures.Add(new FigureItemModel { Name = "Процесс" });
            Figures.Add(new FigureItemModel { Name = "Решение" });
            Figures.Add(new FigureItemModel { Name = "Начало / конец" });
            Figures.Add(new FigureItemModel { Name = "Ввод / вывод" });

            SelectedFigure = Figures[0];
        }
    }
}
