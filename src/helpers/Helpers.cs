using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver
{
    class Helpers
    {
        public static double EPSILON = 0.000001;

        public static double max(double x, double y)
        {
            if (x < y)
            {
                return y;
            }
            return x;
        }

        public static double min(double x, double y)
        {
            if (x < y)
            {
                return x;
            }
            return y;
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        public static bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= max(p.X, r.X) && q.X >= min(p.X, r.X) &&
                q.Y <= max(p.Y, r.Y) && q.Y >= min(p.Y, r.Y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        public static double orientation(Point p, Point q, Point r)
        {
            // See 10th slides from following link for derivation of the formula
            // http://www.dcs.gla.ac.uk/~pat/52233/slides/Geometry1x1.pdf
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0 || (val < EPSILON && val > -EPSILON)) return 0;  // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool doesIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            double o1 = orientation(p1, q1, p2);
            double o2 = orientation(p1, q1, q2);
            double o3 = orientation(p2, q2, p1);
            double o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        //get point 30 cm before, and after line
        public static Point[] boundingPoints(Point p, Point q)
        {
            Point[] points = new Point[2];

            double length = CountDistance(p, q);

            double newLength = length + 0.3; //length + 30cm

            double x = q.X + (q.X - p.X) / length*newLength;
            double y = q.Y + (q.Y - p.Y) / length*newLength;

            points[0] = new Point(x, y);
            x = p.X + (p.X - q.X) / length * newLength;
            y = p.Y + (p.Y - q.Y) / length * newLength;
            
            points[1] = new Point(x, y);

            return points;
        }

        public static double CountDistance(Point p, Point q) {
            return Math.Sqrt(Math.Pow(p.X-q.X, 2) + Math.Pow(p.Y-q.Y, 2));
        }
    }
}
