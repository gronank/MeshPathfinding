using MyMath;
using System;

namespace MeshPathfinding.MapBuilding
{
    [Serializable]
    public class PopulationCentreConnection
    {
        internal PathNode node1;
        internal PathNode node2;
        public float weight;
        public RoadType roadType;
        public float Distance
        {
            get { return (node1.position - node2.position).Length; }
        }
        public Point Position1
        {
            get { return node1.position; }
        }
        public Point Position2
        {
            get { return node2.position; }
        }
        public PopulationCentreConnection(PopulationCentre populationCentre1, PopulationCentre populationCentre2)
        {
            this.node1 = populationCentre1.node;
            this.node2 = populationCentre2.node;
            
            weight = populationCentre1.getWeight(populationCentre2);
        }
        public bool Equal(PopulationCentreConnection other)
        {
            return (node1 == other.node1 && node2 == other.node2) || (node1 == other.node2 && node2 == other.node1);
        }
    }
}