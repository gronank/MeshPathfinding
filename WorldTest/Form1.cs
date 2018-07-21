using MeshPathfinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Bitmap map = (Bitmap)Bitmap.FromFile("map.png");
            var width = map.Width;
            var height = map.Height;
            int[,] biomeMap = new int[width, height];
            for(int i = 0; i< width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    biomeMap[i, j] = map.GetPixel(i, j).B;
                }
            }
            var world = new World(biomeMap,new List<TerrainType>());
        }
    }
}
