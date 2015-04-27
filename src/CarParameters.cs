using System;

namespace CityDriver
{
    public class CarParameters
    {
        public double AngularVelocity;
        public int Id;
        public double[] Position = new double[2];
        public double Rotation;
        public double Velocity;

        public unsafe CarParameters(int id, double* position, double* rotation)
        {
            Id = id;
            for (var i = 0; i < 2; i++)
            {
                Position[i] = position[i];
            }
            Rotation = CountRotation(rotation[0], rotation[3]);
            Velocity = 0.0;
            AngularVelocity = 0.0;
        }

        public void SetNewPosition(double[] position, TimeSpan delta)
        {
            Velocity = CountVelocity(position, delta);
            for (var i = 0; i < 2; i++)
            {
                Position[i] = position[i];
            }
        }

        public void SetNewRotation(double rotation, TimeSpan delta)
        {
            AngularVelocity = CountAngularVelocity(rotation, delta);
            Rotation = rotation;
        }

        private double CountVelocity(double[] newPosition, TimeSpan delta)
        {
            return
                Math.Sqrt((newPosition[0] - Position[0])*(newPosition[0] - Position[0]) +
                          (newPosition[1] - Position[1])*(newPosition[1] - Position[1]))/delta.TotalSeconds;
        }

        private double CountAngularVelocity(double newRotation, TimeSpan delta)
        {
            var tmp = newRotation - Rotation;
            if (tmp > Math.PI)
            {
                return (tmp - 2*Math.PI)/delta.TotalSeconds;
            }
            if (tmp < -Math.PI)
            {
                return (tmp + 2*Math.PI)/delta.TotalSeconds;
            }
            return tmp/delta.TotalSeconds;
        }

        private double CountRotation(double w, double z)
        {
            return w*z >= 0 ? Math.Asin(Math.Abs(z)) : Math.Asin(Math.Abs(z)) + Math.PI;
        }
    }
}