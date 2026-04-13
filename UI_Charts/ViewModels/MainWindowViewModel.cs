using Prism.Mvvm;
using System.Collections.ObjectModel;
using UICharts.Core.Models;
namespace UICharts.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// Коллекция доступных фигур для добавления на диаграмму. В реальном приложении эти данные могут загружаться из базы данных или конфигурационного файла.
        /// </summary>
        public ObservableCollection<FigureItemModel> Figures { get; } = new();

        private FigureItemModel _selectedFigure;
        
        public FigureItemModel SelectedFigure
        {
            get => _selectedFigure;
            set
            {
                if (SetProperty(ref _selectedFigure, value))
                {
                    RaisePropertyChanged(nameof(SelectedFigureName));
                }
            }
        }

        public string SelectedFigureName => SelectedFigure?.Name ?? "Не выбрана";

        public ObservableCollection<WorkspaceItemModel> Workspaces { get; } = new();

        private WorkspaceItemModel _selectedWorkspace;
        public WorkspaceItemModel SelectedWorkspace
        {
            get => _selectedWorkspace;
            set
            {
                if (SetProperty(ref _selectedWorkspace, value))
                {
                    RaisePropertyChanged(nameof(SelectedWorkspaceName));
                    RaisePropertyChanged(nameof(CurrentBlockCount));
                    RaisePropertyChanged(nameof(CurrentConnectionCount));
                }
            }
        }

        public string SelectedWorkspaceName => SelectedWorkspace?.Name ?? "Нет активного окна";

        public int CurrentBlockCount => SelectedWorkspace?.Diagram?.Blocks?.Count ?? 0;

        public int CurrentConnectionCount => SelectedWorkspace?.Diagram?.Connections?.Count ?? 0;

        public MainWindowViewModel()
        {
            Figures.Add(new FigureItemModel { Name = "Процесс" });
            Figures.Add(new FigureItemModel { Name = "Решение" });
            Figures.Add(new FigureItemModel { Name = "Начало / конец" });
            Figures.Add(new FigureItemModel { Name = "Ввод / вывод" });

            SelectedFigure = Figures[0];

            Workspaces.Add(new WorkspaceItemModel
            {
                Name = "Рабочее окно 1",
                Diagram = new DiagramModel { Name = "Диаграмма 1" }
            });

            Workspaces.Add(new WorkspaceItemModel
            {
                Name = "Рабочее окно 2",
                Diagram = new DiagramModel { Name = "Диаграмма 2" }
            });

            Workspaces.Add(new WorkspaceItemModel
            {
                Name = "Рабочее окно 3",
                Diagram = new DiagramModel { Name = "Диаграмма 3" }
            });

            SelectedWorkspace = Workspaces[0];
        }
    }
}