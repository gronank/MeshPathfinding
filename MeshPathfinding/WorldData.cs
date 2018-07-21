using MeshPathfinding.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshPathfinding
{
    [Serializable]
    public class WorldData
    {
        public List<RegionData> RegionData;
        public List<TerrainType> terrainTypes;
        public PathSolver pathSolver;
    }
}
