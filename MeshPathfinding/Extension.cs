using MeshPathfinding.Pathfinding;
using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TriangleNet.Geometry;

namespace MeshPathfinding
{
    public static class Extension
    {
        public static bool sameAsNeighbor<T>(this T[,] map, PointInt p, int direction)
        {
            return map.getBorderValue(p, direction).Equals( map.getValue(p));
        }
        public static T getBorderValue<T>(this T[,] map, PointInt p,int direction)
        {
            var neighbor = p.Neighbor(direction);
            return map.getValue(neighbor);
        }
        public static T getValue<T>(this T[,] map, PointInt p)
        {
            int x = p.x;
            int y = p.y;
            if (!map.inBounds(p)) return default(T);
            return map[x, y];
        }
        public static int getValue(this int[,] map, PointInt p)
        {
            int x = p.x;
            int y = p.y;
            if (!map.inBounds(p)) return -1;
            return map[x, y];
        }
        public static bool inBounds<T>(this T[,] map, PointInt p)
        {
            int x = p.x;
            int y = p.y;
            return !(x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1));
        }
        public static PointInt Neighbor(this MyMath.PointInt p,int direction)
        {
            if (direction < 0 || direction > 5) throw new ArgumentException();
            switch (direction)
            {
                case 0:
                    return p + new PointInt(0, -1);
                case 1:
                    return p + new PointInt(1, -1);
                case 2:
                    return p + new PointInt(1, 0);
                case 3:
                    return p + new PointInt(0, 1);
                case 4:
                    return p + new PointInt(-1, 1);
                case 5:
                    return p + new PointInt(-1, 0);
                default:
                    return p;

            }
        }
        public static Vertex getVertex(this MyMath.Point p,int m)
        {
            return new Vertex(p.x, p.y,m);
        }
        public static MyMath.Point getPoint(this TriangleNet.Geometry.Point p)
        {
            return new MyMath.Point((float)p.X, (float)p.Y);
        }
        internal static void EnqueueNode(this Priority_Queue.FastPriorityQueue<SolutionNode> queue,SolutionNode node)
        {
            queue.Enqueue(node, node.TotalDistance);
        }
    }
}
