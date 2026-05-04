using System;
using System.Windows;
using System.Windows.Media;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class ConnectionViewModel : BindableBase
    {
        private readonly ConnectionModel model;

        public ConnectionViewModel(
            ConnectionModel model,
            BlockViewModel from,
            BlockViewModel to)
        {
            this.model = model;

            From = from;
            To = to;

            From.PropertyChanged += (_, __) => Update();
            To.PropertyChanged += (_, __) => Update();
        }

        public BlockViewModel From { get; }
        public BlockViewModel To { get; }

        public ConnectionModel Model => model;

        public double X1 => GetEdgePoint(From, To).X;
        public double Y1 => GetEdgePoint(From, To).Y;

        public double X2 => GetEdgePoint(To, From).X;
        public double Y2 => GetEdgePoint(To, From).Y;

        private double UnitX => Math.Cos(AngleRadians);
        private double UnitY => Math.Sin(AngleRadians);

        private double ArrowLength => 14;
        private double ArrowWidth => 7;

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

        private double AngleRadians => Math.Atan2(Y2 - Y1, X2 - X1);

        private Point GetEdgePoint(BlockViewModel from, BlockViewModel to)
        {
            var fromCenter = new Point(
                from.X + from.Width / 2,
                from.Y + from.Height / 2);

            var toCenter = new Point(
                to.X + to.Width / 2,
                to.Y + to.Height / 2);

            double dx = toCenter.X - fromCenter.X;
            double dy = toCenter.Y - fromCenter.Y;

            if (dx == 0 && dy == 0)
                return fromCenter;

            double halfW = from.Width / 2;
            double halfH = from.Height / 2;

            double scaleX = dx == 0 ? double.PositiveInfinity : halfW / Math.Abs(dx);
            double scaleY = dy == 0 ? double.PositiveInfinity : halfH / Math.Abs(dy);

            double scale = Math.Min(scaleX, scaleY);

            return new Point(
                fromCenter.X + dx * scale,
                fromCenter.Y + dy * scale);
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
        }
    }
}