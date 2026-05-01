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
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;

            double halfW = from.Width / 2;
            double halfH = from.Height / 2;

            if (Math.Abs(dx) > Math.Abs(dy))
            {

                return dx > 0
                    ? new Point(from.X + halfW, from.Y + halfH)
                    : new Point(from.X, from.Y + halfH);
            }
            else
            {

                return dy > 0
                    ? new Point(from.X + halfW, from.Y + from.Height)
                    : new Point(from.X + halfW, from.Y);
            }
        }

        private void Update()
        {
            RaisePropertyChanged(nameof(X1));
            RaisePropertyChanged(nameof(Y1));
            RaisePropertyChanged(nameof(X2));
            RaisePropertyChanged(nameof(Y2));
            RaisePropertyChanged(nameof(ArrowPoints));
        }
    }
}