using MeshPathfinding.Pathfinding;
using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace MeshPathfinding
{
    public class World
    {
        //public List<Polygon> polygons;
        List<Region> regions;
        List<TerrainType> terrainTypes;
        int width, height;
        public PathSolver pathSolver;
        public World(int[,] biomeMap, List<TerrainType> terrainTypes)
        {
            width = biomeMap.GetLength(0);
            height = biomeMap.GetLength(1);
            this.terrainTypes = terrainTypes;
            regions = buildRegions(biomeMap);
            //polygons = regions.ConvertAll(x => x.polygon);
            //NodeList nodes = new NodeList(regions);
            //List<PathArea> areas = getPathAreas(regions, ref nodes, terrainTypes);
        }
        public PathSolver getPathSolver()
        {
            if (pathSolver == null)
            {
                pathSolver = new PathSolver(width, height, regions, terrainTypes);
            }
            return pathSolver;
        }
        public List<IMesh> getMeshes()
        {
            return regions.ConvertAll(x => x.pathMesh);
        }
        public WorldData getWorldData()
        {
            WorldData data = new WorldData();
            data.RegionData = getRegionData();
            data.pathSolver = getPathSolver();
            data.terrainTypes = terrainTypes;
            return data;
        }
        public List<RegionData> getRegionData()
        {
            return regions.ConvertAll(x => new RegionData(x));
        }
        private List<Region> buildRegions(int[,] biomeMap)
        {
            List<RegionRule> regionRules = buildRegionRules(biomeMap);
            //regionRules.RemoveAt(0);
            List<Region> regions = regionRules.ConvertAll(x => new Region(x, biomeMap));
            return regions;
        }
        /*public Image Test(int[,] biomeMap)
        {
            Bitmap map;
            buildRegionRules(biomeMap),out map);
            return map;
        }*/
        List<RegionRule> buildRegionRules(int[,] biomeMap)//,out Bitmap map)
        {

            int width = biomeMap.GetLength(0);
            int height = biomeMap.GetLength(1);
            //map = new Bitmap(width, height);
            //image = new Bitmap(width, height);
            RegionRule[,] regionMap = new RegionRule[width, height];
            //int regionId = 1;
            BorderRuleList borderRules = new BorderRuleList();

            List<RegionRule> regionRules = new List<RegionRule>();
            Queue<Edge> newRegionQueue = new Queue<Edge>();
            newRegionQueue.Enqueue(new Edge(new PointInt(0, 0), new PointInt(0, -1)));
            while (newRegionQueue.Count > 0)
            {
                Queue<PointInt> regionExplorationQueue = new Queue<PointInt>();
                var startEdge = newRegionQueue.Dequeue();
                var startPoint = startEdge.touchPoint;
                if (regionMap.getValue(startPoint) != null) continue;
                var workRegion = new RegionRule();
                regionRules.Add(workRegion);
                workRegion.biome = biomeMap[startPoint.x,startPoint.y];
                regionExplorationQueue.Enqueue(startPoint);

                if (!(startPoint.x == 0 && startPoint.y == 0))
                {
                    var neighbour = regionMap.getValue(startEdge.otherTouchPoint);
                    neighbour.edges.Add(startEdge.Reverse());
                }
                workRegion.edges.Add(startEdge);





                while (regionExplorationQueue.Count > 0)
                {
                    var explorationPoint = regionExplorationQueue.Dequeue();

                    
                    for (int d = 0; d < 6; d++)
                    {
                        Direction dir = new Direction(d);
                        if (regionMap.getBorderValue(explorationPoint, dir) == null)
                        {
                            PointInt neighbourPoint = explorationPoint.Neighbor(dir);
                            if (biomeMap.inBounds(neighbourPoint))
                            {
                                if (biomeMap.sameAsNeighbor(explorationPoint, dir))
                                {
                                    regionExplorationQueue.Enqueue(neighbourPoint);
                                    regionMap[neighbourPoint.x, neighbourPoint.y] = workRegion;
                                }
                                else
                                {
                                    if (!dir.Diagonal())
                                    {
                                        newRegionQueue.Enqueue(new Edge(neighbourPoint, explorationPoint));
                                    }
                                }
                            }
                        }
                    }
                }
                regionMap[startPoint.x , startPoint.y] = workRegion;
            }


            return regionRules;
        }
    }
    internal class RegionRule
    {
        public int biome = -1;
        public List<Edge> edges = new List<Edge>();

        /*internal void Clean()
        {
            List<Edge> uniqueEdges = new List<Edge>();
            for(int i = edges.Count - 1; i >= 0; i--)
            {
                var edge = edges[i];
                if (this == edge.opposite.region)
                {
                    edges.Remove(edge);
                }
                else if(uniqueEdges.Any(x=>x.opposite.region== edge.opposite.region))
                {
                    edges.Remove(edge);
                }
                else
                {
                    uniqueEdges.Add(edge);
                }
            }
            edges = uniqueEdges;
        }*/

        internal void setBiome(int v)
        {
            if (biome != -1 && biome != v) throw new ArgumentException();
            biome = v;
        }
    }
    internal class Edge
    {
        public PointInt touchPoint;
        public PointInt otherTouchPoint;
        public Edge(PointInt p1, PointInt p2)
        {
            touchPoint = p1;
            otherTouchPoint = p2;
        }
        public Edge Reverse()
        {
            return new Edge(otherTouchPoint, touchPoint);
        }
        /*public RegionRule region;
        public Edge opposite;*/
    }
    internal class BorderRuleList : List<BorderRule>
    {
        internal void AddRule(int[,] regionMap, PointInt point, int direction)
        {
            if (!regionMap.sameAsNeighbor(point, direction))
            {
                var newRule = new BorderRule(regionMap, point, direction);
                this.Add(newRule);
            }
        }
        internal new void Add(BorderRule rule)
        {
            if (!this.Any(x => x.isDuplicate(rule)))
            {
                base.Add(rule);
            }
        }
    }
    class Region
    {
        private static int regionCount = 0;
        private static MyMath.Point[] facePos = new MyMath.Point[]
        {
            new MyMath.Point(0,-.5f),
            new MyMath.Point(.5f,-.5f),
            new MyMath.Point(.5f,0),
            new MyMath.Point(0,.5f),
            new MyMath.Point(-.5f,.5f),
            new MyMath.Point(-.5f,0)
        };
        private static MyMath.Point[] cornerPos = new MyMath.Point[]
        {
            new MyMath.Point(-.5f,-.5f),
            new MyMath.Point(.5f,-.5f),
            new MyMath.Point(.5f,-.5f),
            new MyMath.Point(.5f,.5f),
            new MyMath.Point(-.5f,.5f),
            new MyMath.Point(-.5f,.5f)
        };
        public int biome = -1;
        public int Id;
        public List<MyMath.Point> outerEdge;
        public List<Hole> holes = new List<Hole>();
        public TriangleNet.Meshing.IMesh pathMesh;
        public TriangleNet.Meshing.IMesh graphicsMesh;
        public Region(RegionRule x, int[,] biomeMap)
        {
            Id = regionCount++;
            biome = x.biome;
            foreach (var edge in x.edges)
            {
                foreach (int direction in startDirections(edge.touchPoint, biomeMap))
                {
                    calculateBounds(edge, direction, biomeMap);
                }
            }
            Simplify();
            if (holes.Count == 0)
            {
                foreach (var edge in x.edges)
                {
                    foreach (int direction in startDirections(edge.touchPoint, biomeMap))
                    {
                        calculateBounds(edge, direction, biomeMap);
                    }
                }
            }
            outerEdge = holes.First().Border;
            holes.RemoveAt(0);
            //holes.ForEach(hole => hole.Border.Reverse());
            var options = new ConstraintOptions()
            {
                ConformingDelaunay = true
            };
            var quality = new QualityOptions()
            {
                MinimumAngle = 25
            };
            var polygon = getPolygon();
            pathMesh = polygon.Triangulate(options, quality);
            graphicsMesh = polygon.Triangulate();
        }

        private void Simplify()
        {
            foreach (var hole in holes)
            {
                var bound = hole.Border;
                for (int i = bound.Count - 1; i >= 0; i--)
                {
                    var origin = bound[(i + 1) % bound.Count];
                    var current = bound[i];
                    var next = bound[(i + bound.Count - 1) % bound.Count];
                    if ((current - origin).Angle == (next - origin).Angle)
                    {
                        bound.RemoveAt(i);
                    }
                }
            }

        }

        private void calculateBounds(Edge edge, int direction, int[,] biomeMap)
        {
            Direction dir = new Direction(direction);
            var point = edge.touchPoint;
            MyMath.Point startFace = getFace(point, dir);
            int previousNeighbourBiome = biomeMap.getBorderValue(point, dir);
            dir.Increment();
            if (holes.Any(x => x.Border.Any(y => y.Equal(startFace)))) return;
            Hole border = new Hole(edge.otherTouchPoint);
            PointInt currentPoint = point;

            while (true)
            {
                int newNeighbourBiome = biomeMap.getBorderValue(currentPoint, dir);
                if (!biomeMap.sameAsNeighbor(currentPoint, dir))
                {
                    if (!dir.Diagonal())
                    {
                        if (previousNeighbourBiome != newNeighbourBiome)
                        {
                            border.Add(getCornerVertex(currentPoint, dir));
                            previousNeighbourBiome = newNeighbourBiome;
                        }
                        MyMath.Point face = getFace(currentPoint, dir);
                        if (face.Equal(startFace))
                        {
                            break;
                        }
                        else
                        {
                            border.Add(face);
                        }
                    }
                    dir.Increment();
                }
                else
                {
                    currentPoint = currentPoint.Neighbor(dir);
                    if (dir.Diagonal())
                    {
                        dir.Decrement();
                    }
                    dir.Decrement();
                    if (dir.Diagonal())
                    {
                        dir.Decrement();
                    }
                }

            }
            if (border.Border.Count == 0) throw new Exception();
            border.Add(startFace);
            holes.Add(border);
        }

        private MyMath.Point getFace(PointInt point, int dir)
        {
            MyMath.Point pointF = (MyMath.Point)point;
            MyMath.Point face = pointF + facePos[dir];
            return face;
        }
        private MyMath.Point getCornerVertex(PointInt point, int dir)
        {
            MyMath.Point pointF = (MyMath.Point)point;

            MyMath.Point face = pointF + cornerPos[dir];
            return face;
        }

        private IEnumerable<int> startDirections(PointInt touchPoint, int[,] biomeMap)
        {
            Direction dir = new Direction(0);
            for (int i = 0; i < 6; i++)
            {
                if (!dir.Diagonal() && !biomeMap.sameAsNeighbor(touchPoint, i))
                {
                    yield return i;
                }
                dir.Increment();
            }
        }
        public Polygon getPolygon()
        {
            var pol = new Polygon();
            int k = 1;
            var contour = new Contour(outerEdge.ConvertAll(x => x.getVertex(k)), k);
            pol.Add(contour);
            //var h = holes.Where(x => x.Border.Count ==29).Skip(3);
            //var i=holes.IndexOf(h.First());
            foreach (var inner in holes)
            {
                //if (inner.Count>=16) continue;
                k += 1;
                var holeContour = new Contour(inner.Border.ConvertAll(x => x.getVertex(k)), k);
                //var p = edges[i - 1].touchPoint;
                pol.Add(holeContour,inner.inside);
                
            }
            return pol;

        }
    }
    internal class Hole
    {
        public Vertex inside;
        public List<MyMath.Point> Border=new List<MyMath.Point>();
        public Hole(MyMath.PointInt p)
        {
            inside = new Vertex(p.x, p.y);
        }
        public void Add(MyMath.Point p)
        {
            Border.Add(p);
        }

    }
    internal class BorderRule
    {
        public int regionId1;
        public PointInt touchingPoint1;
        public int regionId2;
        public PointInt touchingPoint2;
        public BorderRule(int[,] regionMap, PointInt point, int direction)
        {
            touchingPoint1 = point;
            regionId1 = regionMap.getValue(point);

            touchingPoint2 = point.Neighbor(direction);
            regionId2 = regionMap.getValue(touchingPoint2);

        }
        public bool isDuplicate(BorderRule rule)
        {
            if (regionId1 == rule.regionId1 && regionId2 == rule.regionId2) return true;
            if (regionId2 == rule.regionId1 && regionId1 == rule.regionId2) return true;
            return false;
        }
        public bool sameBiome(int[,] biome)
        {
            return biome.getValue(touchingPoint1) == biome.getValue(touchingPoint2);
        }
    }
    class Direction
    {
        int dir;
        int count = 6;
        public Direction(int i)
        {
            dir = i;
        }
        public void Increment()
        {
            dir += 1;
            dir = (dir + count) % count;
        }
        public void Decrement()
        {
            dir -= 1;
            dir = (dir + count) % count;
        }
        public bool Diagonal()
        {
            return dir == 1 || dir == 4;
        }
        public static implicit operator int(Direction d)
        {
            return d.dir;
        }
    }
}
