using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UICharts.Core.Models;

namespace UICharts.Desktop.Behaviors
{
    public static class BendPointDragBehavior
    {
        public static readonly DependencyProperty InteractionCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InteractionCanvas",
                typeof(Canvas),
                typeof(BendPointDragBehavior),
                new PropertyMetadata(null));

        public static void SetInteractionCanvas(DependencyObject obj, Canvas value) =>
            obj.SetValue(InteractionCanvasProperty, value);

        public static Canvas GetInteractionCanvas(DependencyObject obj) =>
            (Canvas)obj.GetValue(InteractionCanvasProperty);


        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseDownCommand",
                typeof(ICommand),
                typeof(BendPointDragBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetMouseDownCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(MouseDownCommandProperty, value);

        public static ICommand GetMouseDownCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(MouseDownCommandProperty);


        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseMoveCommand",
                typeof(ICommand),
                typeof(BendPointDragBehavior),
                new PropertyMetadata(null));

        public static void SetMouseMoveCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(MouseMoveCommandProperty, value);

        public static ICommand GetMouseMoveCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(MouseMoveCommandProperty);


        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof(ICommand),
                typeof(BendPointDragBehavior),
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

            if (element.DataContext is not RoutePointModel point)
                return;

            GetMouseDownCommand(element)?.Execute(point);

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
