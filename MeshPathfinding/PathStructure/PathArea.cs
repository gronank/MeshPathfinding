using MyMath;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Topology;
using System;

namespace MeshPathfinding
{
    [Serializable]
    internal class PathArea
    {
        float intermediatePointDistance=15;
        Bounds bounds;
        List<PathNode> nodes = new List<PathNode>();
        List<PathNode> corners = new List<PathNode>();
        List<RoadIntersector> roadIntersectors = new List<RoadIntersector>();
        TerrainType terrainType;
        public PathArea(Triangle tri,ref NodeList nodeList,TerrainType terrainType)
        {
            this.terrainType = terrainType;
            for (int i = 0; i < 3; i++)
            {

               corners.Add(nodeList.AddAndGet(tri.GetVertex(i)));
            }
            nodes.AddRange(corners);
            bounds = new Bounds(corners.ConvertAll(x => x.position));
            for (int i = 0; i < 3; i++)
            {
                int j = (i + 1) % 3;
                foreach (var midPoint in getIntermediatePoints(corners[i].position , corners[j].position)) { 
                    PathNode midNode = nodeList.AddAndGet(midPoint);
                    nodes.Add(midNode);
                }
            }
            for(int i = 0; i < nodes.Count - 1; i++)
            {
                for (int j = i+1; j < nodes.Count; j++)
                {
                    createLink(nodes[i], nodes[j], terrainType);
                }
            }
            
        }
        private IEnumerable<Point> getIntermediatePoints(Point p1, Point p2)
        {
            int n = (int)((p1 - p2).Length / intermediatePointDistance);
            for (int k = 0; k < n; k++)
            {
                yield return 1 / (1f + n) * ((k + 1) * p1 + (n - k) * p2);
            }
        }
        internal List<Point> createRoad(PathLink pathLink, TerrainType roadType)
        {
            var link2 = pathLink.reverseLink;
            var node1 = link2.destinationNode;
            var node2 = pathLink.destinationNode;
            node1.links.Remove(pathLink);
            node2.links.Remove(link2);
            var originNode = node1;
            var roadNodes = new List<PathNode>();
            foreach (var destinationPoint in getIntermediatePoints(node1.position, node2.position))
            {
                
                var destinationNode = new PathNode(destinationPoint);
                roadNodes.Add(destinationNode);
                createLink(originNode, destinationNode, roadType);
                originNode = destinationNode;
            }
            createLink(originNode, node2, roadType);

            //add links to rest of area nodes
            foreach(var areaNode in nodes)
            {
                if (areaNode == node1 || areaNode == node2) continue;
                foreach (var roadNode in roadNodes)
                {
                    createLink(areaNode, roadNode, terrainType);
                }
            }

            //prune illegal paths
            var newIntersector = new RoadIntersector(node1.position, node2.position);
            roadIntersectors.Add(newIntersector);
            foreach(var node in nodes)
            {
                newIntersector.prune(node);

            }

            nodes.AddRange(roadNodes);

            var roadPoints=roadNodes.ConvertAll(x=>x.position);
            roadPoints.Add(node2.position);
            return roadPoints;
        }

        public void AddNode(PathNode newNode)
        {
            foreach(var node in nodes)
            {
                createLink(newNode, node, terrainType);
            }
        }
        private void createLink(PathNode pathNode1, PathNode pathNode2, TerrainType terrainType)
        {
            if (roadIntersectors.Any(x => x.intersects(pathNode1.position, pathNode2.position))) return;
            float length = (pathNode1.position - pathNode2.position).Length;
            PathLink link1 = new PathLink(pathNode2, terrainType, length,this);
            PathLink link2 = new PathLink(pathNode1, terrainType, length,this);

            link1.reverseLink = link2;
            link2.reverseLink = link1;

            pathNode1.Add(link1);
            pathNode2.Add(link2);
        }
        public bool isInside(PathNode node)
        {
            if (!bounds.inside(node.position)) return false;
            for (int i = 0; i < 3; i++)
            {
                int j = (i + 1) % 3;
                var p1 = corners[i].position;
                var p2 = corners[j].position;
                var v = p2 - p1;
                var p = node.position - p1;
                if (v.Cross(p) < 0) return false;
            }
            return true;

        }
        [Serializable]
        private class Bounds
        {
            double xMin, xMax, yMin, yMax;
            Bounds(Point p1, Point p2, Point p3)
                :this(new List<Point>() { p1, p2, p3 })
            {}
            public Bounds(List<Point> points)
            {
                xMin = points.Min(p => p.x);
                yMin = points.Min(p => p.y);
                xMax = points.Max(p => p.x);
                yMax = points.Max(p => p.y);
            }
            public bool inside(Point p)
            {
                return p.x >= xMin && p.x <= xMax && p.y >= yMin && p.y <= yMax;
            }

        }
        [Serializable]
        private class RoadIntersector
        {
            Point origin, delta;
            public RoadIntersector(Point p1,Point p2)
            {
                this.origin = p1;
                this.delta = p2-p1;
            }

            internal void prune(PathNode node)
            {
                var point = node.position;
                for(int i= node.links.Count-1;i>=0;i--)
                {
                    var link = node.links[i];
                    if (intersects(point, link.destinationNode.position))
                    {
                        link.Remove();
                    }
                }
            }

            internal bool intersects(Point v1,Point v2)
            {
                var s = v2 - v1;
                var q = v1 - origin;
                var r = delta;
                var rs = r.Cross(s);
                float t = q.Cross(s)/rs;
                float u = q.Cross(r)/rs;
                if (!(t > 0 && t < 1)) return false;
                return u >= 0 && u <= 1;
            }
        }
    }
}