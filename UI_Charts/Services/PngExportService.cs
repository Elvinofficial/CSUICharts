using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.Services
{
    public class PngExportService : IPngExportService
    {
        private const double PADDING = 30;
        public void ExportToPng(
     FrameworkElement element,
     Rect bounds,
     string filePath)
        {
            if (element == null)
                return;

            element.UpdateLayout();

            bounds = Rect.Intersect(
                bounds,
                new Rect(0, 0, element.ActualWidth, element.ActualHeight));

            if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var dpi = 96d;

            var renderTarget = new RenderTargetBitmap(
                (int)Math.Ceiling(bounds.Width),
                (int)Math.Ceiling(bounds.Height),
                dpi,
                dpi,
                PixelFormats.Pbgra32);

            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                var outputRect = new Rect(0, 0, bounds.Width, bounds.Height);

                context.DrawRectangle(
                    Brushes.White,
                    null,
                    outputRect);

                var brush = new VisualBrush(element)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = bounds,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = outputRect,
                    Stretch = Stretch.Fill
                };

                context.DrawRectangle(
                    brush,
                    null,
                    outputRect);
            }

            renderTarget.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using var stream = File.Create(filePath);
            encoder.Save(stream);
        }
    }
}

