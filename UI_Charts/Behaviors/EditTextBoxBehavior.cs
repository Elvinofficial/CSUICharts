using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Behaviors
{
    public static class EditTextBoxBehavior
    {
        public static readonly DependencyProperty EndEditCommandProperty =
           DependencyProperty.RegisterAttached(
               "EndEditCommand",
               typeof(ICommand),
               typeof(EditTextBoxBehavior),
               new PropertyMetadata(null, OnEndEditCommandChanged));

        public static void SetEndEditCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EndEditCommandProperty, value);
        }

        public static ICommand GetEndEditCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EndEditCommandProperty);
        }

        private static void OnEndEditCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.KeyDown -= OnKeyDown;
                textBox.LostFocus -= OnLostFocus;

                textBox.KeyDown += OnKeyDown;
                textBox.LostFocus += OnLostFocus;
            }
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EndEdit(sender);
            e.Handled = true;
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            EndEdit(sender);
        }

        private static void EndEdit(object sender)
        {
            if (sender is not TextBox textBox)
                return;

            if (textBox.DataContext is not BlockViewModel blockVm)
                return;

            var command = GetEndEditCommand(textBox);

            if (command?.CanExecute(blockVm) == true)
            {
                command.Execute(blockVm);
            }
        }
    }
}
