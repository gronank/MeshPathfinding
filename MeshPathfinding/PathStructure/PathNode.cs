using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MeshPathfinding
{
    [Serializable]
    class PathNode
    {
        public Point position;
        public List<PathLink> links = new List<PathLink>();
        public PathNode(TriangleNet.Geometry.Point p)
            :this(p.getPoint())
        {}
        public PathNode(Point p)
        {
            position = p;
        }
        public void Add(PathLink link)
        {
            PathLink existingLink = links.FirstOrDefault(x => x.destinationNode == link.destinationNode);
            if (existingLink == null)
            {
                links.Add(link);
            }
            else if (existingLink.TerrainType.cost > link.TerrainType.cost)
            { 
                existingLink.update(link);
            }
        }

        internal void Remove()
        {
            foreach(var link in links)
            {
                var node = link.destinationNode;
                node.links.Remove(link.reverseLink);
            }
        }

    }
    [Serializable]
    class NodeList:IEnumerable<PathNode>
    {
        InternalNodeList[,] lists;
        public NodeList(int width, int height,List<Region> regions)
            :this(width,height)
        {
            foreach (var region in regions)
            {
                foreach (var v in region.pathMesh.Vertices)
                {
                    Add(v);
                }
            }
        }
        public NodeList(int width, int height)
        {
            lists = new InternalNodeList[width, height];
        }
        public void Add(TriangleNet.Geometry.Vertex v)
        {
            AddAndGet(v);
        }
        public void Add(Point p)
        {
            AddAndGet(p);
        }
        public PathNode AddAndGet(TriangleNet.Geometry.Vertex v)
        {
            var p = v.getPoint();
            return AddAndGet(p);
        }
        public PathNode AddAndGet(PathNode p)
        {
            return AddAndGet(p.position);
        }
        public PathNode AddAndGet(Point p)
        {
            int x = (int)p.x;
            int y = (int)p.y;

            if (lists[x, y] == null) lists[x, y] = new InternalNodeList();
            return lists[x, y].AddAndGet(p);
        }
        public bool Contains(PathNode node)
        {
            int x = (int)node.position.x;
            int y = (int)node.position.y;
            if (lists[x, y] == null) return false;
            else return lists[x, y].Contains(node);
        }

        public IEnumerator<PathNode> GetEnumerator()
        {
            foreach (var list in lists)
            {
                if (list == null) continue;
                foreach(var item in list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    [Serializable]
    class InternalNodeList : List<PathNode>
    {
        public InternalNodeList()
        {

        }
        public void Add(TriangleNet.Geometry.Vertex v)
        {
            AddAndGet(v);
        }
        public void Add(Point p)
        {
            AddAndGet(p);
        }
        public PathNode AddAndGet(TriangleNet.Geometry.Vertex v)
        {
            var p = v.getPoint();
            return AddAndGet(p);
        }
        public PathNode AddAndGet(PathNode p)
        {
            return AddAndGet(p.position);
        }
        public PathNode AddAndGet(Point p)
        {
            var node = this.FirstOrDefault(x => (p-x.position).Length<0.1);
            if (node == null)
            {
                node = new PathNode(p);
                Add(node);
            }
            return node;
        }
        /*public PathNode AddAndGet(Point p)
        {

            int x = BitConverter.ToInt32(BitConverter.GetBytes(p.x), 0);
            int y = BitConverter.ToInt32(BitConverter.GetBytes(p.y), 0);

            long key=x<<32|y;
            PathNode node;
            this.TryGetValue(key, out node);

            if (node == null)
            {
                node = new PathNode(p);
                Add(key,node);
            }
            return node;
        }
        public bool Contains(Point p)
        {
            var key = getKey(p);
            return ContainsKey(key);
        }
        private long getKey(Point p)
        {
            int x = BitConverter.ToInt32(BitConverter.GetBytes(p.x), 0);
            int y = BitConverter.ToInt32(BitConverter.GetBytes(p.y), 0);

            long key = (long)x << 32 | y;
            return key;
        }*/

    }
}
