using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using UICharts.Core.Enums;
using UICharts.Core.Models;
using UICharts.Desktop.Services;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.ViewModels
{
    public class ConnectionViewModel : BindableBase
    {
        private readonly ConnectionModel model;
        private readonly ObservableCollection<BlockViewModel> blocks;

        private readonly IConnectionRoutingService routingService;

        public ConnectionViewModel(
    IConnectionRoutingService routingService,
    ConnectionModel model,
    BlockViewModel from,
    BlockViewModel to,
    ObservableCollection<BlockViewModel> blocks)
        {
            this.model = model;
            this.routingService = routingService;
            this.blocks = blocks;

            BendPoints = new ObservableCollection<RoutePointModel>(Model.BendPoints);

            From = from;
            To = to;

            From.PropertyChanged += OnBlockChanged;
            To.PropertyChanged += OnBlockChanged;
        }

        private BlockViewModel from;

        public BlockViewModel From
        {
            get => from;
            private set => SetProperty(ref from, value);
        }

        private BlockViewModel to;
        public string Text
        {
            get => Model.Text;
            set
            {
                if (Model.Text != value)
                {
                    Model.Text = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Point LabelPosition
        {
            get
            {
                var route = RoutePoints;

                if (route.Count == 0)
                    return new Point(0, 0);

                var middle = route[route.Count / 2];

                return new Point(middle.X, middle.Y);
            }
        }
        public ObservableCollection<RoutePointModel> BendPoints { get; }

        public BlockViewModel To
        {
            get => to;
            private set => SetProperty(ref to, value);
        }
        public Point StartHandle
        {
            get
            {
                const double handleOffset = 18;

                return new Point(
                    X1 + UnitX * handleOffset - 6,
                    Y1 + UnitY * handleOffset - 6);
            }
        }

        public Point EndHandle
        {
            get
            {
                const double handleOffset = 18;

                return new Point(
                    X2 - UnitX * handleOffset - 6,
                    Y2 - UnitY * handleOffset - 6);
            }
        }

        private ConnectionSide StartSide =>
            From.GetConnectionSide(new Point(X1, Y1));

        private ConnectionSide EndSide =>
            To.GetConnectionSide(new Point(X2, Y2));


        public ConnectionModel Model => model;
        private IReadOnlyList<Point> Route
        {
            get
            {
                const double obstaclePadding = 40;

                var obstacles = blocks
                    .Select(block => new Rect(
                        block.X - obstaclePadding,
                        block.Y - obstaclePadding,
                        block.Width + obstaclePadding * 2,
                        block.Height + obstaclePadding * 2));

                return routingService.BuildRoute(
                    new Point(X1, Y1),
                    StartSide,
                    new Point(X2, Y2),
                    EndSide,
                    Model.BendPoints,
                    obstacles);
            }
        }

        public double X1 => From.GetConnectionPointByIndex(Model.FromPointIndex).X;
        public double Y1 => From.GetConnectionPointByIndex(Model.FromPointIndex).Y;

        public double X2 => To.GetConnectionPointByIndex(Model.ToPointIndex).X;
        public double Y2 => To.GetConnectionPointByIndex(Model.ToPointIndex).Y;

        private double UnitX => Math.Cos(AngleRadians);
        private double UnitY => Math.Sin(AngleRadians);

        private double ArrowLength => 14;
        private double ArrowWidth => 7;

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
        public Point ArrowTip => new Point(X2, Y2);

        public Point ArrowBase1 => new Point(
            X2 - UnitX * ArrowLength - UnitY * ArrowWidth,
            Y2 - UnitY * ArrowLength + UnitX * ArrowWidth);

        public Point ArrowBase2 => new Point(
            X2 - UnitX * ArrowLength + UnitY * ArrowWidth,
            Y2 - UnitY * ArrowLength - UnitX * ArrowWidth);

        public PointCollection ArrowPoints => new PointCollection
        {
            ArrowTip,
            ArrowBase1,
            ArrowBase2
        };

        public PointCollection RoutePoints =>
            new PointCollection(Route);

        private double AngleRadians
        {
            get
            {
                var route = Route;

                if (route.Count < 2)
                    return 0;

                var beforeTip = route[^2];
                var tip = route[^1];

                return Math.Atan2(
                    tip.Y - beforeTip.Y,
                    tip.X - beforeTip.X);
            }
        }

        private static Point GetNeatestConnectionPoint(
            BlockViewModel from,
            BlockViewModel to)
        {
            var targetCentter = new Point(
                to.X + to.Width / 2,
                to.Y + to.Height / 2);

            return from.ConnectionPoints
                .OrderBy(point => ConnectionService.GetDistance(point, targetCentter))
                .First();
        }

        public void ChangeFrom(BlockViewModel newFrom)
        {
            From.PropertyChanged -= OnBlockChanged;

            From = newFrom;
            Model.FromBlockId = newFrom.Model.Id;

            From.PropertyChanged += OnBlockChanged;

            Update();
        }

        private double GetDistance(Point a, Point b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private double GetDistanceToSegment(Point point, Point a, Point b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;

            if (Math.Abs(dx) < 0.001 && Math.Abs(dy) < 0.001)
                return GetDistance(point, a);

            var t =
                ((point.X - a.X) * dx + (point.Y - a.Y) * dy) /
                (dx * dx + dy * dy);

            t = Math.Max(0, Math.Min(1, t));

            var projection = new Point(
                a.X + t * dx,
                a.Y + t * dy);

            return GetDistance(point, projection);
        }

        private int FindNearestSegmentIndex(List<Point> route, Point point)
        {
            var bestIndex = 0;
            var bestDistance = double.MaxValue;

            for (int i = 0; i < route.Count - 1; i++)
            {
                var distance = GetDistanceToSegment(
                    point,
                    route[i],
                    route[i + 1]);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        public void InsertBendPoint(Point point)
        {
            var route = Route.ToList();

            if (route.Count < 2)
            {
                AddBendPoint(point);
                return;
            }

            var nearestSegmentIndex = FindNearestSegmentIndex(route, point);

            var bendPoint = new RoutePointModel
            {
                X = point.X,
                Y = point.Y
            };

            var insertIndex = Math.Max(0, nearestSegmentIndex - 1);

            Model.BendPoints.Insert(
                Math.Min(insertIndex, Model.BendPoints.Count),
                bendPoint);

            BendPoints.Insert(
                Math.Min(insertIndex, BendPoints.Count),
                bendPoint);

            Update();
        }

        public void AddBendPoint(Point point)
        {
            var bendPoint = new RoutePointModel
            {
                X = point.X,
                Y = point.Y
            };

            Model.BendPoints.Add(bendPoint);
            BendPoints.Add(bendPoint);

            Update();
        }

        public void Update()
        {
            RaisePropertyChanged(nameof(X1));
            RaisePropertyChanged(nameof(Y1));
            RaisePropertyChanged(nameof(X2));
            RaisePropertyChanged(nameof(Y2));
            
            RaisePropertyChanged(nameof(ArrowTip));
            RaisePropertyChanged(nameof(ArrowBase1));
            RaisePropertyChanged(nameof(ArrowBase2));
            RaisePropertyChanged(nameof(ArrowPoints));

            RaisePropertyChanged(nameof(RoutePoints));

            RaisePropertyChanged(nameof(StartHandle));
            RaisePropertyChanged(nameof(EndHandle));

            RaisePropertyChanged(nameof(LabelPosition));
            RaisePropertyChanged(nameof(Text));
        }

        public void ChangeTo(BlockViewModel newTo)
        {
            To.PropertyChanged -= OnBlockChanged;

            To = newTo;

            Model.ToBlockId = newTo.Id;

            To.PropertyChanged += OnBlockChanged;

            Update();
        }

        private void OnBlockChanged(object? sender, PropertyChangedEventArgs e)
        {
            Update();
        }
    }
}