using Prism.Mvvm;
using System.Collections.ObjectModel;
using UICharts.Core.Models;
using Prism.Commands;

namespace UICharts.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// Коллекция доступных фигур для добавления на диаграмму. В реальном приложении эти данные могут загружаться из базы данных или конфигурационного файла.
        /// </summary>
        public ObservableCollection<WorkspaceItemModel> Workspaces { get; } = new();

        public EditorViewModel Editor { get; } = new();

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

        public MainWindowViewModel()
        {
            CreateWorkspaceCommand = new DelegateCommand(OnCreateWorkspace);
        }

        private void OnCreateWorkspace()
        {
            workspaceCounter++;
            var workspace = new WorkspaceItemModel
            {
                Name = $"Рабочее окно {workspaceCounter}",
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