using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshPathfinding
{
    [Serializable]
    class PathLink
    {
        public PathLink reverseLink;
        public PathNode destinationNode;
        private TerrainType _terrainType;
        private PathArea pathArea;
        public TerrainType TerrainType
        {
            set {
                _terrainType = value;
                reverseLink._terrainType = value;
            }
            get
            {
                return _terrainType;
            }
        }
        private float length;
        internal PathLink(PathNode destination,TerrainType type,float length,PathArea pathArea)
        {
            destinationNode = destination;
            _terrainType = type;
            this.length = length;
            this.pathArea = pathArea;
        }

        internal void update(PathLink link)
        {
            pathArea = link.pathArea;
            _terrainType = link._terrainType;
        }

        public float getCost()
        {
            return length * _terrainType.cost;
        }

        internal List<Point> upgradeToRoad(TerrainType roadType)
        {
            return pathArea.createRoad(this, roadType);
        }

        internal void Remove()
        {
            var link2 = reverseLink;
            var node1 = link2.destinationNode;
            var node2 = destinationNode;
            node1.links.Remove(this);
            node2.links.Remove(link2);
        }
    }
}
