using System.Windows;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface IPngExportService
    {
        void ExportToPng(FrameworkElement element, string filePath);
    }
}