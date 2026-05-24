using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Interaction
{
    public class ConnectionMouseEventArgs
    {
        public ConnectionViewModel Connection { get; }

        public Point MousePosition { get; }

        public ConnectionMouseEventArgs(
            ConnectionViewModel connection,
            Point mousePosition)
        {
            Connection = connection;
            MousePosition = mousePosition;
        }
    }
}
