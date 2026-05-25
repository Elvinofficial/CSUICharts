using Microsoft.Win32;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using UICharts.Desktop.Events;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Modules
{
    public class PngExportModule : IModule
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IPngExportService pngExportService;

        public PngExportModule(
            IEventAggregator eventAggregator,
            IPngExportService pngExportService)
        {
            this.eventAggregator = eventAggregator;
            this.pngExportService = pngExportService;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            eventAggregator
                .GetEvent<ExportPngRequestedEvent>()
                .Subscribe(canvas => ExportPng(canvas), keepSubscriberReferenceAlive: true);
        }

        private void ExportPng(FrameworkElement canvas)
        {
            if (canvas == null)
            {
                MessageBox.Show("Не удалось найти область для экспорта.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Экспортировать в PNG",
                Filter = "PNG изображение (*.png)|*.png",
                FileName = "diagram.png",
                DefaultExt = ".png",
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
                return;

            var bounds = CalculateDiagramBounds(canvas);

            pngExportService.ExportToPng(
                canvas,
                bounds,
                dialog.FileName);

            MessageBox.Show("Диаграмма экспортирована в PNG.");
        }
        private Rect CalculateDiagramBounds(FrameworkElement canvas)
        {
            if (canvas.DataContext is not EditorViewModel editor)
                return new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight);

            double left = double.MaxValue;
            double top = double.MaxValue;
            double right = double.MinValue;
            double bottom = double.MinValue;

            foreach (var block in editor.Blocks)
            {
                left = Math.Min(left, block.X);
                top = Math.Min(top, block.Y);

                right = Math.Max(right, block.X + block.Width);
                bottom = Math.Max(bottom, block.Y + block.Height);
            }

            foreach (var connection in editor.Connections)
            {
                foreach (var point in connection.RoutePoints)
                {
                    left = Math.Min(left, point.X);
                    top = Math.Min(top, point.Y);

                    right = Math.Max(right, point.X);
                    bottom = Math.Max(bottom, point.Y);
                }

                var label = connection.LabelPosition;

                left = Math.Min(left, label.X);
                top = Math.Min(top, label.Y);

                right = Math.Max(right, label.X + 120);
                bottom = Math.Max(bottom, label.Y + 40);
            }

            if (left == double.MaxValue)
            {
                return new Rect(
                    0,
                    0,
                    canvas.ActualWidth,
                    canvas.ActualHeight);
            }

            const double padding = 40;

            return new Rect(
                left - padding,
                top - padding,
                (right - left) + padding * 2,
                (bottom - top) + padding * 2);
        }
    }
}
