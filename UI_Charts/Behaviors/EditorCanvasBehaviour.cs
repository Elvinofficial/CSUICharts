using System.Windows;
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
    }
}
