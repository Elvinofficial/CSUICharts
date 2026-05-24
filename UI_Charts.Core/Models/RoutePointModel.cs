using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace UICharts.Core.Models
{
    public class RoutePointModel : INotifyPropertyChanged
    {
        private double x;
        public double X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged();
                }
            }
        }

        private double y;
        public double Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    y = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
