using Fractal;
using MeshPathfinding;
using MeshPathfinding.MapBuilding;
using MeshPathfinding.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TriangleNet;
namespace BorderTest
{
    public partial class Form1 : Form
    {
        PathSolver pathfinding;
        IEnumerator<List<RoadSegment>> segmentGeneration;
        public Form1()
        {
            InitializeComponent();
            int[,] biomeMap;
            var popCenters=buildData(out biomeMap);
            var field = new TerrainType()
            {
                cost = 10f,
            };
            var wood = new TerrainType()
            {
                cost = 2f,
            };
            var town = new TerrainType()
            {
                cost = 1.5f,
            };
            World world = new World(biomeMap,new List<TerrainType>() { field, wood,town });
            Console.WriteLine("built world");
            pathfinding = world.getPathSolver();
            Console.WriteLine("built pathFinding");
            //pictureBox.Image = world.image;
            //pictureBox.Image=world.();
            var road = new RoadType()
            {
                weightRequirement = 0,
                cost = 1f
            };
            var highway = new RoadType()
            {
                weightRequirement = 5f,
                cost = 1f
            };
            var roads = new List<RoadType> { road,highway };
            var popCenter1 = new PopulationCentre(new MyMath.Point(10, 10), 100);
            var popCenter2 = new PopulationCentre(new MyMath.Point(74, 3), 30);
            var popCenter3 = new PopulationCentre(new MyMath.Point(230, 100), 70);
            var popCenter4 = new PopulationCentre(new MyMath.Point(200, 250), 30);
            var pops = new List<PopulationCentre>() { popCenter1,popCenter3, popCenter4 };
            pathfinding.AddRoads(popCenters, roads);
            
            //var path = pathfinding.getPath(new MyMath.Point(3, 3), new MyMath.Point(95, 53));
            var g = Graphics.FromImage(pictureBox.Image);

            foreach (var segment in pathfinding.roadNetwork.roadSegments)
            {
                Color c = Color.Gray;
                if ((segment.roadType as RoadType).weightRequirement !=0) c = Color.Purple;
                g.DrawLines(new Pen(c, 1), segment.ConvertAll(x => new PointF(x.x, x.y)).ToArray());
            }
            pictureBox.Image.Save("roadMap.png");
            var meshes=world.getMeshes();
            /*foreach(var v in pathfinding.navigationNodes())
            {
                
                    g.FillRectangle(new SolidBrush(Color.Red), (float)v.x, (float)v.y, 1, 1);
                
            }*/
        }
        public List<PopulationCentre> buildData(out int[,] terrainMap)
        {
            var options = new FractalOptions(10, 15);
            int n = 10;
            var popFractal = new TiledFractal(128, n, n, options, FractalWrapMode.NoWrap);
            popFractal.Initialize(2576987386);
            var popMap = popFractal.getMap();

            var forestFractal = new TiledFractal(128, n, n, options, FractalWrapMode.NoWrap);
            var forestMap = forestFractal.getMap();
            var forestLevels = FractalUtility.getCutOffLevels(forestMap, new float[] { 80, 20 });
            var image = new Bitmap(128 * n, 128 * n);
            terrainMap = new int[128 * n, 128 * n];
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    popMap[i, j] = popMap[i, j] - forestMap[i, j];
                }
            }
            var popLevels = FractalUtility.getCutOffLevels(popMap, new float[] { 98, 2 });
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    popMap[i, j] = popMap[i, j] - popLevels[0];
                    if (popMap[i, j] < 0) popMap[i, j] = 0;
                    var pop = (popMap[i, j] / (popLevels[1] - popLevels[0]));
                    if (forestMap[i, j] > forestLevels[0])
                    {
                        terrainMap[i, j] = 1;
                        forestMap[i, j] -= forestLevels[0];
                        pop -= 1f;
                        var g =  (int)(255 * forestMap[i, j] / (forestLevels[1] - forestLevels[0]));
                        image.SetPixel(i, j, Color.FromArgb(255 - g, 255, 255 - g));
                    }
                    else// if (pop >0)
                    {
                        terrainMap[i, j] = 2;
                        var b =  (int)(255 * pop);
                        image.SetPixel(i, j, Color.FromArgb(255, 255 - b, 255 - b));
                    }
                    /*else
                    {
                        image.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }*/
                    popMap[i, j] = pop;


                }
            }
            pictureBox.Image = image;
            return getStartPoints(popMap);
        }
        private List<PopulationCentre> getStartPoints(float[,] popMap)
        {
            int range = 15;
            List<PopulationCentre> startPoints = new List<PopulationCentre>();
            for (int i = range; i < popMap.GetLength(0) - range; i++)
            {
                for (int j = range; j < popMap.GetLength(1) - range; j++)
                {
                    if (popMap[i, j] == 0) continue;
                    if (localMaximum(i, j, popMap, range))
                    {
                        var pop = getPop(i, j, popMap, range);
                        if (pop > 1)
                        {
                            startPoints.Add(new PopulationCentre(new MyMath.Point(i, j), pop));
                        }
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
        private void pictureBox_Click(object sender, EventArgs e)
        {
            
        }

        /*private void button1_Click(object sender, EventArgs e)
        { 
            if(!segmentGeneration.MoveNext())return;

            var g = Graphics.FromImage(pictureBox.Image);
            /*foreach (var v in pathfinding.navigationNodes())
            {

                g.FillRectangle(new SolidBrush(Color.Red), (float)v.x - 1, (float)v.y - 1, 3, 3);

            }
            pictureBox.Refresh();


            var segments = segmentGeneration.Current;
            
            

            foreach (var segment in segments)
            {
                Color c = Color.Gray;
                if ((segment.roadType as RoadType).weightRequirement != 0) c = Color.Black;
                g.DrawLines(new Pen(c, 1), segment.ConvertAll(x => new PointF(x.x, x.y)).ToArray());
            }
            g.Flush();
            pictureBox.Refresh();
        }*/
    }
}
