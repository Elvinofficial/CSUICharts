namespace UICharts.Core.Models
{
    public class DiagramModel
    {
        public required string Name { get; set; } = "Новая диаграмма";

        public List<BlockModel> Blocks { get; set; } = new();
        public List<ConnectionModel> Connections { get; set; } = new();

    }
}
