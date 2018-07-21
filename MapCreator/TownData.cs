using MeshPathfinding.MapBuilding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TriangleNet;
using TriangleNet.Meshing;
using TriangleNet.Geometry;
using System.ComponentModel;

namespace MapCreator
{
    class TownData
    {

        public Canvas townsCanvas;
        public List<PopulationCentreConnection> populationConnections = new List<PopulationCentreConnection>();
        public TownData(OptionsDataContext optionsDataContext)
        {
            Configuration config = new Configuration();
            var startPoints = optionsDataContext.MapData.popCentres;//getStartPoints(, (int)optionsDataContext.SettlementSize);
            List<PopulationCentre> towns = new List<PopulationCentre>();
            List<PopulationCentre> villages = new List<PopulationCentre>();
            foreach (var settlement in startPoints)
            {
                if (settlement.population > optionsDataContext.TownPopulation)
                {
                    towns.Add(settlement);
                }
                else if (settlement.population > optionsDataContext.VillagePopulation)
                {
                    villages.Add(settlement);
                }
            }
            townsCanvas = new Canvas();
            townsCanvas.Width = optionsDataContext.MapData.terrainMap.GetLength(0);
            townsCanvas.Height = optionsDataContext.MapData.terrainMap.GetLength(1);

            var highway = new RoadType();
            highway.cost = 1;
            highway.name = "highway";
            var highwayConnections = getNearestNeighbourConnections(towns,highway);
            highwayConnections.ForEach(x => townsCanvas.Children.Add(DrawConnection(x, true)));
            /*for (int i = 0; i < towns.Count - 1; i++)
            {
                for (int j = i + 1; j < towns.Count; j++)
                {
                    var connection = new PopulationCentreConnection(towns[i], towns[j]);
                    connection.roadType = highway;
                    if (connection.Distance < optionsDataContext.HighwayDistance)
                    {
                        highwayConnections.Add(connection);
                        townsCanvas.Children.Add(DrawConnection(connection, true));
                    }
                }
            }*/
            var road = new RoadType();
            road.cost = 1;
            road.name = "road";
            var roadConnections = getNearestNeighbourConnections(towns.Concat(villages).ToList(), road, highwayConnections);
            roadConnections.ForEach(x => townsCanvas.Children.Add(DrawConnection(x, false)));
            /*foreach (var town in towns)
            {
                foreach (var village in villages)
                {
                    var connection = new PopulationCentreConnection(town, village);
                    connection.roadType = road;
                    if (connection.Distance < optionsDataContext.TownRoadDistance)
                    {
                        roadConnections.Add(connection);
                        townsCanvas.Children.Add(DrawConnection(connection, false));
                    }
                }
            }
            for (int i = 0; i < villages.Count - 1; i++)
            {
                for (int j = i + 1; j < villages.Count; j++)
                {
                    var connection = new PopulationCentreConnection(villages[i], villages[j]);
                    connection.roadType = highway;
                    if (connection.Distance < optionsDataContext.VillageRoadDistance)
                    {
                        highwayConnections.Add(connection);
                        townsCanvas.Children.Add(DrawConnection(connection, false));
                    }
                }
            }
            */
            foreach (var town in towns)
            {
                townsCanvas.Children.Add(getTownMarker(town));
            }
            foreach (var village in villages)
            {
                townsCanvas.Children.Add(getVillageMarker(village));
            }
            highwayConnections.OrderByDescending(x => x.weight);
            roadConnections.OrderByDescending(x => x.weight);
            populationConnections.AddRange(highwayConnections);
            populationConnections.AddRange(roadConnections);
        }
        List<PopulationCentreConnection> getNearestNeighbourConnections(List<PopulationCentre> pops,RoadType roadType,List<PopulationCentreConnection> connections=null)
        {
            var geometry = new TriangleNet.Geometry.Polygon();
            //Dictionary<Vertex, PopulationCentre> popDictionary = new Dictionary<Vertex, PopulationCentre>();
            var connectionList = new List<PopulationCentreConnection>();
            foreach (var pop in pops)
            {
                var vertex = new Vertex(pop.Position.x, pop.Position.y);
                //popDictionary.Add(vertex, pop);
                geometry.Add(vertex);
            }
            var mesh=geometry.Triangulate();
            int org, dest;
            ITriangle neighbor;
            int nid;
            foreach (var tri in mesh.Triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    // The neighbor opposite of vertex i.
                    GetNeighbor(tri, i, out neighbor, out nid);

                    // Consider each edge only once.
                    if ((tri.ID < nid) || (nid < 0))
                    {
                        // Get the edge endpoints.

                        org = tri.GetVertexID((i + 1) % 3);
                        dest = tri.GetVertexID((i + 2) % 3);
                        var pop1 = pops[org];
                        var pop2 = pops[dest];
                        var connection = new PopulationCentreConnection(pop1, pop2);
                        connection.roadType = roadType;
                        if (connections == null || !connections.Any(x => x.Equal(connection)))
                        {
                            connectionList.Add(connection);
                        }
                        
                    }
                }
            }
            return connectionList;



        }
        private void GetNeighbor(ITriangle tri, int i, out ITriangle neighbor, out int nid)
        {
            neighbor = tri.GetNeighbor(i);
            nid = neighbor == null ? -1 : neighbor.ID;
        }
        UIElement getTownMarker(PopulationCentre town)
        {
            var marker = new System.Windows.Shapes.Rectangle();
            marker.Width = 5;
            marker.Height = 5;
            marker.Fill = new SolidColorBrush(Color.FromRgb(0,0,255));
            marker.Stroke= new SolidColorBrush(Color.FromRgb(0, 0, 0));
            marker.StrokeThickness = 1;
            Canvas.SetLeft(marker, town.Position.x - 2);
            Canvas.SetTop(marker, town.Position.y - 2);
            return marker;
        }
        UIElement getVillageMarker(PopulationCentre town)
        {
            var marker = new Ellipse();
            marker.Width = 3;
            marker.Height = 3;
            marker.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            marker.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            marker.StrokeThickness = 1;
            Canvas.SetLeft(marker, town.Position.x - 1);
            Canvas.SetTop(marker, town.Position.y - 1);
            return marker;
        }
        UIElement DrawConnection(PopulationCentreConnection connection,bool highway)
        {
            var line = new Line();
            line.X1 = connection.Position1.x;
            line.Y1 = connection.Position1.y;
            line.X2 = connection.Position2.x;
            line.Y2 = connection.Position2.y;
            float thickness;
            Brush brush;
            if (highway)
            {
                thickness = 2;
                brush = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            }
            else
            {
                thickness = 1;
                brush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            }
            line.Stroke = brush;
            line.StrokeThickness = thickness;
            return line;
        }
        private List<PopulationCentre> getStartPoints(float[,] popMap, int range)
        {
            List<PopulationCentre> startPoints = new List<PopulationCentre>();
            for (int i = range; i < popMap.GetLength(0) - range; i++)
            {
                for (int j = range; j < popMap.GetLength(1) - range; j++)
                {
                    if (popMap[i, j] == 0) continue;
                    if (localMaximum(i, j, popMap, range))
                    {
                        var pop = getPop(i, j, popMap, range);
                        
                        startPoints.Add(new PopulationCentre(new MyMath.Point(i, j), pop));
                        
                    }
                }

            }
            return startPoints;
        }

        private float getPop(int i, int j, float[,] popMap, int range)
        {
            float sum = 0;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (popMap[i + dx, j + dy] > 0)
                    {
                        sum += popMap[i + dx, j + dy];
                    }
                }
            }
            return sum;
        }

        private bool localMaximum(int i, int j, float[,] popMap, int range)
        {
            var pop = popMap[i, j];
            if (pop <= 0) return false;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (popMap[i + dx, j + dy] > pop) return false;
                }
            }
            return true;
        }
        public IEnumerable<PopulationCentreConnection> RoadIterator(BackgroundWorker worker,int progress)
        {
            int count = populationConnections.Count;
            int i = 0;
            foreach(var connection in populationConnections)
            {
                yield return connection;
                worker.ReportProgress(progress, string.Format("Adding Road ({0}/{1})", i++, count));
                progress++; 
            }
        }
    }
    class PointSet : ITriangulator
    {
        public IMesh Triangulate(IList<Vertex> points, Configuration config)
        {
            throw new NotImplementedException();
        }
    }
}
