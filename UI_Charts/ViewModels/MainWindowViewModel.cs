using System.Collections.ObjectModel;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<WorkspaceItemModel> Workspaces { get; } = new();

        public EditorViewModel Editor { get; }

        public ToolboxViewModel Toolbox { get; }

        private WorkspaceItemModel selectedWorkspace;
        public WorkspaceItemModel SelectedWorkspace
        {
            get => selectedWorkspace;
            set
            {
                if (SetProperty(ref selectedWorkspace, value))
                {
                    Editor.CurrentDiagram = value?.Diagram;

                    RaisePropertyChanged(nameof(SelectedWorkspaceName));
                    RaisePropertyChanged(nameof(CurrentBlockCount));
                    RaisePropertyChanged(nameof(CurrentConnectionCount));
                }
            }
        }

        public string SelectedWorkspaceName => SelectedWorkspace?.Name ?? "Нет активного окна";

        public int CurrentBlockCount => SelectedWorkspace?.Diagram?.Blocks?.Count ?? 0;

        public int CurrentConnectionCount => SelectedWorkspace?.Diagram?.Connections?.Count ?? 0;

        public DelegateCommand CreateWorkspaceCommand { get; }

        private int workspaceCounter = 0;

        public MainWindowViewModel(IProjectService a)
        {
            CreateWorkspaceCommand = new DelegateCommand(OnCreateWorkspace);

            Toolbox = new ToolboxViewModel();
            Editor = new EditorViewModel();

            Toolbox.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ToolboxViewModel.SelectedFigure))
                {
                    Editor.SelectedFigure = Toolbox.SelectedFigure;
                }
            };

            OnCreateWorkspace();
        }

        private void OnCreateWorkspace()
        {
            workspaceCounter++;
            var workspace = new WorkspaceItemModel
            {
                Name = $"Вкладка {workspaceCounter}",
                Diagram = new DiagramModel
                {
                    Name = $"Диаграмма {workspaceCounter}"
                }

            };
            Workspaces.Add(workspace);
            SelectedWorkspace = workspace;
        }
    }
}