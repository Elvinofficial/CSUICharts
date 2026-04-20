using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Views
{
    /// <summary>
    /// Логика взаимодействия для EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {

        private BlockViewModel? draggedBlock;
        private Point dragStartMousePosition;
        private double dragStartBlockX;
        private double dragStartBlockY;
        private bool isDragging;
        public EditorView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as EditorViewModel;

            if (vm?.SelectedBlock != null && vm.SelectedBlock.IsEditing)
            {
                vm.EndEditBlockCommand.Execute(vm.SelectedBlock);
                return;
            }

            var pos = e.GetPosition(EditorCanvas);

            vm?.CanvasClickCommand.Execute(pos);
        }

        private void Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as EditorViewModel;

            if (sender is FrameworkElement element &&
                element.DataContext is BlockViewModel blockVm)
            {

                foreach (var block in vm.Blocks)
                {
                    block.IsEditing = false;
                }

                if (e.ClickCount == 2)
                {
                    vm?.BeginEditBlockCommand.Execute(blockVm);
                }
                else
                {
                    vm?.SelectBlockCommand.Execute(blockVm);

                    draggedBlock = blockVm;
                    dragStartMousePosition = e.GetPosition(EditorCanvas);
                    dragStartBlockX = blockVm.X;
                    dragStartBlockY = blockVm.Y;
                    isDragging = true;

                    element.CaptureMouse();
                }

                e.Handled = true;
            }
        }

        private void Block_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || draggedBlock == null)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPos = e.GetPosition(EditorCanvas);

            double deltaX = currentPos.X - dragStartMousePosition.X;
            double deltaY = currentPos.Y - dragStartMousePosition.Y;

            draggedBlock.X = dragStartBlockX + deltaX;
            draggedBlock.Y = dragStartBlockY + deltaY;
        }

        private void Block_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.ReleaseMouseCapture();
            }

            isDragging = false;
            draggedBlock = null;
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (sender is TextBox textBox &&
                textBox.DataContext is BlockViewModel blockvm &&
                DataContext is EditorViewModel vm)
            {
                vm.EndEditBlockCommand.Execute(blockvm);
                e.Handled = true;
            }
        }
        
        private void EditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox &&
                textBox.DataContext is BlockViewModel blockvm &&
                DataContext is EditorViewModel vm)
            {
                vm.EndEditBlockCommand.Execute(blockvm);
            }
        }
    }
}