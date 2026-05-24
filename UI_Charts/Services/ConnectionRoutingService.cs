using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using UICharts.Core.Enums;
using UICharts.Core.Models;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.Services
{
    public class ConnectionRoutingService : IConnectionRoutingService
    {
        public IReadOnlyList<Point> BuildRoute(
    Point start,
    ConnectionSide startSide,
    Point end,
    ConnectionSide endSide,
    IEnumerable<RoutePointModel> bendPoints,
    IEnumerable<Rect> obstacles)
        {
            var bends = bendPoints
                .Select(p => new Point(p.X, p.Y))
                .ToList();

            if (bends.Count > 0)
                return BuildManualRoute(start, startSide, end, endSide, bends);

            var candidates = BuildCandidateRoutes(start, startSide, end, endSide);

            var best = candidates
                .Where(route => !IntersectsObstacles(route, obstacles))
                .OrderBy(GetRouteLength)
                .FirstOrDefault();

            return best ?? candidates.OrderBy(GetRouteLength).First();
        }
        private IReadOnlyList<Point> BuildManualRoute(
    Point start,
    ConnectionSide startSide,
    Point end,
    ConnectionSide endSide,
    List<Point> bends)
        {
            const double offset = 25;

            var points = new List<Point>();

            var startOut = GetOffsetPoint(start, startSide, offset);
            var endIn = GetOffsetPoint(end, endSide, offset);

            points.Add(start);
            points.Add(startOut);

            foreach (var bend in bends)
            {
                AddOrthogonalPath(points, bend);
            }

            AddOrthogonalPath(points, endIn);

            points.Add(end);

            return Simplify(points);
        }

        private List<IReadOnlyList<Point>> BuildCandidateRoutes(
            Point start,
            ConnectionSide startSide,
            Point end,
            ConnectionSide endSide)
        {
            const double offset = 25;
            const double detour = 80;

            var startOut = GetOffsetPoint(start, startSide, offset);
            var endIn = GetOffsetPoint(end, endSide, offset);

            var routes = new List<IReadOnlyList<Point>>();

            routes.Add(BuildRouteSegment(start, startSide, end, endSide));

            var middleX = (startOut.X + endIn.X) / 2;
            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(middleX, startOut.Y),
        new Point(middleX, endIn.Y),
        endIn,
        end
    }));

            var middleY = (startOut.Y + endIn.Y) / 2;
            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(startOut.X, middleY),
        new Point(endIn.X, middleY),
        endIn,
        end
    }));

            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(startOut.X, startOut.Y - detour),
        new Point(endIn.X, startOut.Y - detour),
        endIn,
        end
    }));

            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(startOut.X, startOut.Y + detour),
        new Point(endIn.X, startOut.Y + detour),
        endIn,
        end
    }));

            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(startOut.X - detour, startOut.Y),
        new Point(startOut.X - detour, endIn.Y),
        endIn,
        end
    }));

            routes.Add(Simplify(new List<Point>
    {
        start,
        startOut,
        new Point(startOut.X + detour, startOut.Y),
        new Point(startOut.X + detour, endIn.Y),
        endIn,
        end
    }));

            return routes;
        }

        private bool IntersectsObstacles(
    IReadOnlyList<Point> route,
    IEnumerable<Rect> obstacles)
        {
            if (route.Count < 2)
                return false;

            for (int i = 1; i < route.Count - 1; i++)
            {
                foreach (var obstacle in obstacles)
                {
                    if (SegmentIntersectsRect(
                        route[i],
                        route[i + 1],
                        obstacle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SegmentIntersectsRect(Point a, Point b, Rect rect)
        {
            if (rect.Contains(a) || rect.Contains(b))
                return true;

            var leftTop = new Point(rect.Left, rect.Top);
            var rightTop = new Point(rect.Right, rect.Top);
            var rightBottom = new Point(rect.Right, rect.Bottom);
            var leftBottom = new Point(rect.Left, rect.Bottom);

            return LinesIntersect(a, b, leftTop, rightTop)
                || LinesIntersect(a, b, rightTop, rightBottom)
                || LinesIntersect(a, b, rightBottom, leftBottom)
                || LinesIntersect(a, b, leftBottom, leftTop);
        }

        private bool LinesIntersect(Point a, Point b, Point c, Point d)
        {
            double Direction(Point p1, Point p2, Point p3)
            {
                return (p3.X - p1.X) * (p2.Y - p1.Y)
                     - (p2.X - p1.X) * (p3.Y - p1.Y);
            }

            var d1 = Direction(c, d, a);
            var d2 = Direction(c, d, b);
            var d3 = Direction(a, b, c);
            var d4 = Direction(a, b, d);

            return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
                && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
        }

        private double GetRouteLength(IReadOnlyList<Point> route)
        {
            double length = 0;

            for (int i = 0; i < route.Count - 1; i++)
            {
                var dx = route[i + 1].X - route[i].X;
                var dy = route[i + 1].Y - route[i].Y;

                length += Math.Sqrt(dx * dx + dy * dy);
            }

            return length;
        }
        private void AddOrthogonalPath(List<Point> points, Point target)
        {
            if (points.Count == 0)
            {
                points.Add(target);
                return;
            }

            var current = points[^1];

            if (Math.Abs(current.X - target.X) < 0.01 ||
                Math.Abs(current.Y - target.Y) < 0.01)
            {
                points.Add(target);
                return;
            }

            points.Add(new Point(target.X, current.Y));
            points.Add(target);
        }

        private IReadOnlyList<Point> BuildRouteSegment(
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

        private ConnectionSide GetSideToTarget(Point from, Point target)
        {
            var dx = target.X - from.X;
            var dy = target.Y - from.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                return dx > 0
                    ? ConnectionSide.Right
                    : ConnectionSide.Left;
            }

            return dy > 0
                ? ConnectionSide.Bottom
                : ConnectionSide.Top;
        }

        private void AddWithoutDuplicate(
            List<Point> target,
            IReadOnlyList<Point> source)
        {
            foreach (var point in source)
            {
                if (target.Count == 0)
                {
                    target.Add(point);
                    continue;
                }

                var last = target[^1];

                if (Math.Abs(last.X - point.X) < 0.01 &&
                    Math.Abs(last.Y - point.Y) < 0.01)
                    continue;

                target.Add(point);
            }
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
