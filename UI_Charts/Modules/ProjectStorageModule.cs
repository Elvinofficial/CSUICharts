using Microsoft.Win32;
using System.Windows;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;
using UICharts.Desktop.Events;

namespace UICharts.Desktop.Modules
{
    public class ProjectStorageModule : IModule
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IProjectService projectService;

        public ProjectStorageModule(
            IEventAggregator eventAggregator,
            IProjectService projectService)
        {
            this.eventAggregator = eventAggregator;
            this.projectService = projectService;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            eventAggregator
                .GetEvent<SaveProjectRequestedEvent>()
                .Subscribe(diagram => SaveProject(diagram), keepSubscriberReferenceAlive: true);
        }

        private void SaveProject(DiagramModel diagram)
        {
            if (diagram == null)
            {
                MessageBox.Show("Нет активной диаграммы для сохранения.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Сохранить проект",
                Filter = "JSON проект (*.json)|*.json",
                FileName = "diagram.json",
                DefaultExt = ".json",
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
                return;


            string filename = dialog.SafeFileName.Replace(".json","");

            diagram.Name = filename;

            projectService.SaveDiagram(diagram, dialog.FileName);

            MessageBox.Show("Проект сохранён.");
        }
    }
}
