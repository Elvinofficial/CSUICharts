using System.IO;
using System.Text.Json;
using UICharts.Core.Interfaces;
using UICharts.Core.Models;

namespace UICharts.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public void SaveDiagram(DiagramModel diagram, string filePath)
        {
            var json = JsonSerializer.Serialize(diagram, options);

            File.WriteAllText(filePath, json);
        }

        public DiagramModel LoadDiagram(string filePath)
        {
            var json = File.ReadAllText(filePath);

            var diagram = JsonSerializer.Deserialize<DiagramModel>(json, options);

            if (diagram == null)
                throw new InvalidOperationException("Не удалось загрузить диаграмму из файла.");

            return diagram;
        }
    }
}