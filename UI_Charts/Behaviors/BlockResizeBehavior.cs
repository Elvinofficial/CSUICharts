using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UICharts.Desktop.Interaction;

namespace UICharts.Desktop.Behaviors
{
    public static class BlockResizeBehavior
    {
        public static readonly DependencyProperty InteractionCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InteractionCanvas",
                typeof(Canvas),
                typeof(BlockResizeBehavior),
                new PropertyMetadata(null));

        public static void SetInteractionCanvas(DependencyObject obj, Canvas value) =>
            obj.SetValue(InteractionCanvasProperty, value);

        public static Canvas GetInteractionCanvas(DependencyObject obj) =>
            (Canvas)obj.GetValue(InteractionCanvasProperty);

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseDownCommand",
                typeof(ICommand),
                typeof(BlockResizeBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetMouseDownCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(MouseDownCommandProperty, value);

        public static ICommand GetMouseDownCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(MouseDownCommandProperty);

        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseMoveCommand",
                typeof(ICommand),
                typeof(BlockResizeBehavior),
                new PropertyMetadata(null));

        public static void SetMouseMoveCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(MouseMoveCommandProperty, value);

        public static ICommand GetMouseMoveCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(MouseMoveCommandProperty);

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof(ICommand),
                typeof(BlockResizeBehavior),
                new PropertyMetadata(null));

        public static void SetMouseUpCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(MouseUpCommandProperty, value);

        public static ICommand GetMouseUpCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(MouseUpCommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.PreviewMouseLeftButtonDown -= OnMouseDown;
                element.PreviewMouseMove -= OnMouseMove;
                element.PreviewMouseLeftButtonUp -= OnMouseUp;

                element.PreviewMouseLeftButtonDown += OnMouseDown;
                element.PreviewMouseMove += OnMouseMove;
                element.PreviewMouseLeftButtonUp += OnMouseUp;
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            var canvas = GetInteractionCanvas(element);
            if (canvas == null)
                return;

            if (element.DataContext is not ViewModels.BlockViewModel block)
                return;

            var command = GetMouseDownCommand(element);
            var args = new BlockMouseEventArgs(
                block,
                e.GetPosition(canvas),
                e.ClickCount,
                Keyboard.Modifiers.HasFlag(ModifierKeys.Shift),
                new Size (canvas.Width, canvas.Height));

            command?.Execute(args);

            command?.Execute(args);

            element.CaptureMouse();
            e.Handled = true;
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not FrameworkElement element || !element.IsMouseCaptured)
                return;

            var canvas = GetInteractionCanvas(element);
            if (canvas == null)
                return;

            GetMouseMoveCommand(element)?.Execute(e.GetPosition(canvas));
            e.Handled = true;
        }

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            GetMouseUpCommand(element)?.Execute(null);

            if (element.IsMouseCaptured)
                element.ReleaseMouseCapture();

            e.Handled = true;
        }
    }
}