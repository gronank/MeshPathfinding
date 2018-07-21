using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MeshPathfinding
{
    [Serializable()]
    public class RegionData
    {
        public int biome;
        public int Id;
        public List<Point> Verteces;
        public List<Point> OuterEdge;
        public List<int> Indices=new List<int>();
        public List<List<Point>> Holes;
        internal RegionData(Region region)
        {
            Id = region.Id;
            Holes = region.holes.ConvertAll(x=>x.Border);
            OuterEdge = region.outerEdge;
            biome = region.biome;
            var mesh = region.graphicsMesh;
            var _verteces = mesh.Vertices.ToList();
            foreach(var tri in mesh.Triangles)
            {
                for(int i=0;i<3; i++)
                {
                    var vertex=tri.GetVertex(i);
                    Indices.Add(_verteces.IndexOf(vertex));
                }
            }
            Verteces = _verteces.ConvertAll(x => x.getPoint());
        }
    }
}
