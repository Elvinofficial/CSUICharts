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
                }
            }
        }

        private static readonly BlockFigureFactory figureFactory = new();

        private IBlockFigure Figure => figureFactory.GetFigure(Type);

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


        /// <summary>
        /// Возвращает абсолютные координаты точек подключения блока, вычисляемые на основе его типа и размеров
        /// </summary>
        public IEnumerable<Point> ConnectionPoints =>
            Figure.ConnectionPoints.Select(point => new Point(
                X + Width * point.XRatio,
                Y + Height * point.YRatio));
    }
}
