using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<WorkspaceItemModel> Workspaces { get; } = new();

        private readonly IProjectService projectService;
        public DelegateCommand SaveProjectCommand { get; }
        public DelegateCommand LoadProjectCommand { get; }
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

                }
            }
        }

        public DelegateCommand CreateWorkspaceCommand { get; }

        private int workspaceCounter = 0;

        public MainWindowViewModel(IProjectService projectService)
        {
            CreateWorkspaceCommand = new DelegateCommand(OnCreateWorkspace);
            SaveProjectCommand = new DelegateCommand(SaveProject);
            LoadProjectCommand = new DelegateCommand(LoadProject);
            Toolbox = new ToolboxViewModel();
            Editor = new EditorViewModel();
            this.projectService = projectService;
            Toolbox.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ToolboxViewModel.SelectedFigure))
                {
                    Editor.SelectedFigure = Toolbox.SelectedFigure;
                }
            };

            // OnCreateWorkspace();
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
        private void SaveProject()
        {
            if (SelectedWorkspace == null)
                return;

            var dialog = new SaveFileDialog
            {
                Title = "Сохранить диаграмму",
                Filter = "JSON файл (*.json)|*.json",
                FileName = $"{SelectedWorkspace.Name}.json",
                DefaultExt = ".json",
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
                return;


            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(dialog.FileName);

            SelectedWorkspace.Diagram.Name = fileNameWithoutExt;
            SelectedWorkspace.Name = fileNameWithoutExt;

            projectService.SaveDiagram(
                SelectedWorkspace.Diagram,
                dialog.FileName);
            MessageBox.Show("Диаграмма успешно сохранена.");
        }

        private void LoadProject()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Открыть диаграмму",
                Filter = "JSON файл (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() != true)
                return;

            var diagram = projectService.LoadDiagram(dialog.FileName);

            var workspace = new WorkspaceItemModel
            {
                Name = diagram.Name,
                Diagram = diagram
            };

            Workspaces.Add(workspace);
            SelectedWorkspace = workspace;
        }
    }
}