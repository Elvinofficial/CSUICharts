using System.Collections.ObjectModel;
using System.Windows;
using UICharts.Core.Models;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services
{
    public class ConnectionService : IConnectionService
    {
        private BlockViewModel? connectionStartBlock;
        private readonly IConnectionRoutingService routingService;
        private int connectionStartPointIndex;
        public ConnectionService(
            IConnectionRoutingService routingService)
        {
            this.routingService = routingService;
        }

        public void Reset()
        {
            connectionStartBlock = null;
        }
        public void HandleConnectionClick(
                BlockViewModel block,
                Point mousePosition,
                DiagramModel? currentDiagram,
                ObservableCollection<ConnectionViewModel> connections)
        {
            if (currentDiagram == null)
                return;

            if (connectionStartBlock == null)
            {
                connectionStartBlock = block;
                connectionStartPointIndex = GetNearestConnectionPointIndex(block, mousePosition);
                return;
            }

            if (connectionStartBlock == block)
                return;

            if (connectionStartBlock.Model.Type == Core.Enums.BlockType.End)
                return;

            if (block.Type == Core.Enums.BlockType.Start)
                return;

            var targetPointIndex = GetNearestConnectionPointIndex(block, mousePosition);

            var model = new ConnectionModel
            {
                FromBlockId = connectionStartBlock.Model.Id,
                ToBlockId = block.Model.Id,

                FromPointIndex = connectionStartPointIndex,
                ToPointIndex = targetPointIndex
            };

            var vm = new ConnectionViewModel(
                routingService,
                model,
                connectionStartBlock,
                block);

            currentDiagram.Connections.Add(model);
            connections.Add(vm);

            connectionStartBlock = null;
        }
        public static Point GetBlockCenter(BlockViewModel block)
        {
            return new Point(
                block.X + block.Width / 2,
                block.Y + block.Height / 2);
        }

        

        public static double GetDistance(Point a, Point b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        private Point GetNearestConnectionPoint(BlockViewModel block, Point target)
        {
            return block.ConnectionPoints
                .OrderBy(point => GetDistance(point, target))
                .First();
        }
        public (Point Start, Point End) GetPreviewPoints(
            BlockViewModel fromBlock,
            BlockViewModel? targetBlock,
            Point mousePoint)
        {
            if (targetBlock == null)
            {
                var start = GetNearestConnectionPoint(fromBlock, mousePoint);
                return (start, mousePoint);
            }

            var targetCenter = GetBlockCenter(targetBlock);
            var fromCenter = GetBlockCenter(fromBlock);

            var startPoint = GetNearestConnectionPoint(fromBlock, targetCenter);
            var endPoint = GetNearestConnectionPoint(targetBlock, fromCenter);

            return (startPoint, endPoint);


        }
        private int GetNearestConnectionPointIndex(BlockViewModel block, Point target)
        {
            var points = block.ConnectionPoints.ToList();

            if (points.Count == 0)
                return 0;

            return points
                .Select((point, index) => new
                {
                    Index = index,
                    Distance = GetDistance(point, target)
                })
                .OrderBy(x => x.Distance)
                .First()
                .Index;
        }

    }
}