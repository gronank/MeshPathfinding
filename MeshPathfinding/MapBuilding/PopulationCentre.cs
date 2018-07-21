using MyMath;
using System;

namespace MeshPathfinding.MapBuilding
{
    [Serializable]
    public class PopulationCentre
    {
        internal PathNode node;
        public float population;
        public PopulationCentre(Point position,float pop)
        {
            node = new PathNode(position);
            population = pop;
        }
        public Point Position
        {
            get { return node.position; }
        }
        internal float getWeight(PopulationCentre other)
        {
            return population * other.population / (node.position - other.node.position).Length;
        }
    }
}