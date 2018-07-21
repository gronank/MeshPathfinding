using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMath
{
    [Serializable()]
    public class PointInt
    {
        public int x, y;


        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
            set
            {
                x *= (int)(value / Length);
                y *= (int)(value / Length);
            }
        }
        public float Angle
        {
            get
            {
                return (float)Math.Atan2(y, x);
            }
        }
        public PointInt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static PointInt operator +(PointInt p1, PointInt p2)
        {
            return new PointInt(p1.x + p2.x, p1.y + p2.y);
        }
        public static PointInt operator -(PointInt p1, PointInt p2)
        {
            return new PointInt(p1.x - p2.x, p1.y - p2.y);
        }
        public float Cross(PointInt p2)
        {
            return Cross(this, p2);
        }
        public static PointInt operator *(float scalar, PointInt p2)
        {
            return new PointInt((int)(scalar * p2.x), (int)(scalar * p2.y));
        }
        public static int Cross(PointInt p1, PointInt p2)
        {
            return p1.x * p2.y - p1.y * p2.x;
        }
        public static PointInt Polar(int length, int angle)
        {
            return new PointInt((int)(length * Math.Cos(angle)), (int)(length * Math.Sin(angle)));
        }
       /* public static explicit operator System.Drawing.PointF(PointInt p)
        {
            return new System.Drawing.PointF(p.x, p.y);
        }*/
        public static explicit operator Point(PointInt p)
        {
            return new Point(p.x, p.y);
        }
        public bool Equal(PointInt p)
        {
            return this.x == p.x && this.y == p.y;
        }
    }
    [Serializable()]
    public class Point
    {
        public float x, y;


        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
            set
            {
                x *= value / Length;
                y *= value / Length;
            }
        }
        public float Angle
        {
            get
            {
                return (float)Math.Atan2(y, x);
            }
        }
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static Point operator + (Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }
        public bool Equal(Point p2)
        {
            return this.x == p2.x && this.y == p2.y;
        }
        public float Cross(Point p2)
        {
            return Cross(this, p2);
        }
        public static Point operator *(float scalar,Point p2)
        {
            return new Point(scalar * p2.x, scalar * p2.y);
        }
        public static float Cross(Point p1, Point p2)
        {
            return p1.x * p2.y - p1.y * p2.x;
        }
        public static Point Polar(float length,float angle)
        {
            return new Point((float)(length * Math.Cos(angle)), (float)(length * Math.Sin(angle)));
        }
        /*public static explicit operator System.Drawing.PointF(Point p)
        {
            return new System.Drawing.PointF(p.x, p.y);
        }*/
    }
}
