using MeshPathfinding.MapBuilding;
using MyMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MeshPathfinding.Pathfinding
{
    [Serializable]
    public class PathSolver
    {
        private NodeList nodes;
        private List<PathArea> areas;
        public RoadNetwork roadNetwork;
        internal PathSolver(int width,int height,List<Region> regions,List<TerrainType> terrainTypes)
        {
            nodes = new NodeList(width,height);
            areas = getPathAreas(regions, ref nodes, terrainTypes);

            ValidateNodes(nodes);
            roadNetwork = new RoadNetwork();
        }
        [Conditional("DEBUG")]
        private void ValidateNodes(NodeList nodes)
        {
            foreach(var node in nodes)
            {
                var links = node.links;
                if(links.Any(x=>x.TerrainType.name=="town")&& links.Any(x => x.TerrainType.name == "field"))
                {
                    return;
                }
            }
            throw new Exception();
        }

        public void AddRoads(List<PopulationCentre> popCentres, List<RoadType> roadTypes)
        {

            var links = getPopCenterConnections(popCentres);
            var types = roadTypes.OrderByDescending(x => x.weightRequirement);
            foreach (var link in links)
            {
                if (link.weight < 2)
                {
                    if((link.node1.position-link.node2.position).Length>200)
                    continue;
                }
                TerrainType roadType = types.First(x => x.weightRequirement < link.weight);
                var path = solvePath(link.node1, link.node2);
                roadNetwork.Add(path, roadType);
                //yield return new List<RoadSegment>(roadNetwork.roadSegments);
            }
        }
        public void AddRoads(IEnumerable<PopulationCentreConnection> popCentresConnections)
        {
            foreach(var link in popCentresConnections)
            {
                var path = solvePath(link.node1, link.node2);
                roadNetwork.Add(path, link.roadType);
            }
        }
        private static IEnumerable<PopulationCentreConnection> getPopCenterConnections(List<PopulationCentre> popCentres)
        {
            SortedList<float, PopulationCentreConnection> links = new SortedList<float, PopulationCentreConnection>((popCentres.Count* (popCentres.Count-1))/2);
            for (int i = 0; i < popCentres.Count - 1; i++)
            {
                for (int j = i + 1; j < popCentres.Count; j++)
                {
                    var connection = new PopulationCentreConnection(popCentres[i], popCentres[j]);
                    try
                    {
                        links.Add(connection.weight, connection);
                    }
                    catch(ArgumentException a) { }

                }
            }
            return links.Values.Reverse();
        }
        private List<PathArea> getPathAreas(List<Region> regions, ref NodeList nodes, List<TerrainType> terrainTypes)
        {
            List<PathArea> areas = new List<PathArea>();
            foreach (Region region in regions)
            {
                TerrainType terrainType = terrainTypes[region.biome-1];
                foreach (var tri in region.pathMesh.Triangles)
                {
                    areas.Add(new PathArea(tri, ref nodes, terrainType));
                }
            }
            return areas;
        }
        public List<Point> getPath(Point origin, Point destination)
        {
            var originNode = new PathNode(origin);
            var destinationNode = new PathNode(destination);
            var path = solvePath(originNode, destinationNode);
            originNode.Remove();
            destinationNode.Remove();
            return path.ConvertAll(x => x.position);
        }
        private List<PathNode> solvePath(PathNode origin,PathNode destination)
        {
            AddNode(ref origin);
            AddNode(ref destination);
            return Astar.findPath(origin, destination);
        }

        private void AddNode(ref PathNode origin)
        {
            if (!nodes.Contains(origin))
            {
                origin=nodes.AddAndGet(origin);
                foreach (var area in areas)
                {
                    if (area.isInside(origin))
                    {
                        area.AddNode(origin);
                    }
                }
            }
        }
        /*public List<Point> navigationNodes()
        {
            return nodes.ConvertAll(x => x.position);
        }*/
    }
}
