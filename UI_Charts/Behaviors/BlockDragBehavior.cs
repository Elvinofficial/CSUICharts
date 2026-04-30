using System.Windows;
using System.Windows.Input;

namespace UICharts.Desktop.Behaviors
{
    public static class BlockDragBehavior
    {
        public static readonly DependencyProperty InteractionCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InteractionCanvas",
                typeof(FrameworkElement),
                typeof(BlockDragBehavior),
                new PropertyMetadata(null));

        public static void SetInteractionCanvas(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(InteractionCanvasProperty, value);
        }

        public static FrameworkElement GetInteractionCanvas(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(InteractionCanvasProperty);
        }

        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseMoveCommand",
                typeof(ICommand),
                typeof(BlockDragBehavior),
                new PropertyMetadata(null, OnCommandsChanged));

        public static void SetMouseMoveCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseMoveCommandProperty, value);
        }

        public static ICommand GetMouseMoveCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseMoveCommandProperty);
        }

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof(ICommand),
                typeof(BlockDragBehavior),
                new PropertyMetadata(null, OnCommandsChanged));

        public static void SetMouseUpCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseUpCommandProperty, value);
        }

        public static ICommand GetMouseUpCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseUpCommandProperty);
        }

        private static void OnCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseMove -= OnMouseMove;
                element.MouseLeftButtonUp -= OnMouseLeftButtonUp;

                element.MouseMove += OnMouseMove;
                element.MouseLeftButtonUp += OnMouseLeftButtonUp;
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var canvas = GetInteractionCanvas(element);
            if (canvas == null)
                return;

            var point = e.GetPosition(canvas);
            var command = GetMouseMoveCommand(element);

            if (command?.CanExecute(point) == true)
            {
                command.Execute(point);
            }
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            var command = GetMouseUpCommand(element);

            if (command?.CanExecute(null) == true)
            {
                command.Execute(null);
            }

            element.ReleaseMouseCapture();
            e.Handled = true;
        }
    }
}