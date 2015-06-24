using RoBOSSCommunicator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace CityDriver.drawing
{
    class DrawingManager
    {
        private double scale = 1.0f;
        private int MARGIN = 10;
        private double ROBOT_SIZE = 0.3;
        private double NODE_SIZE = 0.1;
        private double POINT_SIZE = 0.05;
        private System.Drawing.Graphics g;
        private System.Windows.Forms.Panel panel1;
        private Bitmap background;
        private List<double> boundaries;
        private Point translation;
        private Point to;

        public DrawingManager(System.Windows.Forms.Panel panel1, List<double> boundaries)
        {
            this.panel1 = panel1;
            Console.WriteLine("Draw manager initialized!");
            this.g = panel1.CreateGraphics();
            this.background = new Bitmap(this.panel1.Width, this.panel1.Height);
            this.boundaries = boundaries;

            this.translation = new Point(boundaries[0], boundaries[1]);
            this.to = new Point(Math.Abs(boundaries[2] - boundaries[0]), Math.Abs(boundaries[3] - boundaries[1]));

            //scale to fit width
            this.scale = Math.Min((this.panel1.Width / this.to.X) - 2 * MARGIN, (this.panel1.Height / this.to.Y) - 2 * MARGIN);
        }

        public void drawBackground()
        {
            Graphics g = Graphics.FromImage(this.background);

            this.g.DrawImage(this.background, 0, 0);
        }

        public void draw()
        {
            drawBackground();
        }

        public void clean()
        {
            System.Drawing.Point[] points = new System.Drawing.Point[4];

            points[0] = new System.Drawing.Point(0, 0);
            points[1] = new System.Drawing.Point(0, panel1.Height);
            points[2] = new System.Drawing.Point(panel1.Width, panel1.Height);
            points[3] = new System.Drawing.Point(panel1.Width, 0);

            Brush brush = new SolidBrush(Color.Black);
            this.g.FillPolygon(brush, points);
        }

        public void cleanBackground()
        {
            Graphics g = Graphics.FromImage(this.background);

            g.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.Black)), panel1.DisplayRectangle);

            System.Drawing.Point[] points = new System.Drawing.Point[4];

            points[0] = new System.Drawing.Point(0, 0);
            points[1] = new System.Drawing.Point(0, panel1.Height);
            points[2] = new System.Drawing.Point(panel1.Width, panel1.Height);
            points[3] = new System.Drawing.Point(panel1.Width, 0);

            Brush brush = new SolidBrush(Color.Black);
            g.FillPolygon(brush, points);
            
            this.g.DrawImage(this.background, 0, 0);
        }

        public void drawWalls(List<Wall> walls)
        {
            Graphics g = Graphics.FromImage(this.background);

            foreach (Wall wall in walls)
            {
                drawWall(g, wall);
            }

            this.g.DrawImage(this.background, 0, 0);
        }

        private void drawWall(Graphics g, Wall wall)
        {
            Pen greenPen = new Pen(Color.Green, 5);

            System.Drawing.Point point1 = new System.Drawing.Point(Convert.ToInt32(scale * wall.From.X), Convert.ToInt32(scale * wall.From.Y));
            System.Drawing.Point point2 = new System.Drawing.Point(Convert.ToInt32(scale * wall.To.X), Convert.ToInt32(scale * wall.To.Y));

            /*
            Console.WriteLine("Line:");
            Console.WriteLine("from:");
            Console.Write(scale*wall.From.X);
            Console.Write(" ");
            Console.Write(scale*wall.From.Y);
            Console.WriteLine("to:");
            Console.Write(scale*wall.To.X);
            Console.Write(" ");
            Console.Write(scale*wall.To.Y);
            Console.WriteLine("");*/

            // Draw line to screen.
            g.DrawLine(greenPen, point1, point2);
        }


        public void drawFuturePoints(List<Point> points)
        {
            Pen pen = new Pen(Color.Beige, 3);

            foreach (Point point in points)
            {
                int x = Convert.ToInt32(point.X * scale);
                int y = Convert.ToInt32(point.Y * scale);
                int radius = Convert.ToInt32(POINT_SIZE * scale);

                g.DrawEllipse(pen, x, y, radius, radius);
            }
        }

        public void drawNodes(List<Node> nodes)
        {
            Pen pen = new Pen(Color.Violet, 3);

            foreach (Node node in nodes)
            {
                int x = Convert.ToInt32(node.Position.X * scale);
                int y = Convert.ToInt32(node.Position.Y * scale);
                int radius = Convert.ToInt32(NODE_SIZE * scale);
                
                g.DrawEllipse(pen, x, y, radius, radius);
            }
        }

        public void drawRobot(Robot robot, RobotType type)
        {
            Pen pen;
            switch (type)
            {
                case RobotType.Normal:
                    pen = new Pen(Color.Gray, 5);
                    break;
                case RobotType.Selected:
                    pen = new Pen(Color.Green, 5);
                    break;
                case RobotType.Visible:
                    pen = new Pen(Color.Red, 5);
                    break;
                default:
                    pen = new Pen(Color.Yellow, 5);
                    break;    
            }
            unsafe
            {
                double* pos = robot.position;
                int x = Convert.ToInt32(pos[0] * scale);
                int y = Convert.ToInt32(pos[1] * scale);
                int radius = Convert.ToInt32(ROBOT_SIZE*scale);
                
                g.DrawEllipse(pen, x, y, radius, radius);
            }
        }
    }
}
