using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;
using UICharts.Desktop.Events;
using UICharts.Desktop.Services;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<WorkspaceItemModel> Workspaces { get; } = new();

        private readonly IProjectService projectService;
        public DelegateCommand SaveProjectCommand { get; }
        public DelegateCommand LoadProjectCommand { get; }
        public EditorViewModel Editor { get; }
        public DelegateCommand<object> ExportPngCommand { get; }
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

        private readonly IEventAggregator eventAggregator;

        public DelegateCommand ShowHelpCommand { get; }


        public MainWindowViewModel(
            IProjectService projectService,
            IPngExportService pngExportService,
            IEventAggregator eventAggregator,
            EditorViewModel editorViewModel,
            ToolboxViewModel toolboxViewModel)
        {
            CreateWorkspaceCommand = new DelegateCommand(OnCreateWorkspace);
            SaveProjectCommand = new DelegateCommand(SaveProject);
            LoadProjectCommand = new DelegateCommand(LoadProject);
            ExportPngCommand = new DelegateCommand<object>(ExportPng);
            ShowHelpCommand = new DelegateCommand(ShowHelp);
            this.eventAggregator = eventAggregator;
            Toolbox = toolboxViewModel;
            Editor = editorViewModel;
            this.projectService = projectService;
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

        private void ShowHelp()
        {
            //MessageBox.Show("Команда справки вызвалась");
            eventAggregator
                .GetEvent<ShowHelpRequestedEvent>()
                .Publish();
        }
        private void SaveProject()
        {
            if (SelectedWorkspace?.Diagram == null)
            {
                MessageBox.Show("Нет активной диаграммы для сохранения.");
                return;
            }

            eventAggregator
                .GetEvent<SaveProjectRequestedEvent>()
                .Publish(SelectedWorkspace.Diagram);
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


        /// <summary>
        /// Ищет дочерний элемент с типом T и именем childName. Подход рекурсивный
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        private T? FindChild<T>(DependencyObject parent, string childName)
             where T : FrameworkElement
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T frameworkElement &&
                    frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }

                var result = FindChild<T>(child, childName);

                if (result != null)
                    return result;
            }

            return null;
        }

        private void ExportPng(object parameter)
        {
            if (parameter is not Window window)
            {
                MessageBox.Show("Не удалось получить окно приложения.");
                return;
            }

            Editor.ClearSelection();

            var canvas = FindChild<FrameworkElement>(window, "EditorCanvas");

            if (canvas == null)
            {
                MessageBox.Show("Не удалось найти рабочее поле для экспорта.");
                return;
            }

            eventAggregator
                .GetEvent<ExportPngRequestedEvent>()
                .Publish(canvas);
        }

    }
}