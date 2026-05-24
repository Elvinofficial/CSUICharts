using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using System.Windows;
using UICharts.Core.Models;
using UICharts.Core.Enums;
using UICharts.Core.Figures;
using UICharts.Core.Factorys;
using UICharts.Core.Interfaces;
namespace UICharts.Desktop.ViewModels
{
    public class BlockViewModel : BindableBase
    {
        private readonly BlockModel model;
        public BlockViewModel(BlockModel model)
        {
            this.model = model;
        }
        public BlockModel Model => model;

        public Guid Id => model.Id;
        public BlockType Type
        {
            get => model.Type;
            set
            {
                if (model.Type != value)
                {
                    model.Type = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Width));
                    RaisePropertyChanged(nameof(Height));
                    RaisePropertyChanged(nameof(ConnectionPoints));
                    RaisePropertyChanged(nameof(LocalConnectionPoints));
                }
            }
        }

        private static readonly BlockFigureFactory figureFactory = new();

        private IBlockFigure Figure => figureFactory.GetFigure(Type);

        public ConnectionSide GetConnectionSide(Point point)
        {
            var left = X;
            var right = X + Width;
            var top = Y;
            var bottom = Y + Height;

            var distanceToLeft = Math.Abs(point.X - left);
            var distanceToRight = Math.Abs(point.X - right);
            var distanceToTop = Math.Abs(point.Y - top);
            var distanceToBottom = Math.Abs(point.Y - bottom);

            var min = new[]
            {
        (Side: ConnectionSide.Left, Distance: distanceToLeft),
        (Side: ConnectionSide.Right, Distance: distanceToRight),
        (Side: ConnectionSide.Top, Distance: distanceToTop),
        (Side: ConnectionSide.Bottom, Distance: distanceToBottom)
    }
            .OrderBy(x => x.Distance)
            .First();

            return min.Side;
        }

        private bool showConnectionPoints;

        public bool ShowConnectionPoints
        {
            get => showConnectionPoints;
            set => SetProperty(ref showConnectionPoints, value);
        }
        public IEnumerable<Point> LocalConnectionPoints =>
    Figure.ConnectionPoints.Select(point => new Point(
        Width * point.XRatio,
        Height * point.YRatio));

        public double X
        {
            get => model.X;
            set
            {
                if (model.X != value)
                {
                    model.X = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ConnectionPoints));
                }
            }
        }
        public double Y
        {
            get => model.Y;
            set
            {
                if (model.Y != value)
                {
                    model.Y = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ConnectionPoints));
                }
            }
        }

        public double Width
        {
            get => model.Width;
            set
            {
                if (model.Width != value)
                {
                    model.Width = value;

                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ConnectionPoints));
                    RaisePropertyChanged(nameof(LocalConnectionPoints));
                }
            }
        }

        public double Height
        {
            get => model.Height;
            set
            {
                if (model.Height != value)
                {
                    model.Height = value;

                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ConnectionPoints));
                    RaisePropertyChanged(nameof(LocalConnectionPoints));
                }
            }
        }

        public string Text
        {
            get => model.Text;
            set
            {
                if (model.Text != value)
                {
                    model.Text = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {     get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        private bool isEditing;
        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value);
        }

        public Point GetConnectionPointByIndex(int index)
        {
            var points = ConnectionPoints.ToList();

            if (points.Count == 0)
                return new Point(X + Width / 2, Y + Height / 2);

            if (index < 0 || index >= points.Count)
                index = 0;

            return points[index];
        }
        /// <summary>
        /// Возвращает абсолютные координаты точек подключения блока, вычисляемые на основе его типа и размеров
        /// </summary>
        public IEnumerable<Point> ConnectionPoints =>
            Figure.ConnectionPoints.Select(point => new Point(
                X + Width * point.XRatio,
                Y + Height * point.YRatio));
    }
}
