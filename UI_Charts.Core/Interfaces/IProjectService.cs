using UICharts.Core.Models;

namespace UICharts.Core.Interfaces
{
    public interface IProjectService
    {
        void SaveDiagram(DiagramModel diagram, string filePath);

        DiagramModel LoadDiagram(string filePath);
    }
}