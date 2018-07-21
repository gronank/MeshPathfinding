using Fractal;
using MeshPathfinding;
using MeshPathfinding.MapBuilding;
using MyMath;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MapCreator
{
    class OptionsDataContext: INotifyPropertyChanged
    {
        public int SideLength
        {
            get { return blocks*128; }
        }
        public int Blocks
        {
            get { return blocks; }
            set {
                OnPropertyChanged("Blocks");
                OnPropertyChanged("SideLength");
                blocks = value;
            }
        }
        public float ForestSmallScale
        {
            get
            {
                return forestSmallScale;
            }

            set
            {
                OnPropertyChanged("ForestSmallScale");
                if (forestSmallScale > forestLargeScale)
                {
                    ForestLargeScale = value;
                    
                }
                forestSmallScale = value;
            }
        }
        public float ForestLargeScale
        {
            get
            {
                return forestLargeScale;
            }

            set
            {
                OnPropertyChanged("ForestLargeScale");
                forestLargeScale = value;
            }
        }

        public float TownSmallScale
        {
            get
            {
                return townSmallScale;
            }

            set
            {
                OnPropertyChanged("TownSmallScale");
                townSmallScale = value;
            }
        }

        public float TownLargeScale
        {
            get
            {
                return townLargeScale;
            }

            set
            {
                OnPropertyChanged("TownLargeScale");
                townLargeScale = value;
            }
        }

        public float ForestProportion
        {
            get
            {
                return forestProportion;
            }

            set
            {
                OnPropertyChanged("MaxTownProportion");
                OnPropertyChanged("ForestProportion");
                if (MaxTownProportion < TownProportion) TownProportion = MaxTownProportion;
                 forestProportion = value;
            }
        }

        public float MaxTownProportion
        {
            get
            {
                float max = 100 - forestProportion;
                if (max > 5) return 5;
                else return max;
            }
        }
        public float TownProportion
        {
            get
            {
                return townProportion;
            }

            set
            {
                OnPropertyChanged("TownProportion");
                townProportion = value;
            }
        }

        public string Seed
        {
            get
            {
                if (seed > 0)
                {
                    return seed.ToString();
                }else
                {
                    return "Random";
                }
            }

            set
            {
                
                if(!int.TryParse(value,out seed))
                {
                    seed = -1;
                }
                OnPropertyChanged("Seed");
            }
        }

        public DelegateCommand OnGenerateMap
        {
            get
            {
                return onGenerateMap;
            }

            set
            {
                onGenerateMap = value;
            }
        }
        public bool TownGenerationAvailable
        {
            get { return mapData != null; }
        }
        public bool RoadGenerationAvailable
        {
            get { return townData != null; }
        }
        public BitmapImage Map
        {
            get
            {
                if (mapData != null)
                {
                    MemoryStream stream = new MemoryStream();
                    mapData.image.Save(stream, ImageFormat.Png);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
                else
                {
                    return null;
                }
            }
        }
        public MapDataPacket MapData
        {
            get
            {
                return mapData;
            }

            set
            {
                
                mapData = value;
                OnPropertyChanged("MapData");
                OnPropertyChanged("Map");
                OnPropertyChanged("TownGenerationAvailable");
            }
        }

        public float TownPopulation
        {
            get
            {
                return townPopulation;
            }

            set
            {
                OnPropertyChanged("TownPopulation");
                townPopulation = value;
            }
        }

        public float VillagePopulation
        {
            get
            {
                return villagePopulation;
            }

            set
            {
                OnPropertyChanged("VillagePopulation");
                villagePopulation = value;
            }
        }

        public float SettlementSize
        {
            get
            {
                return settlementSize;
            }

            set
            {
                OnPropertyChanged("SettlementSize");
                settlementSize = value;
            }
        }

        public float HighwayDistance
        {
            get
            {
                return highwayDistance;
            }

            set
            {
                OnPropertyChanged("HighwayDistance");
                highwayDistance = value;
            }
        }

        public float TownRoadDistance
        {
            get
            {
                return townRoadDistance;
            }

            set
            {
                OnPropertyChanged("TownRoadDistance");
                townRoadDistance = value;
            }
        }

        public float VillageRoadDistance
        {
            get
            {
                return villageRoadDistance;
            }

            set
            {
                OnPropertyChanged("VillageRoadDistance");
                villageRoadDistance = value;
            }
        }

        public DelegateCommand OnGenerateTownData
        {
            get
            {
                return onGenerateTownData;
            }

            set
            {
                onGenerateTownData = value;
            }
        }

        internal TownData TownData
        {
            get
            {
                return townData;
            }

            set
            {
                townData = value;
                OnPropertyChanged("TownData");
                OnPropertyChanged("TownCanvas");
                OnPropertyChanged("RoadGenerationAvailable");
                OnPropertyChanged("EdgeCount");
            }
        }
        public Canvas TownCanvas
        {
            get {
                if (TownData != null) return TownData.townsCanvas;
                else return null;
            }
        }
        public int? EdgeCount
        {
            get
            {
                if (TownData != null) return TownData.populationConnections.Count;
                else return null;
            }
        }

        public DelegateCommand OnGenerateRoads
        {
            get
            {
                return onGenerateRoads;
            }

            set
            {
                onGenerateRoads = value;
            }
        }

        public LoadingContext GenerateRoadContext
        {
            get
            {
                return generateRoadContext;
            }

            set
            {
               
                generateRoadContext = value;
                OnPropertyChanged("GenerateRoadContext");
            }
        }

        public float FieldMoveCost
        {
            get
            {
                return fieldMoveCost;
            }

            set
            {
                fieldMoveCost = value;
                OnPropertyChanged("FieldMoveCost");
            }
        }

        public float ForestMoveCost
        {
            get
            {
                return forestMoveCost;
            }

            set
            {
                OnPropertyChanged("ForestMoveCost");
                forestMoveCost = value;
            }
        }

        public float TownMoveCost
        {
            get
            {
                return townMoveCost;
            }

            set
            {
                OnPropertyChanged("TownMoveCost");
                townMoveCost = value;
            }
        }

        public int MinimumFeatureSize
        {
            get
            {
                return minimumFeatureSize;
            }

            set
            {
                minimumFeatureSize = value;
                OnPropertyChanged("MinimumFeatureSize");
            }
        }

        //map generation
        int seed= 467709349;
        int blocks = 8;
        float forestSmallScale=5;
        float forestLargeScale=10;
        float townSmallScale=5;
        float townLargeScale=10;
        float forestProportion=30;
        float townProportion=0.5f;

        int minimumFeatureSize = 3;
        MapDataPacket mapData;

        //town generation
        float townPopulation=5;
        float villagePopulation=.5f;
        float settlementSize=15;
        float highwayDistance=500;
        float townRoadDistance=300;
        float villageRoadDistance=100;
        TownData townData;

        //road generation
        float fieldMoveCost = 2;
        float forestMoveCost = 3;
        float townMoveCost = 1.5f;
        LoadingContext generateRoadContext;
        void OnPropertyChanged(String prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        DelegateCommand onGenerateMap;
        DelegateCommand onGenerateTownData;
        DelegateCommand onGenerateRoads;
        public OptionsDataContext() {
            OnGenerateMap = new DelegateCommand(x => OnGenerateMapExecute());
            OnGenerateTownData = new DelegateCommand(x => OnGenerateTownDataExecute());
            OnGenerateRoads = new DelegateCommand(x => OnGenerateRoadsExecute());
        }
        public void OnGenerateRoadsExecute()
        {
            GenerateRoadContext = new LoadingContext(SolvePaths,100+townData.populationConnections.Count);
        }
        public void SolvePaths(object obj, DoWorkEventArgs a)
        {
            var worker = (obj as BackgroundWorker);
            var terrain = new List<TerrainType>();
            terrain.Add(new TerrainType()
            {
                cost = FieldMoveCost,
                name = "field"

            });
            terrain.Add(new TerrainType()
            {
                cost = ForestMoveCost,
                name="forest"
            });
            terrain.Add(new TerrainType()
            {
                cost = TownMoveCost,
                name="town"
            });
            worker.ReportProgress(0, "Extracting Regions");
            var world = new World(mapData.terrainMap,terrain);
            worker.ReportProgress(100, "Building mesh world");
            
            
            using (Stream stream = new FileStream("RegionData.bin",
                                     FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                var data = world.getWorldData();
                formatter.Serialize(stream, data);
                stream.Close();
            }
            
            var solver = world.getPathSolver();
            var iterator = townData.RoadIterator(worker, 100);
            
            solver.AddRoads(iterator);
            
            var g = Graphics.FromImage(MapData.image);

            foreach (var segment in solver.roadNetwork.roadSegments)
            {
                Color c = Color.Gray;
                if ((segment.roadType as RoadType).name == "highway") c = Color.Purple;
                g.DrawLines(new Pen(c, 1), segment.ConvertAll(x => new PointF(x.x, x.y)).ToArray());
            }
            MapData.image.Save("roadMap.png");
            OnPropertyChanged("Map");
            
            TownData = null;

        }
        
        public void OnGenerateMapExecute()
        {
            if (seed <= 0)
            {
                var seedGeneration = new Random();
                Seed = seedGeneration.Next().ToString();
            }
            Random r=new Random(seed);

            var townOptions = new FractalOptions(townSmallScale, townLargeScale);
            int n = blocks;
            var popFractal = new TiledFractal(128, n, n, townOptions, FractalWrapMode.NoWrap);
            popFractal.Initialize((uint)r.Next());
            var popMap = popFractal.getMap();
            var forestOptions = new FractalOptions(forestSmallScale, forestLargeScale);
            var forestFractal = new TiledFractal(128, n, n, forestOptions, FractalWrapMode.NoWrap);
            
            forestFractal.Initialize((uint)r.Next());
            
            var forestMap = forestFractal.getMap();
            var forestLevels = FractalUtility.getCutOffLevels(forestMap, new float[] { 100-forestProportion, forestProportion });
            var bitmap = new Bitmap(128 * n, 128 * n);
            var terrainMap = new int[128 * n, 128 * n];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    popMap[i, j] = popMap[i, j] - forestMap[i, j];
                }
            }
            var popLevels = FractalUtility.getCutOffLevels(popMap, new float[] { (100 - (forestProportion+townProportion)), townProportion });
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    popMap[i, j] = popMap[i, j] - popLevels[0];
                    if (popMap[i, j] < 0) popMap[i, j] = 0;
                    var pop = (popMap[i, j] / (popLevels[1] - popLevels[0]));
                    if (forestMap[i, j] > forestLevels[0])
                    {
                        terrainMap[i, j] = 2;
                        /*forestMap[i, j] -= forestLevels[0];
                        pop -= 1f;
                        var g = (int)(255 * forestMap[i, j] / (forestLevels[1] - forestLevels[0]));
                        bitmap.SetPixel(i, j, Color.FromArgb(0, 255, 0));*/
                    }
                    else if (pop >0)
                    {
                        terrainMap[i, j] = 3;
                        /*var b = (int)(255 * pop);
                        bitmap.SetPixel(i, j, Color.FromArgb(255, 0, 0));
                        */
                    }
                    else
                    {
                        terrainMap[i, j] = 1;
                        //bitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    //popMap[i, j] = pop;


                }
            }
            var popCentres=Sanitize(ref terrainMap);

            MapData = new MapDataPacket(terrainMap,popCentres);
            TownData = null;
            

        }

        private List<PopulationCentre> Sanitize(ref int[,] terrainMap)
        {
            List<PopulationCentre> popCentres = new List<PopulationCentre>();
            Queue<SanitationItem> startItems = new Queue<SanitationItem>();
            startItems.Enqueue(new SanitationItem(new PointInt(0, 0), terrainMap[0,0]));
            bool[,] visited = new bool[terrainMap.GetLength(0), terrainMap.GetLength(1)];
            while (startItems.Count > 0)
            {
                int size = 0;
                var startItem = startItems.Dequeue();
                var startPoint = startItem.position;
                if (!visited.inBounds(startPoint)||visited.getValue(startPoint)) continue;

                Queue<PointInt> fillQueue = new Queue<PointInt>();
                fillQueue.Enqueue(startPoint);
                while (fillQueue.Count > 0)
                {
                    var point = fillQueue.Dequeue();
                    if (visited.getValue(point)) continue;
                    visited[point.x, point.y] = true;
                    size++;
                    for (int i = 0; i < 6; i++)
                    {
                        var newPoint = point.Neighbor(i);
                        if (!visited.inBounds(newPoint)||visited.getValue(newPoint)) continue;
                        else if (terrainMap.sameAsNeighbor(point, i))
                        {
                            fillQueue.Enqueue(newPoint);
                        }
                        else
                        {
                            var val = terrainMap.getValue(point);
                            startItems.Enqueue(new SanitationItem(newPoint, val));
                        }
                    }
                }
                if (size < minimumFeatureSize)
                {
                    fill(ref terrainMap, startItem);
                }else if (terrainMap.getValue(startPoint) == 3)
                {
                    popCentres.Add(new PopulationCentre((MyMath.Point)(startPoint), size));
                }
                
            }
            return popCentres;
        }

        private void fill(ref int[,] terrainMap, SanitationItem startItem)
        {
            bool[,] visited = new bool[terrainMap.GetLength(0), terrainMap.GetLength(1)];
            Queue<PointInt> fillQueue = new Queue<PointInt>();
            fillQueue.Enqueue(startItem.position);
            while (fillQueue.Count > 0)
            {
                var point = fillQueue.Dequeue();
                if (visited.getValue(point)) continue;
                visited[point.x, point.y] = true;
                for (int i = 0; i < 6; i++)
                {
                    var newPoint = point.Neighbor(i);
                    if (visited.getValue(newPoint)) continue;
                    else if (terrainMap.sameAsNeighbor(point, i))
                    {
                        fillQueue.Enqueue(newPoint);
                    }
                }
                terrainMap[point.x, point.y] = startItem.defaultValue;
            }  
        }

        public void OnGenerateTownDataExecute()
        {
            TownData = new TownData(this);
        }
        
        public class MapDataPacket
        {
            public System.Drawing.Image image;
            public int[,] terrainMap;
            public List<PopulationCentre> popCentres;

            public MapDataPacket(int[,] terrainMap)
            {
                this.terrainMap = terrainMap;
                Bitmap bitmap = new Bitmap(terrainMap.GetLength(0), terrainMap.GetLength(1));
                for(int i = 0; i < terrainMap.GetLength(0); i++)
                {
                    for (int j = 0; j < terrainMap.GetLength(1); j++)
                    {
                        Color color;
                        switch (terrainMap[i, j])
                        {
                            case 1:
                                color = Color.White;
                                break;
                            case 2:
                                color = Color.Green;
                                break;
                            case 3:
                                color = Color.Magenta;
                                break;
                            default:
                                color = Color.Black;
                                break;
                        }
                        bitmap.SetPixel(i, j, color);
                    }
                }
                image = bitmap;
                bitmap.Save("map.png");
            }

            public MapDataPacket(int[,] terrainMap, List<PopulationCentre> popCentres) : this(terrainMap)
            {
                this.popCentres = popCentres;
            }
        }
        private class SanitationItem
        {
            public PointInt position;
            public int defaultValue;
            public SanitationItem(PointInt p,int d)
            {
                position = p;
                defaultValue = d;
            }
        }
    }
}
