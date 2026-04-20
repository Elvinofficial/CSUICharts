using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using UICharts.Core.Models;
using UICharts.Core.Enums;
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

        public BlockType Type
        {
            get => model.Type;
            set
            {
                if (model.Type != value)
                {
                    model.Type = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double X
        {
            get => model.X;
            set
            {
                if (model.X != value)
                {
                    model.X = value;
                    RaisePropertyChanged();
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
    }
}
