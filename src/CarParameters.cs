using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class CarParameters
    {
        public int Id;
        public double[] Position = new double[3];
        public double Rotation;
        public double Velocity;
        public double AngularVelocity;

        public unsafe CarParameters(int id, double* position, double* rotation)
        {
            Id = id;
            for (int i = 0; i < 3; i++)
            {
                Position[i] = position[i];
            }
            Rotation = CountRotation(rotation[0], rotation[3]);
            Velocity = 0.0;
            AngularVelocity = 0.0;
        }

        private double CountRotation(double w, double z)
        {
            return w * z >= 0 ? Math.Asin(Math.Abs(z)) : Math.Asin(Math.Abs(z)) + Math.PI;
        }
    }
}
