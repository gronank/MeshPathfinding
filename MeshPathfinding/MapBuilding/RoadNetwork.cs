using MyMath;
using System.Collections.Generic;
using System;
using System.Linq;
namespace MeshPathfinding.MapBuilding
{
    [Serializable]
    public class RoadNetwork
    {
        public List<RoadSegment> roadSegments = new List<RoadSegment>();
        internal void Add(List<PathNode> path, TerrainType roadType)
        {

            var originNode = path[0];
            var newSegmentPoints = new List<Point>();
            newSegmentPoints.Add(originNode.position);
            for ( int i = 1; i < path.Count; i++)
            {
                var newNode = path[i];
                var link = originNode.links.First(x => x.destinationNode == newNode);
                
                if (link.TerrainType is RoadType )
                {
                    if (newSegmentPoints.Count > 1)
                    {
                        bisectExistingSegments(originNode);
                        roadSegments.Add(new RoadSegment(newSegmentPoints, roadType));
                    }
                    newSegmentPoints = new List<Point>();
                    newSegmentPoints.Add(newNode.position);
                }
                else if (i == path.Count - 1)
                {
                    newSegmentPoints.AddRange(link.upgradeToRoad(roadType));
                    bisectExistingSegments(newNode);
                    roadSegments.Add(new RoadSegment(newSegmentPoints, roadType));
                }
                else
                {
                    newSegmentPoints.AddRange(link.upgradeToRoad(roadType));
                }
                originNode = newNode;
            }
            /*while (i < path.Count-1)
            {
                var node1 = path[i];
                var newSegmentPoints = new List<Point>();
                newSegmentPoints.Add(node1.position);
                while (i< path.Count-1)
                {
                    i++;
                    var node = path[i];
                    var link= path[i-1].links.First(x => x.destinationNode == node);
                    if(link.TerrainType is RoadType||i== path.Count - 1)
                    {
                        if(i == path.Count - 1)
                        {
                            newSegmentPoints.AddRange(link.upgradeToRoad(roadType));
                        }
                        if (newSegmentPoints.Count > 1)
                        {
                            roadSegments.Add(new RoadSegment(newSegmentPoints, roadType));
                            bisectExistingSegments(path[i - 1]);
                        }
                        break;
                    }
                    else
                    {
                        newSegmentPoints.AddRange(link.upgradeToRoad(roadType));
                        
                    }
                }
                //
                
            }*/
        }

        private void bisectExistingSegments(PathNode pathNode)
        {
            for(int i=0;i<roadSegments.Count;i++)
            {
                var segment = roadSegments[i];
                int cutIndex = segment.IndexOf(pathNode.position);
                if (cutIndex <= 0 || cutIndex >= segment.Count - 2) continue;
                var newPoints = segment.Take(cutIndex + 1).ToList();
                segment.RemoveRange(0, cutIndex);
                roadSegments.Add(new RoadSegment(newPoints, segment.roadType));
                break;
            }
        }
    }
    [Serializable]
    public class RoadSegment:List<Point>
    {
        public TerrainType roadType;

        internal RoadSegment(IEnumerable<Point> points,TerrainType roadType)
            :base(points)
        {
            if (points.Count() <= 1) throw new ArgumentException();
            this.roadType = roadType;
        }
    }
}