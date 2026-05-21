using System;
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

        private readonly IConnectionRoutingService routingService;

        public ConnectionViewModel(
            IConnectionRoutingService routingService,
            ConnectionModel model,
            BlockViewModel from,
            BlockViewModel to)
        {
            this.model = model;
            this.routingService = routingService;

            From = from;
            To = to;

            From.PropertyChanged += (_, __) => Update();
            To.PropertyChanged += (_, __) => Update();
            From.PropertyChanged += OnBlockChanged;
            To.PropertyChanged += OnBlockChanged;

        }

        public BlockViewModel From { get; }

        private BlockViewModel to;

        public BlockViewModel To
        {
            get => to;
            private set => SetProperty(ref to, value);
        }
        public Point StartHandle => new Point(X1, Y1);

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
        private IReadOnlyList<Point> Route =>
            routingService.BuildRoute(
                new Point(X1, Y1),
                StartSide,
                new Point(X2, Y2),
                EndSide);

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

        private void Update()
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