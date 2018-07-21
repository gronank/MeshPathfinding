using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshPathfinding.Pathfinding
{
    [Serializable]
    class SolutionNode:Priority_Queue.FastPriorityQueueNode
    {
        public PathNode pathNode;
        public float distanceTravelled=0;
        public float distanceLeft=0;
        public SolutionNode parent;
        public SolutionNode(SolutionNode parent,PathLink link,PathNode destination)
            :this(link.destinationNode)
        {
            this.parent = parent;
            distanceTravelled = parent.distanceTravelled + link.getCost();
            distanceLeft = (pathNode.position - destination.position).Length;
        }
        public SolutionNode(PathNode pathNode)
        {
            this.pathNode = pathNode;
        }
        public float TotalDistance
        {
            get
            {
                return distanceTravelled + distanceLeft;
            }
        }
    }
}
