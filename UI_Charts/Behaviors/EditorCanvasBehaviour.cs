using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UICharts.Desktop.Behaviors
{
    public static class EditorCanvasBehavior
    {
        public static readonly DependencyProperty CanvasClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "CanvasClickCommand",
                typeof(ICommand),
                typeof(EditorCanvasBehavior),
                new PropertyMetadata(null, OnCanvasClickCommandChanged));

        public static void SetCanvasClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CanvasClickCommandProperty, value);
        }

        public static ICommand GetCanvasClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CanvasClickCommandProperty);
        }

        private static void OnCanvasClickCommandChanged(
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
            if (sender is not FrameworkElement element)
                return;

            var command = GetCanvasClickCommand(element);
            var point = e.GetPosition(element);

            if (command?.CanExecute(point) == true)
            {
                command.Execute(point);
            }
        }
        public static readonly DependencyProperty MouseMoveCommandProperty =
    DependencyProperty.RegisterAttached(
        "MouseMoveCommand",
        typeof(ICommand),
        typeof(EditorCanvasBehavior),
        new PropertyMetadata(null, OnMouseMoveCommandChanged));

        public static void SetMouseMoveCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseMoveCommandProperty, value);
        }

        public static ICommand GetMouseMoveCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseMoveCommandProperty);
        }

        private static void OnMouseMoveCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseMove -= OnMouseMove;
                element.MouseMove += OnMouseMove;
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not Canvas canvas)
                return;

            var command = GetMouseMoveCommand(canvas);

            if (command == null)
                return;

            var point = e.GetPosition(canvas);

            if (command.CanExecute(point))
                command.Execute(point);
        }

        public static readonly DependencyProperty MouseWheelCommandProperty =
    DependencyProperty.RegisterAttached(
        "MouseWheelCommand",
        typeof(ICommand),
        typeof(EditorCanvasBehavior),
        new PropertyMetadata(null, OnMouseWheelCommandChanged));

        public static void SetMouseWheelCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MouseWheelCommandProperty, value);
        }

        public static ICommand GetMouseWheelCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MouseWheelCommandProperty);
        }

        private static void OnMouseWheelCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseWheel -= OnMouseWheel;
                element.MouseWheel += OnMouseWheel;
            }
        }

        private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var command = GetMouseWheelCommand((DependencyObject)sender);

            if (command == null)
                return;

            if (command.CanExecute(e.Delta))
                command.Execute(e.Delta);

            e.Handled = true;
        }

        public static readonly DependencyProperty PanMouseDownCommandProperty =
    DependencyProperty.RegisterAttached(
        "PanMouseDownCommand",
        typeof(ICommand),
        typeof(EditorCanvasBehavior),
        new PropertyMetadata(null, OnPanCommandChanged));

        public static void SetPanMouseDownCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(PanMouseDownCommandProperty, value);

        public static ICommand GetPanMouseDownCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(PanMouseDownCommandProperty);


        public static readonly DependencyProperty PanMouseMoveCommandProperty =
            DependencyProperty.RegisterAttached(
                "PanMouseMoveCommand",
                typeof(ICommand),
                typeof(EditorCanvasBehavior),
                new PropertyMetadata(null));

        public static void SetPanMouseMoveCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(PanMouseMoveCommandProperty, value);

        public static ICommand GetPanMouseMoveCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(PanMouseMoveCommandProperty);


        public static readonly DependencyProperty PanMouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "PanMouseUpCommand",
                typeof(ICommand),
                typeof(EditorCanvasBehavior),
                new PropertyMetadata(null));

        public static void SetPanMouseUpCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(PanMouseUpCommandProperty, value);

        public static ICommand GetPanMouseUpCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(PanMouseUpCommandProperty);
        private static void OnPanCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseDown -= OnPanMouseDown;
                element.MouseMove -= OnPanMouseMove;
                element.MouseUp -= OnPanMouseUp;

                element.MouseDown += OnPanMouseDown;
                element.MouseMove += OnPanMouseMove;
                element.MouseUp += OnPanMouseUp;
            }
        }

        private static void OnPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
                return;

            if (sender is not FrameworkElement element)
                return;

            var command = GetPanMouseDownCommand(element);
            var point = e.GetPosition(element);

            if (command?.CanExecute(point) == true)
                command.Execute(point);

            element.CaptureMouse();
            e.Handled = true;
        }

        private static void OnPanMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not FrameworkElement element || !element.IsMouseCaptured)
                return;

            if (e.MiddleButton != MouseButtonState.Pressed)
                return;

            var command = GetPanMouseMoveCommand(element);
            var point = e.GetPosition(element);

            if (command?.CanExecute(point) == true)
                command.Execute(point);

            e.Handled = true;
        }

        private static void OnPanMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
                return;

            if (sender is not FrameworkElement element)
                return;

            var command = GetPanMouseUpCommand(element);

            if (command?.CanExecute(null) == true)
                command.Execute(null);

            if (element.IsMouseCaptured)
                element.ReleaseMouseCapture();

            e.Handled = true;
        }
    }
}
