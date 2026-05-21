using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UICharts.Core.Enums;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.Services
{
    public class ConnectionRoutingService : IConnectionRoutingService
    {
        public IReadOnlyList<Point> BuildRoute(
    Point start,
    ConnectionSide startSide,
    Point end,
    ConnectionSide endSide)
        {
            const double offset = 25;

            var points = new List<Point>();

            var startOut = GetOffsetPoint(start, startSide, offset);
            var endIn = GetOffsetPoint(end, endSide, offset);

            points.Add(start);
            points.Add(startOut);

            var startHorizontal =
                startSide == ConnectionSide.Left ||
                startSide == ConnectionSide.Right;

            var endHorizontal =
                endSide == ConnectionSide.Left ||
                endSide == ConnectionSide.Right;

            if (startHorizontal && endHorizontal)
            {
                var middleX = (startOut.X + endIn.X) / 2;

                points.Add(new Point(middleX, startOut.Y));
                points.Add(new Point(middleX, endIn.Y));
            }
            else if (!startHorizontal && !endHorizontal)
            {
                var middleY = (startOut.Y + endIn.Y) / 2;

                points.Add(new Point(startOut.X, middleY));
                points.Add(new Point(endIn.X, middleY));
            }
            else if (startHorizontal && !endHorizontal)
            {
                points.Add(new Point(endIn.X, startOut.Y));
            }
            else
            {
                points.Add(new Point(startOut.X, endIn.Y));
            }

            points.Add(endIn);
            points.Add(end);

            return Simplify(points);
        }

        private Point GetOffsetPoint(Point point, ConnectionSide side, double offset)
        {
            return side switch
            {
                ConnectionSide.Left => new Point(point.X - offset, point.Y),
                ConnectionSide.Right => new Point(point.X + offset, point.Y),
                ConnectionSide.Top => new Point(point.X, point.Y - offset),
                ConnectionSide.Bottom => new Point(point.X, point.Y + offset),
                _ => point
            };
        }

        private IReadOnlyList<Point> Simplify(List<Point> points)
        {
            var result = new List<Point>();

            foreach (var point in points)
            {
                if (result.Count == 0)
                {
                    result.Add(point);
                    continue;
                }

                var last = result[^1];

                if (Math.Abs(last.X - point.X) < 0.01 &&
                    Math.Abs(last.Y - point.Y) < 0.01)
                {
                    continue;
                }

                result.Add(point);
            }

            for (int i = result.Count - 2; i > 0; i--)
            {
                var prev = result[i - 1];
                var current = result[i];
                var next = result[i + 1];

                var sameX =
                    Math.Abs(prev.X - current.X) < 0.01 &&
                    Math.Abs(current.X - next.X) < 0.01;

                var sameY =
                    Math.Abs(prev.Y - current.Y) < 0.01 &&
                    Math.Abs(current.Y - next.Y) < 0.01;

                if (sameX || sameY)
                {
                    result.RemoveAt(i);
                }
            }

            return result;
        }
    }
}
