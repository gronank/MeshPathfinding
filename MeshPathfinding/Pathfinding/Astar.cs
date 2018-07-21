using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;
namespace MeshPathfinding.Pathfinding
{
    class Astar
    {
        public static List<PathNode> findPath(PathNode origin,PathNode destination)
        {
            Dictionary<PathNode, SolutionNode> visited = new Dictionary<PathNode, SolutionNode>();
            FastPriorityQueue<SolutionNode> queue = new FastPriorityQueue<SolutionNode>(5000);
            var startNode = new SolutionNode(origin);
            visited.Add(startNode.pathNode, startNode);
            queue.EnqueueNode(startNode);
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (visited[currentNode.pathNode].TotalDistance < currentNode.TotalDistance) continue;

                if (currentNode.pathNode== destination)
                {
                    return getPath(currentNode);
                }
                if (currentNode.pathNode.links.Any(x => x.TerrainType.name != "town"))
                {

                }
                foreach(var link in currentNode.pathNode.links)
                {
                    var solutionNode = new SolutionNode(currentNode, link, destination);
                    if (!visited.ContainsKey(solutionNode.pathNode))
                    {
                        visited.Add(solutionNode.pathNode, solutionNode);
                        queue.EnqueueNode(solutionNode);
                    }
                    else if(visited[solutionNode.pathNode].TotalDistance> solutionNode.TotalDistance)
                    {
                        visited[solutionNode.pathNode]= solutionNode;
                        queue.EnqueueNode(solutionNode);
                    }
                }
            }
            throw new Exception();
            
        }
        private static List<PathNode> getPath(SolutionNode node)
        {
            var path = new List<PathNode>();
            while (node != null)
            {
                path.Add(node.pathNode);
                node = node.parent;
            }
            return path;
        }
    }
}
