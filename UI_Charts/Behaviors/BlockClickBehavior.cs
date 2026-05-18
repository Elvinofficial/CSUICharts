using System.Windows;
using System.Windows.Input;
using UICharts.Desktop.Interaction;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Behaviors
{
    public static class BlockClickBehavior
    {
        public static readonly DependencyProperty InteractionCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InteractionCanvas",
                typeof(FrameworkElement),
                typeof(BlockClickBehavior),
                new PropertyMetadata(null));

        public static void SetInteractionCanvas(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(InteractionCanvasProperty, value);
        }

        public static FrameworkElement GetInteractionCanvas(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(InteractionCanvasProperty);
        }

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseDownCommand",
                typeof(ICommand),
                typeof(BlockClickBehavior),
                new PropertyMetadata(null, OnMouseDownCommandChanged));

        public static void SetMouseDownCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseDownCommandProperty, value);
        }

        public static ICommand GetMouseDownCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseDownCommandProperty);
        }

        private static void OnMouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                element.MouseLeftButtonDown += OnMouseLeftButtonDown;
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            if (element.DataContext is not BlockViewModel blockVm)
                return;

            var canvas = GetInteractionCanvas(element);
            if (canvas == null)
                return;

            var args = new BlockMouseEventArgs(
                blockVm,
                e.GetPosition(canvas),
                e.ClickCount,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                new Size (canvas.ActualWidth, canvas.ActualHeight));

            var command = GetMouseDownCommand(element);

            if (command?.CanExecute(args) == true)
            {
                command.Execute(args);
            }

            element.CaptureMouse();
            e.Handled = true;
        }
    }
}
