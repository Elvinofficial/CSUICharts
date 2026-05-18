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
        public void ExportToPng(FrameworkElement element, string filePath)
        {
            if (element == null)
                return;
            
            element.UpdateLayout();

            var bounds = VisualTreeHelper.GetDescendantBounds(element);

            if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            bounds.Inflate(PADDING, PADDING);

            var width = (int)Math.Ceiling(bounds.Width);
            var height = (int)Math.Ceiling(bounds.Height);

            var renderBitmap = new RenderTargetBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32);

            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(0, 0, width, height));

                var brush = new VisualBrush(element)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = bounds,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = new Rect(0, 0, width, height),
                    Stretch = Stretch.Fill
                };

                context.DrawRectangle(
                    brush,
                    null,
                    new Rect(0,0, width, height));
            }

            renderBitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
            
        }
    }
}

