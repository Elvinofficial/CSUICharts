using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UICharts.Desktop.Services
{
    public class PngExportService
    {
        public void ExportToPng(FrameworkElement element, string filePath)
        {
            if (element == null)
                return;
            {
                element.UpdateLayout();

                var width = element.ActualWidth;
                var height = element.ActualHeight;

                if (width <= 0 || height <= 0)
                    return;

                var renderBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

                renderBitmap.Render(element);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using var fileStream = new FileStream(filePath, FileMode.Create);
                encoder.Save(fileStream);
            }
        }
    }
}

