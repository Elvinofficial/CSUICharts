using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UICharts.Desktop.Interaction;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Behaviors
{
    public static class ConnectionClickBehavior
    {
        public static readonly DependencyProperty InteractionCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InteractionCanvas",
                typeof(Canvas),
                typeof(ConnectionClickBehavior),
                new PropertyMetadata(null));

        public static void SetInteractionCanvas(DependencyObject obj, Canvas value) =>
            obj.SetValue(InteractionCanvasProperty, value);

        public static Canvas GetInteractionCanvas(DependencyObject obj) =>
            (Canvas)obj.GetValue(InteractionCanvasProperty);

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "DoubleClickCommand",
                typeof(ICommand),
                typeof(ConnectionClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(DoubleClickCommandProperty, value);

        public static ICommand GetDoubleClickCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(DoubleClickCommandProperty);

        private static void OnCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                element.MouseLeftButtonDown += OnMouseLeftButtonDown;
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
                return;

            if (sender is not FrameworkElement element)
                return;

            if (element.DataContext is not ConnectionViewModel connection)
                return;

            if (!connection.IsSelected)
                return;

            var canvas = GetInteractionCanvas(element);
            if (canvas == null)
                return;

            var args = new ConnectionMouseEventArgs(
                connection,
                e.GetPosition(canvas));

            var command = GetDoubleClickCommand(element);

            if (command?.CanExecute(args) == true)
                command.Execute(args);

            e.Handled = true;
        }
    }
}
