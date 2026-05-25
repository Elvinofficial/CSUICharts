using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UICharts.Core.Enums;
using UICharts.Core.Models;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.Services
{
    public class ConnectionRoutingService : IConnectionRoutingService
    {
        private const double Offset = 50;
        private const double CorridorMargin = 40;
        private const double Epsilon = 0.01;

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

            var obstacleList = obstacles.ToList();

            return BuildSmartAutoRoute(
                start,
                startSide,
                end,
                endSide,
                obstacleList);
        }

        private IReadOnlyList<Point> BuildSmartAutoRoute(
            Point start,
            ConnectionSide startSide,
            Point end,
            ConnectionSide endSide,
            List<Rect> obstacles)
        {
            var startOut = GetOffsetPoint(start, startSide, Offset);
            var endIn = GetOffsetPoint(end, endSide, Offset);

            var xLines = BuildCorridorLines(
                start.X,
                startOut.X,
                endIn.X,
                end.X,
                obstacles.SelectMany(o => new[]
                {
                    o.Left - CorridorMargin,
                    o.Right + CorridorMargin
                }));

            var yLines = BuildCorridorLines(
                start.Y,
                startOut.Y,
                endIn.Y,
                end.Y,
                obstacles.SelectMany(o => new[]
                {
                    o.Top - CorridorMargin,
                    o.Bottom + CorridorMargin
                }));

            var nodes = BuildNodes(xLines, yLines);

            var startNode = GetOrAddNode(nodes, startOut);
            var endNode = GetOrAddNode(nodes, endIn);

            var path = FindPath(startNode, endNode, nodes, obstacles);

            if (path == null || path.Count == 0)
                return BuildFallbackRoute(start, startSide, end, endSide);

            var result = new List<Point>
            {
                start,
                startOut
            };

            AddWithoutDuplicate(result, path);

            result.Add(endIn);
            result.Add(end);

            return Simplify(result);
        }

        private List<double> BuildCorridorLines(
            double first,
            double second,
            double third,
            double fourth,
            IEnumerable<double> extra)
        {
            var values = new List<double>
            {
                first,
                second,
                third,
                fourth,
                (second + third) / 2
            };

            values.AddRange(extra);

            return values
                .DistinctBy(v => Math.Round(v, 2))
                .OrderBy(v => v)
                .ToList();
        }

        private List<Point> BuildNodes(
            IReadOnlyList<double> xLines,
            IReadOnlyList<double> yLines)
        {
            var nodes = new List<Point>();

            foreach (var x in xLines)
            {
                foreach (var y in yLines)
                {
                    nodes.Add(new Point(x, y));
                }
            }

            return nodes;
        }

        private Point GetOrAddNode(List<Point> nodes, Point point)
        {
            var existing = nodes.FirstOrDefault(p => AreSamePoint(p, point));

            if (!AreSamePoint(existing, default))
                return existing;

            nodes.Add(point);
            return point;
        }

        private List<Point>? FindPath(
            Point start,
            Point end,
            List<Point> nodes,
            List<Rect> obstacles)
        {
            var open = new List<Point> { start };
            var cameFrom = new Dictionary<Point, Point>();

            var gScore = nodes.ToDictionary(p => p, _ => double.PositiveInfinity);
            var fScore = nodes.ToDictionary(p => p, _ => double.PositiveInfinity);

            gScore[start] = 0;
            fScore[start] = GetManhattanDistance(start, end);

            while (open.Count > 0)
            {
                var current = open
                    .OrderBy(p => fScore[p])
                    .First();

                if (AreSamePoint(current, end))
                    return ReconstructPath(cameFrom, current);

                open.Remove(current);

                foreach (var neighbor in GetNeighbors(current, nodes, obstacles))
                {
                    var tentativeScore =
                        gScore[current] +
                        GetManhattanDistance(current, neighbor);

                    if (tentativeScore >= gScore[neighbor])
                        continue;

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeScore;
                    fScore[neighbor] =
    tentativeScore +
    GetManhattanDistance(neighbor, end) +
    GetBendPenalty(cameFrom, current, neighbor) +
    GetObstacleProximityPenalty(current, neighbor, obstacles);

                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }

            return null;
        }
        private double GetObstacleProximityPenalty(
    Point a,
    Point b,
    IEnumerable<Rect> obstacles)
        {
            double penalty = 0;

            foreach (var obstacle in obstacles)
            {
                var expanded = obstacle;
                expanded.Inflate(60, 60);

                if (SegmentIntersectsRect(a, b, expanded))
                    penalty += 500;
            }

            return penalty;
        }

        private IEnumerable<Point> GetNeighbors(
            Point current,
            List<Point> nodes,
            List<Rect> obstacles)
        {
            var sameX = nodes
                .Where(p => Math.Abs(p.X - current.X) < Epsilon && !AreSamePoint(p, current))
                .OrderBy(p => Math.Abs(p.Y - current.Y));

            var sameY = nodes
                .Where(p => Math.Abs(p.Y - current.Y) < Epsilon && !AreSamePoint(p, current))
                .OrderBy(p => Math.Abs(p.X - current.X));

            foreach (var neighbor in sameX.Concat(sameY))
            {
                if (!SegmentIntersectsAnyObstacle(current, neighbor, obstacles))
                    yield return neighbor;
            }
        }

        private bool SegmentIntersectsAnyObstacle(
            Point a,
            Point b,
            IEnumerable<Rect> obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                if (SegmentIntersectsRect(a, b, obstacle))
                    return true;
            }

            return false;
        }

        private double GetBendPenalty(
            Dictionary<Point, Point> cameFrom,
            Point current,
            Point next)
        {
            if (!cameFrom.TryGetValue(current, out var previous))
                return 0;

            var previousDirection = GetDirection(previous, current);
            var nextDirection = GetDirection(current, next);

            return previousDirection == nextDirection ? 0 : 30;
        }

        private string GetDirection(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) > Math.Abs(a.Y - b.Y)
                ? "H"
                : "V";
        }

        private List<Point> ReconstructPath(
            Dictionary<Point, Point> cameFrom,
            Point current)
        {
            var path = new List<Point> { current };

            while (cameFrom.TryGetValue(current, out var previous))
            {
                current = previous;
                path.Add(current);
            }

            path.Reverse();

            return (List<Point>)Simplify(path);
        }

        private IReadOnlyList<Point> BuildFallbackRoute(
            Point start,
            ConnectionSide startSide,
            Point end,
            ConnectionSide endSide)
        {
            return BuildRouteSegment(start, startSide, end, endSide);
        }

        private IReadOnlyList<Point> BuildManualRoute(
            Point start,
            ConnectionSide startSide,
            Point end,
            ConnectionSide endSide,
            List<Point> bends)
        {
            var points = new List<Point>();

            var startOut = GetOffsetPoint(start, startSide, Offset);
            var endIn = GetOffsetPoint(end, endSide, Offset);

            points.Add(start);
            points.Add(startOut);

            foreach (var bend in bends)
                AddOrthogonalPath(points, bend);

            AddOrthogonalPath(points, endIn);

            points.Add(end);

            return Simplify(points);
        }

        private void AddOrthogonalPath(List<Point> points, Point target)
        {
            if (points.Count == 0)
            {
                points.Add(target);
                return;
            }

            var current = points[^1];

            if (Math.Abs(current.X - target.X) < Epsilon ||
                Math.Abs(current.Y - target.Y) < Epsilon)
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
            var points = new List<Point>();

            var startOut = GetOffsetPoint(start, startSide, Offset);
            var endIn = GetOffsetPoint(end, endSide, Offset);

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

        private double GetManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private bool AreSamePoint(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) < Epsilon &&
                   Math.Abs(a.Y - b.Y) < Epsilon;
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

                if (AreSamePoint(last, point))
                    continue;

                target.Add(point);
            }
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

                if (AreSamePoint(last, point))
                    continue;

                result.Add(point);
            }

            for (int i = result.Count - 2; i > 0; i--)
            {
                var prev = result[i - 1];
                var current = result[i];
                var next = result[i + 1];

                var sameX =
                    Math.Abs(prev.X - current.X) < Epsilon &&
                    Math.Abs(current.X - next.X) < Epsilon;

                var sameY =
                    Math.Abs(prev.Y - current.Y) < Epsilon &&
                    Math.Abs(current.Y - next.Y) < Epsilon;

                if (sameX || sameY)
                    result.RemoveAt(i);
            }

            return result;
        }
    }
}